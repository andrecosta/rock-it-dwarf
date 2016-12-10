using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TileSpriteController : MonoBehaviour
{
    public Vector2 TilesetSize = new Vector2(224, 224);

    public Dictionary<Tile, GameObject> GeneratedTiles { get; private set; }

    private Sprite _floorSprite;
    private Dictionary<string, Sprite> _wallSprites;

    private int[] _indexes =
        {
             0,   4,  84,  92, 124, 116,  80,
            16,  28, 117,  95, 255, 253, 113,
            21,  87, 221, 127, 255, 247, 209,
            29, 125, 119, 199, 215, 213,  81,
            31, 255, 241,  20,  65,  17,   1,
            23, 223, 245,  85,  68,  93, 112,
             5,  71, 197,  69,  64,  7 , 193,
        };

    [Flags]
    enum BitField
    {
        N = 1,
        NE = 2,
        E = 4,
        SE = 8,
        S = 16,
        SW = 32,
        W = 64,
        NW = 128,
        HasNE = N | E,
        HasSE = S | E,
        HasSW = S | W,
        HasNW = N | W
    }

    void Awake()
    {
        // Load floor tile sprite
        _floorSprite = Resources.Load<Sprite>("Sprites/FloorTile");

        // Load wall sprites from tileset
        LoadWallsTileset();
    }

    void Start()
    {
        // Instantiate the floor tile GameObjects
        GeneratedTiles = new Dictionary<Tile, GameObject>();
        foreach (Tile tile in GameController.Instance.FloorTiles)
        {
            GameObject go = new GameObject("TILE FLOOR [" + tile.X + ", " + tile.Y + "]");
            go.transform.SetParent(transform);
            go.transform.localPosition = tile.Position;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = "Floor";
            sr.sprite = _floorSprite;

            // Register callbacks
            //tile.CallbackTileTypeChanged += OnTileTypeChanged;

            GeneratedTiles.Add(tile, go);
        }
        foreach (Tile tile in GameController.Instance.WallTiles)
        {
            GameObject go = new GameObject("TILE WALL [" + tile.X + ", " + tile.Y + "]");
            go.transform.SetParent(transform);
            go.transform.localPosition = tile.Position;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = "Walls";
            if (tile.Type == TileType.Wall)
                sr.sprite = GetSpriteFromTileset(tile);

            // Register callbacks
            //tile.CallbackTileTypeChanged += OnTileTypeChanged;

            GeneratedTiles.Add(tile, go);
        }

        // Map callback
        GameController.Instance.CallbackTileChanged += OnTileChanged;
    }

    void LoadWallsTileset()
    {
        _wallSprites = new Dictionary<string, Sprite>();

        Texture2D tileset = Resources.Load<Texture2D>("Textures/Tilesets/tileset_template");
        for (int y = (int)TilesetSize.y - 32, i = 0; y >= 0; y -= 32)
        {
            for (int x = 0; x < (int)TilesetSize.x; x += 32)
            {
                var s = Sprite.Create(tileset, new Rect(x, y, 32, 32), Vector2.zero, 32);
                if (!_wallSprites.ContainsKey(_indexes[i].ToString()))
                    _wallSprites.Add(_indexes[i].ToString(), s);
                i++;
            }
        }
    }

    void OnTileChanged(Tile tile)
    {
        if (GeneratedTiles.ContainsKey(tile) == false)
        {
            // Nope
            return;
        }

        var sr = GeneratedTiles[tile].GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            // Nope
            return;
        }

        switch (tile.Type)
        {
            case TileType.Floor:
                sr.sprite = _floorSprite;
                break;
            default:
                sr.sprite = null;
                break;
        }
    }

    public Sprite GetSpriteFromTileset(Tile tile)
    {
        BitField i = 0;

        Tile t = GameController.Instance.GetWallTileAt(tile.X + 0, tile.Y + 1); // Top
        if (t != null && t.Type == TileType.Wall)
            i |= BitField.N;

        t = GameController.Instance.GetWallTileAt(tile.X + 1, tile.Y + 0); // Right
        if (t != null && t.Type == TileType.Wall)
            i |= BitField.E;

        t = GameController.Instance.GetWallTileAt(tile.X + 0, tile.Y - 1); // Bottom
        if (t != null && t.Type == TileType.Wall)
            i |= BitField.S;

        t = GameController.Instance.GetWallTileAt(tile.X - 1, tile.Y + 0); // Left
        if (t != null && t.Type == TileType.Wall)
            i |= BitField.W;

        if ((i & BitField.HasNE) == BitField.HasNE)
        {
            t = GameController.Instance.GetWallTileAt(tile.X + 1, tile.Y + 1); // Top-right corner
            if (t != null && t.Type == TileType.Wall)
                i |= BitField.NE;
        }
        if ((i & BitField.HasSE) == BitField.HasSE)
        {
            t = GameController.Instance.GetWallTileAt(tile.X + 1, tile.Y - 1); // Bottom-right corner
            if (t != null && t.Type == TileType.Wall)
                i |= BitField.SE;
        }
        if ((i & BitField.HasSW) == BitField.HasSW)
        {
            t = GameController.Instance.GetWallTileAt(tile.X - 1, tile.Y - 1); // Bottom-left corner
            if (t != null && t.Type == TileType.Wall)
                i |= BitField.SW;
        }
        if ((i & BitField.HasNW) == BitField.HasNW)
        {
            t = GameController.Instance.GetWallTileAt(tile.X - 1, tile.Y + 1); // Top-left corner
            if (t != null && t.Type == TileType.Wall)
                i |= BitField.NW;
        }

        // Loop around
        if (i.GetHashCode() > 255)
            i -= 256;

        string ii = i.GetHashCode().ToString();

        if (!_wallSprites.ContainsKey(ii))
        {
            Debug.Log("NO SPRITE " + ii);
            return null;
        }

        return _wallSprites[ii];
    }
}

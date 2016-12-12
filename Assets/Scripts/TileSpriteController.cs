﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TileSpriteController : MonoBehaviour
{
    public string TilesetName;
    public string LavaTilesetName;
    public Vector2 TilesetSize = new Vector2(224, 224);

    public Dictionary<Tile, GameObject> GeneratedTiles { get; private set; }
    public Dictionary<Tile, SpriteRenderer> GeneratedShadows { get; private set; }

    private PlayerController _player;
    private List<Tile> _checkedTiles;

    private Sprite _floorSprite;
    private Dictionary<string, Sprite> _wallSprites;
    private Dictionary<string, Sprite> _lavaSprites;

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
        _floorSprite = Resources.Load<Sprite>("Sprites/Square");

        // Load wall sprites from tileset
        _wallSprites = new Dictionary<string, Sprite>();
        LoadTileset(TilesetName, ref _wallSprites);

        // Load lava sprites from tileset
        _lavaSprites = new Dictionary<string, Sprite>();
        LoadTileset("FullBrown_Lava", ref _lavaSprites);
    }

    void Start()
    {
        _checkedTiles = new List<Tile>();
        // Instantiate the floor tile GameObjects
        GeneratedTiles = new Dictionary<Tile, GameObject>();
        GeneratedShadows = new Dictionary<Tile, SpriteRenderer>();
        foreach (Tile tile in GameController.Instance.FloorTiles)
        {
            GameObject go = new GameObject("TILE FLOOR [" + tile.X + ", " + tile.Y + "]");
            go.transform.SetParent(transform);
            go.transform.localPosition = tile.Position;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = "Floor";
            sr.sprite = _floorSprite;
            sr.color = new Color32(41, 26, 14, 255);

            // Register callbacks
            //tile.CallbackTileChanged += OnTileChanged;

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
                sr.sprite = GetSpriteFromTileset(tile, ref _wallSprites);

            // Register callbacks
            //tile.CallbackTileChanged += OnTileChanged;

            GeneratedTiles.Add(tile, go);

            // Generate shadow tile
            GameObject shadowGo = new GameObject("SHADOW [" + tile.X + ", " + tile.Y + "]");
            shadowGo.transform.localPosition = tile.Position;
            sr = shadowGo.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = "LOS";
            sr.sprite = _floorSprite;
            sr.color = Color.black * tile.ShadowIntensity;
            GeneratedShadows.Add(tile, sr);
        }
        foreach (Tile tile in GameController.Instance.LavaTiles)
        {
            GameObject go = new GameObject("TILE LAVA [" + tile.X + ", " + tile.Y + "]");
            go.transform.SetParent(transform);
            go.transform.localPosition = tile.Position;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = "Lava";
            if (tile.Type == TileType.Lava)
                sr.sprite = GetSpriteFromTileset(tile, ref _lavaSprites);

            GeneratedTiles.Add(tile, go);
        }

        // Map callback
        GameController.Instance.CallbackTileChanged += OnTileChanged;

        _player = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        _checkedTiles.Clear();
        calculateShadows();
    }

    void LoadTileset(string tilesetName, ref Dictionary<string, Sprite> tilesetSprites)
    {
        Texture2D tileset = Resources.Load<Texture2D>("Textures/Tilesets/" + tilesetName);
        for (int y = (int)TilesetSize.y - 32, i = 0; y >= 0; y -= 32)
        {
            for (int x = 0; x < (int)TilesetSize.x; x += 32)
            {
                var s = Sprite.Create(tileset, new Rect(x, y, 32, 32), new Vector2(0.5f, 0.5f), 32);
                if (!tilesetSprites.ContainsKey(_indexes[i].ToString()))
                    tilesetSprites.Add(_indexes[i].ToString(), s);
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
            case TileType.Wall:
                sr.sprite = GetSpriteFromTileset(tile, ref _wallSprites);
                break;
            case TileType.Lava:
                sr.sprite = GetSpriteFromTileset(tile, ref _lavaSprites);
                break;
            default:
                sr.sprite = null;
                break;
        }

        Tile neighbor = GameController.Instance.GetWallTileAt(tile.X + 1, tile.Y);
        if (neighbor != null && neighbor.Type == TileType.Wall)
            GeneratedTiles[neighbor].GetComponent<SpriteRenderer>().sprite = GetSpriteFromTileset(neighbor, ref _wallSprites);
        neighbor = GameController.Instance.GetWallTileAt(tile.X - 1, tile.Y);
        if (neighbor != null && neighbor.Type == TileType.Wall)
            GeneratedTiles[neighbor].GetComponent<SpriteRenderer>().sprite = GetSpriteFromTileset(neighbor, ref _wallSprites);
        neighbor = GameController.Instance.GetWallTileAt(tile.X, tile.Y + 1);
        if (neighbor != null && neighbor.Type == TileType.Wall)
            GeneratedTiles[neighbor].GetComponent<SpriteRenderer>().sprite = GetSpriteFromTileset(neighbor, ref _wallSprites);
        neighbor = GameController.Instance.GetWallTileAt(tile.X, tile.Y - 1);
        if (neighbor != null && neighbor.Type == TileType.Wall)
            GeneratedTiles[neighbor].GetComponent<SpriteRenderer>().sprite = GetSpriteFromTileset(neighbor, ref _wallSprites);
        neighbor = GameController.Instance.GetWallTileAt(tile.X + 1, tile.Y + 1);
        if (neighbor != null && neighbor.Type == TileType.Wall)
            GeneratedTiles[neighbor].GetComponent<SpriteRenderer>().sprite = GetSpriteFromTileset(neighbor, ref _wallSprites);
        neighbor = GameController.Instance.GetWallTileAt(tile.X - 1, tile.Y - 1);
        if (neighbor != null && neighbor.Type == TileType.Wall)
            GeneratedTiles[neighbor].GetComponent<SpriteRenderer>().sprite = GetSpriteFromTileset(neighbor, ref _wallSprites);
        neighbor = GameController.Instance.GetWallTileAt(tile.X + 1, tile.Y - 1);
        if (neighbor != null && neighbor.Type == TileType.Wall)
            GeneratedTiles[neighbor].GetComponent<SpriteRenderer>().sprite = GetSpriteFromTileset(neighbor, ref _wallSprites);
        neighbor = GameController.Instance.GetWallTileAt(tile.X - 1, tile.Y + 1);
        if (neighbor != null && neighbor.Type == TileType.Wall)
            GeneratedTiles[neighbor].GetComponent<SpriteRenderer>().sprite = GetSpriteFromTileset(neighbor, ref _wallSprites);

        neighbor = GameController.Instance.GetLavaTileAt(tile.X + 1, tile.Y);
        if (neighbor != null && neighbor.Type == TileType.Lava)
            GeneratedTiles[neighbor].GetComponent<SpriteRenderer>().sprite = GetSpriteFromTileset(neighbor, ref _lavaSprites);
        neighbor = GameController.Instance.GetLavaTileAt(tile.X - 1, tile.Y);
        if (neighbor != null && neighbor.Type == TileType.Lava)
            GeneratedTiles[neighbor].GetComponent<SpriteRenderer>().sprite = GetSpriteFromTileset(neighbor, ref _lavaSprites);
        neighbor = GameController.Instance.GetLavaTileAt(tile.X, tile.Y + 1);
        if (neighbor != null && neighbor.Type == TileType.Lava)
            GeneratedTiles[neighbor].GetComponent<SpriteRenderer>().sprite = GetSpriteFromTileset(neighbor, ref _lavaSprites);
        neighbor = GameController.Instance.GetLavaTileAt(tile.X, tile.Y - 1);
        if (neighbor != null && neighbor.Type == TileType.Lava)
            GeneratedTiles[neighbor].GetComponent<SpriteRenderer>().sprite = GetSpriteFromTileset(neighbor, ref _lavaSprites);
        neighbor = GameController.Instance.GetLavaTileAt(tile.X + 1, tile.Y + 1);
        if (neighbor != null && neighbor.Type == TileType.Lava)
            GeneratedTiles[neighbor].GetComponent<SpriteRenderer>().sprite = GetSpriteFromTileset(neighbor, ref _lavaSprites);
        neighbor = GameController.Instance.GetLavaTileAt(tile.X - 1, tile.Y - 1);
        if (neighbor != null && neighbor.Type == TileType.Lava)
            GeneratedTiles[neighbor].GetComponent<SpriteRenderer>().sprite = GetSpriteFromTileset(neighbor, ref _lavaSprites);
        neighbor = GameController.Instance.GetLavaTileAt(tile.X + 1, tile.Y - 1);
        if (neighbor != null && neighbor.Type == TileType.Lava)
            GeneratedTiles[neighbor].GetComponent<SpriteRenderer>().sprite = GetSpriteFromTileset(neighbor, ref _lavaSprites);
        neighbor = GameController.Instance.GetLavaTileAt(tile.X - 1, tile.Y + 1);
        if (neighbor != null && neighbor.Type == TileType.Lava)
            GeneratedTiles[neighbor].GetComponent<SpriteRenderer>().sprite = GetSpriteFromTileset(neighbor, ref _lavaSprites);
    }

    public Sprite GetSpriteFromTileset(Tile tile, ref Dictionary<string, Sprite> tileset)
    {
        BitField i = 0;
        if (tile.Type == TileType.Wall)
        {
            Tile t = GameController.Instance.GetWallTileAt(tile.X + 0, tile.Y + 1); // Top
            if (t != null && t.Type == tile.Type)
                i |= BitField.N;

            t = GameController.Instance.GetWallTileAt(tile.X + 1, tile.Y + 0); // Right
            if (t != null && t.Type == tile.Type)
                i |= BitField.E;

            t = GameController.Instance.GetWallTileAt(tile.X + 0, tile.Y - 1); // Bottom
            if (t != null && t.Type == tile.Type)
                i |= BitField.S;

            t = GameController.Instance.GetWallTileAt(tile.X - 1, tile.Y + 0); // Left
            if (t != null && t.Type == tile.Type)
                i |= BitField.W;

            if ((i & BitField.HasNE) == BitField.HasNE)
            {
                t = GameController.Instance.GetWallTileAt(tile.X + 1, tile.Y + 1); // Top-right corner
                if (t != null && t.Type == tile.Type)
                    i |= BitField.NE;
            }
            if ((i & BitField.HasSE) == BitField.HasSE)
            {
                t = GameController.Instance.GetWallTileAt(tile.X + 1, tile.Y - 1); // Bottom-right corner
                if (t != null && t.Type == tile.Type)
                    i |= BitField.SE;
            }
            if ((i & BitField.HasSW) == BitField.HasSW)
            {
                t = GameController.Instance.GetWallTileAt(tile.X - 1, tile.Y - 1); // Bottom-left corner
                if (t != null && t.Type == tile.Type)
                    i |= BitField.SW;
            }
            if ((i & BitField.HasNW) == BitField.HasNW)
            {
                t = GameController.Instance.GetWallTileAt(tile.X - 1, tile.Y + 1); // Top-left corner
                if (t != null && t.Type == tile.Type)
                    i |= BitField.NW;
            }
        }
        else if (tile.Type == TileType.Lava)
        {
            Tile t = GameController.Instance.GetLavaTileAt(tile.X + 0, tile.Y + 1); // Top
            if (t != null && t.Type == tile.Type)
                i |= BitField.N;

            t = GameController.Instance.GetLavaTileAt(tile.X + 1, tile.Y + 0); // Right
            if (t != null && t.Type == tile.Type)
                i |= BitField.E;

            t = GameController.Instance.GetLavaTileAt(tile.X + 0, tile.Y - 1); // Bottom
            if (t != null && t.Type == tile.Type)
                i |= BitField.S;

            t = GameController.Instance.GetLavaTileAt(tile.X - 1, tile.Y + 0); // Left
            if (t != null && t.Type == tile.Type)
                i |= BitField.W;

            if ((i & BitField.HasNE) == BitField.HasNE)
            {
                t = GameController.Instance.GetLavaTileAt(tile.X + 1, tile.Y + 1); // Top-right corner
                if (t != null && t.Type == tile.Type)
                    i |= BitField.NE;
            }
            if ((i & BitField.HasSE) == BitField.HasSE)
            {
                t = GameController.Instance.GetLavaTileAt(tile.X + 1, tile.Y - 1); // Bottom-right corner
                if (t != null && t.Type == tile.Type)
                    i |= BitField.SE;
            }
            if ((i & BitField.HasSW) == BitField.HasSW)
            {
                t = GameController.Instance.GetLavaTileAt(tile.X - 1, tile.Y - 1); // Bottom-left corner
                if (t != null && t.Type == tile.Type)
                    i |= BitField.SW;
            }
            if ((i & BitField.HasNW) == BitField.HasNW)
            {
                t = GameController.Instance.GetLavaTileAt(tile.X - 1, tile.Y + 1); // Top-left corner
                if (t != null && t.Type == tile.Type)
                    i |= BitField.NW;
            }
        }

        string ii = i.GetHashCode().ToString();

        if (!tileset.ContainsKey(ii))
        {
            Debug.Log("NO SPRITE " + ii);
            return null;
        }

        return tileset[ii];
    }

    void calculateShadows()
    {
        foreach (var shadow in GeneratedShadows)
        {

            int playerX = (int)(_player.transform.position.x + 0.5f);
            int playerY = (int)(_player.transform.position.y + 0.5f);
            int tileX = (int)(shadow.Key.Position.x);
            int tileY = (int)(shadow.Key.Position.y);

            Tile testTile;

            int occusionValue = 0;
            float intensity = 0;

            // Line drawing algorithm
            int dx = (tileX - playerX);
            int dy = (tileY - playerY);
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (dx < 0) dx1 = -1; else if (dx > 0) dx1 = 1;
            if (dy < 0) dy1 = -1; else if (dy > 0) dy1 = 1;
            if (dx < 0) dx2 = -1; else if (dx > 0) dx2 = 1;
            int longest = Mathf.Abs(dx);
            int shortest = Mathf.Abs(dy);

            if (!(longest > shortest))
            {
                longest = Mathf.Abs(dy);
                shortest = Mathf.Abs(dx);
                if (dy < 0) dy2 = -1; else if (dy > 0) dy2 = 1;
                dx2 = 0;
            }

            int numerator = longest >> 1;

            int D = 2 * dy - dx;
            int y = playerY;
            int x = playerX;

            for (int i = 0; i <= longest; i++)
            {
                
                testTile = GameController.Instance.GetWallTileAt(x, y);
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
                if (testTile.Type == TileType.Empty)
                    occusionValue += 5;
            }

            Vector2 vectorFromPlayer = shadow.Key.Position - new Vector2(_player.transform.position.x, _player.transform.position.y);
            float distanceToPlayer = Vector3.SqrMagnitude(vectorFromPlayer);

            float multiplier = 0.53f - (Vector2.Dot(vectorFromPlayer.normalized, _player.getOrientation().normalized) * 0.5f);
            float shadowValue = distanceToPlayer * multiplier;

            if (shadowValue < 8)
            {
                shadowValue += occusionValue;
                /*if (distanceToPlayer < 2)
                    intensity = 0;
                else */
                    intensity = Mathf.InverseLerp(1, 8, shadowValue);
            }
            else
                intensity = 1;

            shadow.Value.color = Color.black * Mathf.Lerp(shadow.Value.color.a, intensity, Time.deltaTime * 4);
        }
    }

    private void _occlusion(int playerX, int playerY, int tileX, int tileY)
    {
        int occusionValue = 0;
        Tile testTile;
        int dx = tileX - playerX;
        int dy = tileY - playerY;
        int D = 2 * dy - dx;
        int y = playerY;

        for (int x = playerX; x < tileX; x++)
        {
            //Tile = GeneratedShadows[x,y] or WallTiles[x,y]
            testTile = GameController.Instance.GetWallTileAt(x,y);
            if (testTile.Type == TileType.Wall)
                occusionValue += 2;

            if (D > 0)
            {
                y = y + 1;
                D = D - dx;
            }
            D = D + dy;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TileSpriteController : MonoBehaviour
{
    public string TilesetName;
    public Vector2 TilesetSize = new Vector2(224, 224);

    public Dictionary<Tile, GameObject> GeneratedTiles { get; private set; }
    public Dictionary<Tile, SpriteRenderer> GeneratedShadows { get; private set; }

    private PlayerController _player;
    private List<Tile> _checkedTiles;

    private Sprite _floorSprite;
    private Sprite _shadowSprite;
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
        _floorSprite = Resources.Load<Sprite>("Sprites/Square");
        _shadowSprite = Resources.Load<Sprite>("Sprites/Shadow");

        // Load wall sprites from tileset
        LoadWallsTileset();
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
                sr.sprite = GetSpriteFromTileset(tile);

            // Register callbacks
            //tile.CallbackTileChanged += OnTileChanged;

            GeneratedTiles.Add(tile, go);

            // Generate shadow tile
            GameObject shadowGo = new GameObject("LIGHT [" + tile.X + ", " + tile.Y + "]");
            shadowGo.transform.localPosition = tile.Position;
            sr = shadowGo.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = "LOS";
            sr.sprite = _floorSprite;
            sr.color = Color.black * tile.ShadowIntensity;
            GeneratedShadows.Add(tile, sr);
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

    void LoadWallsTileset()
    {
        _wallSprites = new Dictionary<string, Sprite>();

        Texture2D tileset = Resources.Load<Texture2D>("Textures/Tilesets/" + TilesetName);
        for (int y = (int)TilesetSize.y - 32, i = 0; y >= 0; y -= 32)
        {
            for (int x = 0; x < (int)TilesetSize.x; x += 32)
            {
                var s = Sprite.Create(tileset, new Rect(x, y, 32, 32), new Vector2(0.5f, 0.5f), 32);
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
            case TileType.Wall:
                sr.sprite = GetSpriteFromTileset(tile);
                break;
            default:
                sr.sprite = null;
                break;
        }

        Tile neighbor = GameController.Instance.GetWallTileAt(tile.X + 1, tile.Y);
        if (neighbor != null && neighbor.Type == TileType.Wall)
            GeneratedTiles[neighbor].GetComponent<SpriteRenderer>().sprite = GetSpriteFromTileset(neighbor);
        neighbor = GameController.Instance.GetWallTileAt(tile.X - 1, tile.Y);
        if (neighbor != null && neighbor.Type == TileType.Wall)
            GeneratedTiles[neighbor].GetComponent<SpriteRenderer>().sprite = GetSpriteFromTileset(neighbor);
        neighbor = GameController.Instance.GetWallTileAt(tile.X, tile.Y + 1);
        if (neighbor != null && neighbor.Type == TileType.Wall)
            GeneratedTiles[neighbor].GetComponent<SpriteRenderer>().sprite = GetSpriteFromTileset(neighbor);
        neighbor = GameController.Instance.GetWallTileAt(tile.X, tile.Y - 1);
        if (neighbor != null && neighbor.Type == TileType.Wall)
            GeneratedTiles[neighbor].GetComponent<SpriteRenderer>().sprite = GetSpriteFromTileset(neighbor);
        neighbor = GameController.Instance.GetWallTileAt(tile.X + 1, tile.Y + 1);
        if (neighbor != null && neighbor.Type == TileType.Wall)
            GeneratedTiles[neighbor].GetComponent<SpriteRenderer>().sprite = GetSpriteFromTileset(neighbor);
        neighbor = GameController.Instance.GetWallTileAt(tile.X - 1, tile.Y - 1);
        if (neighbor != null && neighbor.Type == TileType.Wall)
            GeneratedTiles[neighbor].GetComponent<SpriteRenderer>().sprite = GetSpriteFromTileset(neighbor);
        neighbor = GameController.Instance.GetWallTileAt(tile.X + 1, tile.Y - 1);
        if (neighbor != null && neighbor.Type == TileType.Wall)
            GeneratedTiles[neighbor].GetComponent<SpriteRenderer>().sprite = GetSpriteFromTileset(neighbor);
        neighbor = GameController.Instance.GetWallTileAt(tile.X - 1, tile.Y + 1);
        if (neighbor != null && neighbor.Type == TileType.Wall)
            GeneratedTiles[neighbor].GetComponent<SpriteRenderer>().sprite = GetSpriteFromTileset(neighbor);
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

        string ii = i.GetHashCode().ToString();

        if (!_wallSprites.ContainsKey(ii))
        {
            Debug.Log("NO SPRITE " + ii);
            return null;
        }

        return _wallSprites[ii];
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
                    occusionValue += 2;
            }

            Vector2 vectorFromPlayer = shadow.Key.Position - new Vector2(_player.transform.position.x, _player.transform.position.y);
            float distanceToPlayer = Vector3.SqrMagnitude(vectorFromPlayer);

            float multiplier = 0.63f - (Vector2.Dot(vectorFromPlayer.normalized, _player.getOrientation().normalized) * 0.5f);
            float shadowValue = distanceToPlayer * multiplier;

            if (shadowValue < 5)
            {
                shadowValue += occusionValue;
                if (distanceToPlayer < 2)
                    intensity = 0;
                else
                    intensity = Mathf.InverseLerp(0, 5, shadowValue);
            }
            else
                intensity = 1;

            shadow.Value.color = Color.black * Mathf.Lerp(shadow.Value.color.a, intensity, Time.deltaTime * 2);
            //shadow.Value.color = Color.black * Mathf.Lerp(shadow.Value.color.a, 0, Time.deltaTime * 2);
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

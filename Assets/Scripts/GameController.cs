using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    // Singleton
    public static GameController Instance { get; private set; }
    public Tile[,] FloorTiles;
    public Tile[,] WallTiles;
    public Action<Tile> CallbackTileChanged { get; set; }
    private int Width;
    private int Height;

    void Awake()
    {
        // Singleton
        if (Instance != null)
        {
            Debug.LogError("Only one GameController allowed!");
            Destroy(gameObject);
        }
        Instance = this;

        // Generate some test tiles
        GenerateTestTiles(10, 10);
    }

    void GenerateTestTiles(int width, int height)
    {
        Width = width;
        Height = height;
        // Generate some floor tiles
        FloorTiles = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile t = new Tile(x, y);
                t.Type = TileType.Floor;
                t.CallbackTileChanged += OnTileChanged;
                FloorTiles[x, y] = t;
            }
        }

        // Generate some wall tiles
        WallTiles = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile t = new Tile(x, y);
                // Tile is only wall 50% of the time
                if (Random.value > 0.5f)
                    t.Type = TileType.Wall;
                t.CallbackTileChanged += OnTileChanged;
                WallTiles[x, y] = t;
            }
        }
    }

    void Update()
    {

    }

    public Tile GetTileAt(int x, int y)
    {
        if (x >= Width || x < 0 || y >= Height || y < 0)
            return null;

        return FloorTiles[x, y];
    }

    public Tile GetWallTileAt(int x, int y)
    {
        if (x >= Width || x < 0 || y >= Height || y < 0)
            return null;

        return WallTiles[x, y];
    }

    public Tile GetTileAt(float x, float y)
    {
        return GetTileAt((int) Mathf.Floor(x), (int) Math.Floor(y));
    }

    public Tile GetTileAt(Vector2 position)
    {
        return GetTileAt(position.x, position.y);
    }

    void OnTileChanged(Tile tile)
    {
        if (CallbackTileChanged != null)
            CallbackTileChanged(tile);
    }
}

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
    private int size = 10;
    private MapGenerator _map;

    void Awake()
    {
        // Singleton
        if (Instance != null)
        {
            Debug.LogError("Only one GameController allowed!");
            Destroy(gameObject);
        }
        Instance = this;

        _map = new MapGenerator(10,0,0,0,0);
        FloorTiles = _map.getFloors();
        foreach (Tile floor in FloorTiles)
        {
            Debug.Log("Floor Here");
        }
        WallTiles = _map.getWalls();

        // Generate some test tiles
    }

    void Update()
    {

    }

    public Tile GetTileAt(int x, int y)
    {
        if (x >= size || x < 0 || y >= size || y < 0)
            return null;

        return FloorTiles[x, y];
    }

    public Tile GetWallTileAt(int x, int y)
    {
        if (x >= size || x < 0 || y >= size || y < 0)
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

    public void OnTileChanged(Tile tile)
    {
        if (CallbackTileChanged != null)
            CallbackTileChanged(tile);
    }
}

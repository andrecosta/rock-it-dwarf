using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    // Singleton
    public static GameController Instance { get; private set; }

    //Map Generation Variables
    public int mapSize, mapTunnelAmmount, mapRoomAmmount;
    public float mapRandomness, mapEmptyArea;
    public Tile[,] FloorTiles;
    public Tile[,] WallTiles;

    public Action<Tile> CallbackTileChanged { get; set; }
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

        _map = new MapGenerator(mapSize, mapTunnelAmmount, mapRoomAmmount, mapEmptyArea, mapRandomness);
        FloorTiles = _map.getFloors();
        WallTiles = _map.getWalls();

        // Generate some test tiles
    }

    void Update()
    {

    }

    public Tile GetTileAt(int x, int y)
    {
        if (x >= mapSize || x < 0 || y >= mapSize || y < 0)
            return null;

        return FloorTiles[x, y];
    }

    public Tile GetWallTileAt(int x, int y)
    {
        if (x >= mapSize || x < 0 || y >= mapSize || y < 0)
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

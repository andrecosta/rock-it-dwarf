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
    public GameObject player;
    public Tile[,] FloorTiles;
    public Tile[,] WallTiles;
    public Tile[,] LavaTiles;
    public List<Tile> emptyTiles;

    public Action<Tile> CallbackTileChanged { get; set; }
    private MapGenerator _map;
    public GameObject enemy;
    public int enemyAmmount;
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
        emptyTiles = _map.getEmptyTiles();

        // Generate lava tiles
        LavaTiles = new Tile[mapSize, mapSize];
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Tile t = new Tile(x, y);
                t.Type = TileType.Empty;
                t.CallbackTileChanged += OnTileChanged;
                LavaTiles[x, y] = t;
            }
        }

        instantiateEnemies();
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

    public Tile GetLavaTileAt(int x, int y)
    {
        if (x >= mapSize || x < 0 || y >= mapSize || y < 0)
            return null;

        return LavaTiles[x, y];
    }

    public Tile GetTileAt(float x, float y)
    {
        return GetTileAt((int)Mathf.Round(x), (int)Math.Round(y));
    }

    public Tile GetWallTileAt(float x, float y)
    {
        return GetWallTileAt((int)Mathf.Round(x), (int)Math.Round(y));
    }

    public Tile GetLavaTileAt(float x, float y)
    {
        return GetLavaTileAt((int)Mathf.Round(x), (int)Math.Round(y));
    }

    public void OnTileChanged(Tile tile)
    {
        if (CallbackTileChanged != null)
            CallbackTileChanged(tile);
    }

    private void instantiateEnemies()
    {
        for (int i = 0; i < enemyAmmount; i++)
        {
            int random = Random.Range(0, (emptyTiles.Count - 1));
            Tile startingTile = emptyTiles[random];

            GameObject currEnemy = Instantiate(enemy, new Vector3(startingTile.X, startingTile.Y, 0), Quaternion.identity);
            AIController enemyAI = currEnemy.GetComponent<AIController>();
            enemyAI.setTile(startingTile);
        }
    }


}

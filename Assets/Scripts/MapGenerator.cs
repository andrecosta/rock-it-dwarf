using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator {

    //Private Variables
    private int _mapSize, _tunnelAmmount, _roomAmmount, _emptyArea;
    private float _mapRandomness;
    private Tile[,] _floorTiles;
    private Tile[,] _wallTiles;
   

    public MapGenerator(int size, int tunels, int rooms, int area, float random)
    {
        _mapSize = size;
        _tunnelAmmount = tunels;
        _roomAmmount = rooms;
        _emptyArea = area;
        _mapRandomness = random;

        _generateTiles(_mapSize);

    }

    public Tile[,] getFloors() { return _floorTiles; }
    public Tile[,] getWalls() { return _wallTiles; }

    private void _generateTiles(int size)
    {
        // Generate floor tiles
        _floorTiles = new Tile[size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Tile t = new Tile(x, y);
                t.Type = TileType.Floor;
                t.CallbackTileChanged += GameController.Instance.OnTileChanged;
                _floorTiles[x, y] = t;
            }
        }

        // Generate the wall tiles
        _wallTiles = new Tile[size, size];
        for (int x = 0; x < (size); x++)
        {
            for (int y = 0; y < (size); y++)
            {
                Tile t = new Tile(x, y);
                t.Type = TileType.Wall;
                t.CallbackTileChanged += GameController.Instance.OnTileChanged;
                _wallTiles[x, y] = t;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public class MapGenerator {

    //Private Variables
    private int _mapSize, _tunnelAmmount, _roomAmmount;
    private float _mapRandomness, _emptyArea;
    private Tile[,] _floorTiles;
    private Tile[,] _wallTiles;
   

    public MapGenerator(int size, int tunels, int rooms, float area, float random)
    {
        _mapSize = size;
        _tunnelAmmount = tunels;
        _roomAmmount = rooms;
        _emptyArea = area;
        _mapRandomness = random;

        _generateTiles(_mapSize);
        _generateSpace();

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

                // Check start area
                if ((x < 2 && y < 2) || (x < 2 && y > (size - 3)) || (x > (size - 3) && y < 2) || (x > (size - 3) && y > (size - 3)))
                    t.Type = TileType.Empty;
                else
                    t.Type = TileType.Wall;

                t.CallbackTileChanged += GameController.Instance.OnTileChanged;
                _wallTiles[x, y] = t;
            }
        }
    }

    private void _generateSpace()
    {
        int currArea = 0;
        int roomAmmount = 0;
        int corridorAmmount = 0;

        // Getting the Aproximate ocupation size of tunnels
        float tunnelArea = (_emptyArea * _mapSize * _mapSize) / 4;
        // Getting the Average tunnel area
        int averageTunnelSize = (int) tunnelArea / _tunnelAmmount;

        // Getting the Aproximate ocupation size of rooms
        float roomArea = (_emptyArea * _mapSize * _mapSize * 2) / 4;
        // Getting the Average room area
        int averageRoomSize = (int)tunnelArea / _roomAmmount;


        while ((currArea < (_emptyArea * _mapSize * _mapSize)) && (roomAmmount < _roomAmmount) && (corridorAmmount < _tunnelAmmount))
        {
            _insertTunnels(ref corridorAmmount, ref currArea);
            _insertRooms(ref roomAmmount, ref currArea);
            _insertRandom(ref currArea);

            Debug.Log("Current Area: " + currArea + " || TargetArea: " + _emptyArea * _mapSize * _mapSize);
        }
    }

    private void _insertTunnels(ref int corridorAmmount, ref int currArea)
    {
        if (corridorAmmount == _tunnelAmmount)
            return;

        corridorAmmount = _tunnelAmmount;
    }


    private void _insertRooms(ref int roomAmmount, ref int currArea)
    {
        if (roomAmmount == _roomAmmount)
            return;

        roomAmmount = _roomAmmount;

    }


    private void _insertRandom(ref int currArea)
    {
        int x, y;
        Tile currTile;

        //Getting the random tile
        x = Random.Range(0, _mapSize - 1);
        y = Random.Range(0, _mapSize - 1);

        // Checking if the tile will be too close to the starting areas
        if ((x < 3 && y < 3) || (x < 3 && y > (_mapSize - 4)) || (x > (_mapSize - 4) && y < 3) || (x > (_mapSize - 4) && y > (_mapSize - 4)))
            return;


        currTile = _wallTiles[x, y];

        //Checking if the tile is already empty
        if (currTile.Type == TileType.Empty)
            return;

        //Setting to empty
        currTile.Type = TileType.Empty;
        currArea += 1;
    }

}

using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public class MapGenerator {

    //Private Variables
    private int _mapSize, _tunnelAmmount, _roomAmmount, _enemyAmmount;
    private float _mapRandomness, _emptyArea;
    private Tile[,] _floorTiles;
    private Tile[,] _wallTiles;
    private List<Tile> _emptyTiles;
    private GameObject _enemyPrefab;
   

    public MapGenerator(int size, int tunels, int rooms, float area, float random)
    {
        _mapSize = size;
        _emptyTiles = new List<Tile>();
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
                    t.Type = TileType.Wall;
                else
                    t.Type = TileType.Empty;

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
        int averageRoomSize;
        int averageTunnelSize;

        // Getting the Aproximate ocupation size of tunnels
        float tunnelArea = (_emptyArea * _mapSize * _mapSize) / 2;
        if (_roomAmmount == 0)
            tunnelArea *= 3;


        // Getting the Average tunnel area
        if (_tunnelAmmount > 0)
            averageTunnelSize = (int)tunnelArea / _tunnelAmmount;
        else
            averageTunnelSize = 0;

        // Getting the Aproximate ocupation size of rooms
        float roomArea = (_emptyArea * _mapSize * _mapSize * 3) / 4;
        if (_tunnelAmmount == 0)
            roomArea *= (3/2);


        // Getting the Average room area
        if (_roomAmmount > 0)
            averageRoomSize = (int)tunnelArea / _roomAmmount;
        else
            averageRoomSize = 0;


        //
        //
        while ((currArea < (int)(_emptyArea * _mapSize * _mapSize)) || (roomAmmount < _roomAmmount) || (corridorAmmount < _tunnelAmmount))
        {
            _insertTunnels(ref corridorAmmount, ref currArea, averageTunnelSize);
            _insertRooms(ref roomAmmount, ref currArea, averageRoomSize);
            _insertRandom(ref currArea);
        }
    }



    private void _insertTunnels(ref int corridorAmmount, ref int currArea, int averageTunnelSize)
    {
        if (corridorAmmount == _tunnelAmmount)
            return;

        while (corridorAmmount < _tunnelAmmount)
        {
            int corridorSize = 1;
            int tileTrials = 0;
            int firstTrials = 0;
            bool completed = false;
            int targetTunnelSize = averageTunnelSize + (int)((Random.Range(-1f, 1f) * averageTunnelSize) / 2);

            //Getting the starting tile
            Tile startTile = _getTile();
            startTile.Type = TileType.Wall;
            _emptyTiles.Add(startTile);
            Tile currTile = startTile;
            currArea++;
            //Getting random direction
            Vector2 direction = getNewDirection();

            while (!completed)
            {
                int tileX = (int)(Mathf.Clamp(direction.x + currTile.X + 0.5f, 0, _mapSize - 1));
                int tileY = (int)(Mathf.Clamp(direction.y + currTile.Y + 0.5f, 0, _mapSize - 1));
                if (checkTile(tileX, tileY))
                {
                    tileTrials = 0;
                    firstTrials = 0;
                    currTile = _wallTiles[tileX, tileY];
                    currTile.Type = TileType.Wall;
                    _emptyTiles.Add(currTile);
                    corridorSize++;
                    currArea++;

                    if (Random.Range(1, 100) < _mapRandomness * 100)
                    {
                        //Getting random direction
                        direction = getNewDirection();
                    }

                    // If too small, continue
                    if (corridorSize >=  targetTunnelSize)
                        completed = true;
                }
                else
                {
                    tileTrials++;
                    direction = getNewDirection();
                }

                if (tileTrials > 3)
                {
                    firstTrials++;
                    direction = getNewDirection();
                    currTile = startTile;
                }

                if (firstTrials > 3)
                completed = true; 
                    
            } 
            corridorAmmount++;
        } 
       
    }


    private void _insertRooms(ref int roomAmmount, ref int currArea, int averageRoomSize)
    {
        if (roomAmmount == _roomAmmount)
            return;


        while (roomAmmount < _roomAmmount)
        {
            int roomSize = 1;
            int targetRoomSize = averageRoomSize + (int)((Random.Range(-1f, 1f) * averageRoomSize) / 2);
            int averageHeight = (int)(Mathf.Sqrt(targetRoomSize) + 0.5f);

            int roomWidth = (int)(averageHeight + Random.Range( 2 - averageHeight, averageHeight - 2) + 0.5f);
            int roomHeight = (int)((averageHeight - (roomWidth - averageHeight))  + Random.Range(-1, 1) + 0.5f);

            int startX = Random.Range(3, _mapSize - 4 - roomWidth);
            int startY = Random.Range(3, _mapSize - 4 - roomHeight);

            //Debug.Log("Width: " + roomWidth + " Height: " + roomHeight + " X: " + startX + " Y: " + startY);

            for (int i = 0; i < roomWidth; i++)
            {
                for (int j = 0; j < roomHeight; j++)
                {
                    if (checkTile(i + startX, j + startY))
                    {
                        roomSize++;
                        currArea++;
                        _wallTiles[i + startX, j + startY].Type = TileType.Wall;
                        Tile currTile = _wallTiles[i + startX, j + startY];
                        _emptyTiles.Add(currTile);
                    }
                }
            }
            roomAmmount++;
        }
    }

    

    private void _insertRandom(ref int currArea)
    {
        Tile newTile;
        newTile = _getTile();
        //Debug.Log("CurrArea: " + currArea + " Target: " + _emptyArea * _mapSize * _mapSize);
        //Setting to empty
        newTile.Type = TileType.Wall;
        _emptyTiles.Add(newTile);
        currArea += 1;
    }


    private Tile _getTile()
    {
        int x, y;
        Tile currTile = null;

        while (currTile == null)
        {
            //Getting the random tile
            x = Random.Range(0, _mapSize - 1);
            y = Random.Range(0, _mapSize - 1);

            // Checking if the tile will be too close to the starting areas
            if ((x < 3 && y < 3) || (x < 3 && y > (_mapSize - 4)) || (x > (_mapSize - 4) && y < 3) || (x > (_mapSize - 4) && y > (_mapSize - 4)))
                continue;

            //Checking if the tile is already empty
            if (_wallTiles[x, y].Type == TileType.Wall)
                continue;

            currTile = _wallTiles[x, y];
        }
        return currTile;
    }

    private Vector2 getNewDirection()
    {
        Vector2 directionVector;

        int randomDirection = Random.Range(1, 4);
        switch (randomDirection)
        {
            case 1:
                directionVector = Vector2.up;
                break;
            case 2:
                directionVector = Vector2.right;
                break;
            case 3:
                directionVector = Vector2.down;
                break;
            default:
                directionVector = Vector2.left;
                break;
        }

        return directionVector; 
    }

    private bool checkTile(int x, int y)
    {
        Tile currTile = _wallTiles[x, y];

        if ((x < 3 && y < 3) || (x < 3 && y > (_mapSize - 4)) || (x > (_mapSize - 4) && y < 3) || (x > (_mapSize - 4) && y > (_mapSize - 4)))
            return false;

        if (_wallTiles[x, y].Type == TileType.Wall)
            return false;

        return true;
    }

    public List<Tile> getEmptyTiles()
    {
        return _emptyTiles;
    }
}

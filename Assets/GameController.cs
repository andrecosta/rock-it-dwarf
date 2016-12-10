using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    // Singleton
    public static GameController Instance { get; private set; }
    public Tile[,] FloorTiles;
    public Tile[,] WallTiles;
    public Action<Tile> CallbackTileChanged { get; set; }
    int Width = 10;
    int Height = 10;

    void Awake () {
        if (Instance != null)
        {
            Debug.LogError("Only one GameController allowed!");
            Destroy(gameObject);
        }
        Instance = this;

        FloorTiles = new Tile[10, 10];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tile t = new Tile(x, y);
                t.CallbackTileChanged += OnTileChanged;
                FloorTiles[x, y] = t;
            }
        }
    }
	
	void Update () {
		
	}

    public Tile GetTileAt(int x, int y)
    {
        if (x >= Width || x < 0 || y >= Height || y < 0)
            return null;

        return FloorTiles[x, y];
    }
    public Tile GetTileAt(float x, float y)
    {
        return GetTileAt((int)Math.Floor(x), (int)Math.Floor(y));
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

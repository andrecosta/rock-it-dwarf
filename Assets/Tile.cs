using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{

    public enum TileType
    {
        Wall,
        Floor
    }

    private TileType _type;
    public TileType Type
    {
        get { return _type; }
        set
        {
            // Do nothing if the new tile type is the same as the old one
            if (_type == value)
                return;

            _type = value;

            if (CallbackTileChanged != null)
                CallbackTileChanged(this);
        }
    }

    public int X { get; private set; }
    public int Y { get; private set; }

    public Vector2 Position
    {
        get { return new Vector2(X, Y); }
    }

    // Callbacks
    public Action<Tile> CallbackTileChanged { get; set; }

    public Tile(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool IsNeighbour(Tile tile, bool includeCornerTiles = false)
    {
        return Math.Abs(tile.X - X) + Math.Abs(tile.Y - Y) == 1 ||
                (includeCornerTiles && Math.Abs(tile.X - X) == 1 && Math.Abs(tile.Y - Y) == 1);
    }
}

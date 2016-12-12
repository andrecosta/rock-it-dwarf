using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LavaPool
{
    private List<Tile> _poolTiles;
    private bool _isExpanding;
    private float _poolLifetimeTimer;
    private float _poolTickTimer;
    private float _tickDuration;

    public LavaPool(float lifetimeDuration, float tickDuration, ref Tile lavaSource)
    {
        _poolTiles = new List<Tile>();
        _poolTiles.Add(lavaSource);
        _isExpanding = true;
        _poolLifetimeTimer = lifetimeDuration;
        _poolTickTimer = tickDuration;
        _tickDuration = tickDuration;
    }

    public void Simulate()
    {
        // Timer for the pool lifetime. When it reaches the end, the pool stops expanding
        if (_poolLifetimeTimer > 0)
            _poolLifetimeTimer -= Time.deltaTime;
        else
            _isExpanding = false;

        // Timer for each pool expansion or shrinking step
        if (_poolTickTimer > 0)
            _poolTickTimer -= Time.deltaTime;
        else
        {
            if (_isExpanding)
                Expand();
            else
                Shrink();

            _poolTickTimer = _tickDuration;
        }
    }

    void Expand()
    {
        //Tile[] emptyLavaTiles = GameController.Instance.LavaTiles.Cast<Tile>().Where(t => t.Type == TileType.Lava).ToArray();
        List<Tile> tilesToAdd = new List<Tile>();
        foreach (Tile lavaTile in _poolTiles)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (Mathf.Abs(x) + Mathf.Abs(y) > 1)
                        continue;

                    Tile tile = GameController.Instance.GetTileAt(lavaTile.X + x, lavaTile.Y + y);
                    if (tile != null && tile.Type == TileType.Terrain)
                    {
                        Tile newLavaTile = GameController.Instance.GetLavaTileAt(tile.X, tile.Y);

                        // Skip tile if it already is lava
                        if (newLavaTile.Type == TileType.Lava)
                            continue;

                        newLavaTile.Type = TileType.Lava;
                        tilesToAdd.Add(newLavaTile);
                    }
                }
            }
        }
        foreach (Tile tile in tilesToAdd)
            _poolTiles.Add(tile);

        Debug.Log("Expanding lava pool...");
    }

    void Shrink()
    {
        //Tile[] lavaTiles = GameController.Instance.LavaTiles.Cast<Tile>().Where(t => t.Type == TileType.Lava).ToArray();
        Tile tileToRemove = null;
        foreach (Tile lavaTile in _poolTiles)
        {
            lavaTile.Type = TileType.NoLava;
            //GameController.Instance.GetTileAt(lavaTile.X, lavaTile.Y).Type = TileType.Empty;
            tileToRemove = lavaTile;
            break;
        }
        if (_poolTiles.Contains(tileToRemove))
            _poolTiles.Remove(tileToRemove);

        Debug.Log("Evaporating lava pool...");
    }

    public bool IsEvaporated()
    {
        return _poolTiles.Count == 0;
    }
}

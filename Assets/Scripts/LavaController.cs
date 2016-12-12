using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LavaController : MonoBehaviour
{
    public bool GenerateLavaPools = true;
    public float LavaPoolLifetime = 10.0f;
    public float LavaPoolTickDuration = 1.0f;

    private List<LavaPool> LavaPools;

    void Start()
    {
        LavaPools = new List<LavaPool>();
    }

    void Update()
    {
        if (!GenerateLavaPools)
            return;

        LavaPool poolToRemove = null;
        foreach (LavaPool pool in LavaPools)
        {
            pool.Simulate();

            // Remove pool if it has no more lava tiles
            if (pool.IsEvaporated())
                poolToRemove = pool;
        }

        if (LavaPools.Contains(poolToRemove))
        {
            LavaPools.Remove(poolToRemove);
            Debug.Log(">> Lava pool evaporated");
        }
    }

    public void GenerateLavaPool()
    {
        // Generate a small amount of lava tiles
        var availableTiles =
            GameController.Instance.LavaTiles.Cast<Tile>()
                .Where(
                    t =>
                        t.Type == TileType.NoLava &&
                        GameController.Instance.GetTileAt(t.X, t.Y).Type == TileType.Terrain)
                .ToArray();
        if (availableTiles.Length == 0)
        {
            Debug.Log("No available tiles left to place lava source on!");
            return;
        }

        Debug.Log(">> Creating 1 new lava source on the terrain...");

        Tile lavaSource = availableTiles[Random.Range(0, availableTiles.Length)];
        lavaSource.Type = TileType.Lava;

        LavaPool pool = new LavaPool(LavaPoolLifetime, LavaPoolTickDuration, ref lavaSource);
        LavaPools.Add(pool);
    }
}

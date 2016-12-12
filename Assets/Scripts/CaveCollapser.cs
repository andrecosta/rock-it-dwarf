using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CaveCollapser : MonoBehaviour
{
    public int SecondsUntilCollapse = 30;
    public int SecondsUntilLavaSpread = 5;
    public AudioClip WarningSound;
    public AudioClip CollapseSound;

    private float _collapseTimer;
    private bool _collapseWarning;
    private float _lavaTimer;

    void Start()
    {
        _collapseTimer = SecondsUntilCollapse;
        _lavaTimer = SecondsUntilLavaSpread;
    }

    void Update()
    {
        if (_collapseTimer > 0)
        {
            _collapseTimer -= Time.deltaTime;
            if (_collapseTimer < 6)
            {
                Camera.main.GetComponent<CameraShake>().ShakeCamera(Time.deltaTime*0.1f, Time.deltaTime*0.1f);
                if (!_collapseWarning)
                {
                    AudioSource.PlayClipAtPoint(WarningSound, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y));
                    _collapseWarning = true;
                }
            }
        }
        else
        {
            Collapse();
            AudioSource.PlayClipAtPoint(CollapseSound, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y));
            _collapseTimer = SecondsUntilCollapse;
            _collapseWarning = false;
        }

        if (_lavaTimer > 0)
            _lavaTimer -= Time.deltaTime;
        else
        {
            SpreadLava();
            _lavaTimer = SecondsUntilLavaSpread;
        }
    }

    void Collapse()
    {
        Tile[] openTiles = GameController.Instance.FloorTiles.Cast<Tile>().Where(t => t.Type == TileType.Wall).ToArray();
        if (openTiles.Length == 0)
        {
            Debug.Log("No open tiles left!");
            return;
        }

        Debug.Log("Collapsing 10% of open tiles...");

        foreach (Tile tile in openTiles)
        {
            if (Random.value <= 0.1f)
            {
                tile.Type = TileType.Empty;
                GameController.Instance.GetLavaTileAt(tile.X, tile.Y).Type = TileType.Empty;
            }
        }

        openTiles =
            GameController.Instance.LavaTiles.Cast<Tile>()
                .Where(
                    t =>
                        t.Type == TileType.Empty &&
                        GameController.Instance.GetWallTileAt(t.X, t.Y).Type == TileType.Wall)
                .ToArray();
        if (openTiles.Length == 0)
        {
            Debug.Log("No open tiles left!");
            return;
        }

        Debug.Log("Placing lava on 1% of open tiles...");

        foreach (Tile tile in openTiles)
        {
            if (Random.value <= 0.01f)
                tile.Type = TileType.Lava;
        }
    }

    void SpreadLava()
    {
        Tile[] lavaTiles = GameController.Instance.LavaTiles.Cast<Tile>().Where(t => t.Type == TileType.Lava).ToArray();
        foreach (Tile lavaTile in lavaTiles)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (Mathf.Abs(x) + Mathf.Abs(y) > 1)
                        continue;

                    Tile tile = GameController.Instance.GetWallTileAt(lavaTile.X + x, lavaTile.Y + y);
                    if (tile != null && tile.Type == TileType.Wall)
                    {
                        GameController.Instance.GetLavaTileAt(tile.X, tile.Y).Type = TileType.Lava;
                    }
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CaveCollapser : MonoBehaviour
{
    public int SecondsUntilCollapse = 30;
    public AudioClip WarningSound;
    public AudioClip CollapseSound;

    private float _collapseTimer;
    private bool _collapseWarning;

    void Start()
    {
        _collapseTimer = SecondsUntilCollapse;
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
    }

    void Collapse()
    {
        Tile[] openTiles = GameController.Instance.WallTiles.Cast<Tile>().Where(t => t.Type == TileType.Wall).ToArray();
        if (openTiles.Length == 0)
        {
            Debug.Log("No open tiles left!");
            return;
        }

        Debug.Log("Collapsing 10% of open tiles...");

        foreach (Tile tile in openTiles)
        {
            if (Random.value > 0.1f)
                tile.Type = TileType.Empty;
        }
    }
}

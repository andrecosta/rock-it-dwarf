using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {

    public Tile targetTile;
    private Vector3 _shootDirection;
    private GameController _gc;
    private float despawnTime = 5;

    // Use this for initialization
    void Start () {
        _gc = GameController.Instance;
        transform.position = new Vector3(Mathf.Round(transform.position.x * 10) / 10, Mathf.Round(transform.position.y * 10) / 10);
        _shootDirection = (new Vector3(targetTile.X - transform.position.x, targetTile.Y - transform.position.y, 0).normalized);
    }

    // Update is called once per frame
    void Update()
    {

        transform.position += _shootDirection * Time.deltaTime * 4;
        Tile tile = GameController.Instance.GetTileAt(transform.position.x, transform.position.y);
        if (tile != null && tile.Type == TileType.Empty)
        {
            _shootDirection = Vector3.zero;
            Destroy(gameObject, 5);
        }

        if (Vector3.Distance(_gc.player.transform.position, transform.position) < 0.2f)
        {
            _shootDirection = Vector3.zero;
            Destroy(gameObject, 5);
            _gc.PlayerDeath();
        }
    }
}

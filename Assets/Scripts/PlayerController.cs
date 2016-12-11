using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Tile _currentTile;
    private Tile _targetTile;
    private float _moveTimer;
    private Vector2 _movementDirection;

    void Start()
    {
        foreach (var tile in GameController.Instance.WallTiles)
        {
            if (tile.Type == TileType.Wall)
            {
                _currentTile = tile;
                _targetTile = _currentTile;
            }
        }
        transform.position = _currentTile.Position;
        Camera.main.transform.position = transform.position + Vector3.forward * -10;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (Vector2.Distance(transform.position, _targetTile.Position) >= 0.01f)
            transform.position = Vector3.MoveTowards(transform.position, _targetTile.Position, Time.deltaTime * 2);
            
        if (Vector2.Distance(transform.position, _targetTile.Position) < 0.05f)
            _currentTile = _targetTile;

        if (_currentTile != _targetTile)
            return;

        if (h > 0)
        {
            Tile tile = GameController.Instance.GetWallTileAt(transform.position.x + 1, transform.position.y);
            if (tile != null && tile.Type == TileType.Wall)
                _targetTile = tile;
        }
        else if (h < 0)
        {
            Tile tile = GameController.Instance.GetWallTileAt(transform.position.x - 1, transform.position.y);
            if (tile != null && tile.Type == TileType.Wall)
                _targetTile = tile;
        }
        else if (v > 0)
        {
            Tile tile = GameController.Instance.GetWallTileAt(transform.position.x, transform.position.y + 1);
            if (tile != null && tile.Type == TileType.Wall)
                _targetTile = tile;
        }
        else if (v < 0)
        {
            Tile tile = GameController.Instance.GetWallTileAt(transform.position.x, transform.position.y - 1);
            if (tile != null && tile.Type == TileType.Wall)
                _targetTile = tile;
        }
    }

    void LateUpdate()
    {
        Camera.main.transform.position = transform.position + Vector3.forward * -10;
    }
}

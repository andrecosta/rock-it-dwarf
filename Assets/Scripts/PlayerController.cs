using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Tile _currentTile;
    private Tile _targetTile;
    private float _moveTimer;

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
        if (_currentTile != _targetTile)
        {
            _moveTimer += Time.deltaTime*2;

            if (_moveTimer <= 1)
                transform.position = Vector3.Lerp(_currentTile.Position, _targetTile.Position, _moveTimer);
            else
            {
                _currentTile = _targetTile;
                _moveTimer = 0;
            }
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (h > 0)
        {
            Tile tile = GameController.Instance.GetWallTileAt(_currentTile.X + 1, _currentTile.Y);
            if (tile != null && tile.Type == TileType.Wall)
                _targetTile = tile;
        }
        else if (h < 0)
        {
            Tile tile = GameController.Instance.GetWallTileAt(_currentTile.X - 1, _currentTile.Y);
            if (tile != null && tile.Type == TileType.Wall)
                _targetTile = tile;
        }
        else if (v > 0)
        {
            Tile tile = GameController.Instance.GetWallTileAt(_currentTile.X, _currentTile.Y + 1);
            if (tile != null && tile.Type == TileType.Wall)
                _targetTile = tile;
        }
        else if (v < 0)
        {
            Tile tile = GameController.Instance.GetWallTileAt(_currentTile.X, _currentTile.Y - 1);
            if (tile != null && tile.Type == TileType.Wall)
                _targetTile = tile;
        }
    }

    void LateUpdate()
    {
        Camera.main.transform.position = transform.position + Vector3.forward * -10;
    }
}

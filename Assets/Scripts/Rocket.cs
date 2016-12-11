using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        transform.position += transform.up * Time.deltaTime * 4;

        Tile tile = GameController.Instance.GetWallTileAt(transform.position.x, transform.position.y);
        if (tile != null && tile.Type == TileType.Empty)
        {
            tile.Type = TileType.Wall;
            Tile neighbor = GameController.Instance.GetWallTileAt(tile.X + 1, tile.Y);
            if (neighbor != null && neighbor.Type != TileType.Wall)
                neighbor.Type = TileType.Wall;
            neighbor = GameController.Instance.GetWallTileAt(tile.X - 1, tile.Y);
            if (neighbor != null && neighbor.Type != TileType.Wall)
                neighbor.Type = TileType.Wall;
            neighbor = GameController.Instance.GetWallTileAt(tile.X, tile.Y + 1);
            if (neighbor != null && neighbor.Type != TileType.Wall)
                neighbor.Type = TileType.Wall;
            neighbor = GameController.Instance.GetWallTileAt(tile.X, tile.Y - 1);
            if (neighbor != null && neighbor.Type != TileType.Wall)
                neighbor.Type = TileType.Wall;
            neighbor = GameController.Instance.GetWallTileAt(tile.X + 1, tile.Y + 1);
            if (neighbor != null && neighbor.Type != TileType.Wall)
                neighbor.Type = TileType.Wall;
            neighbor = GameController.Instance.GetWallTileAt(tile.X - 1, tile.Y - 1);
            if (neighbor != null && neighbor.Type != TileType.Wall)
                neighbor.Type = TileType.Wall;
            neighbor = GameController.Instance.GetWallTileAt(tile.X + 1, tile.Y - 1);
            if (neighbor != null && neighbor.Type != TileType.Wall)
                neighbor.Type = TileType.Wall;
            neighbor = GameController.Instance.GetWallTileAt(tile.X - 1, tile.Y + 1);
            if (neighbor != null && neighbor.Type != TileType.Wall)
                neighbor.Type = TileType.Wall;

            Destroy(gameObject);
        }
        else if (tile == null)
            Destroy(gameObject);

    }
}

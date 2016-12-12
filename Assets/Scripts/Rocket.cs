using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public AudioClip ExplosionSound;

    private SpriteRenderer _sr;
    private ParticleSystem _ps;

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _ps = GetComponent<ParticleSystem>();
        transform.position = new Vector3(Mathf.Round(transform.position.x*10)/10, Mathf.Round(transform.position.y*10)/10);
    }

    void Update()
    {
        if (!_sr.enabled)
            return;

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

            StartCoroutine(Extinguish());
        }
        else if (tile == null)
            StartCoroutine(Extinguish());
    }

    IEnumerator Extinguish()
    {
        _ps.Stop();
        _sr.enabled = false;
        Camera.main.GetComponent<CameraShake>().ShakeCamera(0.5f, Time.deltaTime);
        AudioSource.PlayClipAtPoint(ExplosionSound, transform.position);

        while (_ps.isPlaying)
            yield return null;

        Destroy(gameObject);
    }
}

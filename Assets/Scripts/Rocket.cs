using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [Header("Explosion")]
    public AudioClip ExplosionSound;
    public GameObject ExplosionEffect;

    [Header("Shell")]
    public Sprite ShellSpriteHorizontal;
    public Sprite ShellSpriteVertical;

    public Vector3 ShootDirection;
    public bool shotByPlayer;

    private SpriteRenderer _sr;
    private List<GameObject> enemiesToClear;
    private ParticleSystem _ps;
    private GameController _gc;
    private CaveCollapser _caveCollapser;

    void Start()
    {
        _gc = GameController.Instance;
        _sr = GetComponent<SpriteRenderer>();
        _ps = GetComponent<ParticleSystem>();
        _caveCollapser = FindObjectOfType<CaveCollapser>();
        transform.position = new Vector3(Mathf.Round(transform.position.x * 10) / 10, Mathf.Round(transform.position.y * 10) / 10);

        if (Mathf.Abs(ShootDirection.x) > Mathf.Abs(ShootDirection.y))
        {
            _sr.sprite = ShellSpriteHorizontal;
            _sr.flipX = ShootDirection.x < 0;
        }
        else
        {
            _sr.sprite = ShellSpriteVertical;
            _sr.flipY = ShootDirection.y > 0;
        }
    }

    void Update()
    {
        if (!_sr.enabled)
            return;

        transform.position += ShootDirection * Time.deltaTime * 6;

        Tile tile = GameController.Instance.GetTileAt(transform.position.x, transform.position.y);

        if (shotByPlayer)
        {
            foreach (GameObject enemy in _gc.enemyList)
            {
                if (Vector3.Distance(enemy.transform.position, transform.position) < 0.2f)
                {
                    areaDamage(tile);
                    StartCoroutine(Extinguish());
                    checkEnemiesinExplosion();
                    _gc.enemyList.Remove(enemy);
                    _gc.KillEnemy(enemy);
                }
            }
            if ((tile != null && tile.Type == TileType.Empty))
            {
                areaDamage(tile);
                StartCoroutine(Extinguish());
                checkEnemiesinExplosion();
            }
            else if (tile == null)
                StartCoroutine(Extinguish());
        }
        else
        {
            if ((tile != null && tile.Type == TileType.Empty) || Vector3.Distance(_gc.player.transform.position, transform.position) < 0.2f)
            {
                if (Vector3.Distance(_gc.player.transform.position, transform.position) < 1.5f)
                {
                    _gc.PlayerDeath();
                }

                areaDamage(tile);

                StartCoroutine(Extinguish());
            }
            else if (tile == null)
                StartCoroutine(Extinguish());
        }
    }

    IEnumerator Extinguish()
    {
        _ps.Stop();
        _sr.enabled = false;
        Destroy(Instantiate(ExplosionEffect, transform.position, Quaternion.identity), 1);
        Camera.main.GetComponent<CameraShake>().ShakeCamera(0.5f, Time.deltaTime);
        AudioSource.PlayClipAtPoint(ExplosionSound, transform.position);
        _caveCollapser.DecreaseCollapseTimer(Random.Range(1, 3));

        while (_ps.isPlaying)
            yield return null;

        Destroy(gameObject);
    }

    private void areaDamage(Tile tile)
    {
        tile.Type = TileType.Terrain;
        Tile neighbor = GameController.Instance.GetTileAt(tile.X + 1, tile.Y);
        if (neighbor != null && neighbor.Type != TileType.Terrain)
            neighbor.Type = TileType.Terrain;
        neighbor = GameController.Instance.GetTileAt(tile.X - 1, tile.Y);
        if (neighbor != null && neighbor.Type != TileType.Terrain)
            neighbor.Type = TileType.Terrain;
        neighbor = GameController.Instance.GetTileAt(tile.X, tile.Y + 1);
        if (neighbor != null && neighbor.Type != TileType.Terrain)
            neighbor.Type = TileType.Terrain;
        neighbor = GameController.Instance.GetTileAt(tile.X, tile.Y - 1);
        if (neighbor != null && neighbor.Type != TileType.Terrain)
            neighbor.Type = TileType.Terrain;
        neighbor = GameController.Instance.GetTileAt(tile.X + 1, tile.Y + 1);
        if (neighbor != null && neighbor.Type != TileType.Terrain)
            neighbor.Type = TileType.Terrain;
        neighbor = GameController.Instance.GetTileAt(tile.X - 1, tile.Y - 1);
        if (neighbor != null && neighbor.Type != TileType.Terrain)
            neighbor.Type = TileType.Terrain;
        neighbor = GameController.Instance.GetTileAt(tile.X + 1, tile.Y - 1);
        if (neighbor != null && neighbor.Type != TileType.Terrain)
            neighbor.Type = TileType.Terrain;
        neighbor = GameController.Instance.GetTileAt(tile.X - 1, tile.Y + 1);
        if (neighbor != null && neighbor.Type != TileType.Terrain)
            neighbor.Type = TileType.Terrain;

        if (Vector3.Distance(_gc.player.transform.position, transform.position) < 1f)
        {
            _gc.PlayerDeath();
        }
    }

    private void checkEnemiesinExplosion()
    {
        foreach (GameObject enemy in _gc.enemyList)
        {
            if (Vector3.Distance(enemy.transform.position, transform.position) < 1.5f)
            {
                _gc.enemyList.Remove(enemy);
                _gc.KillEnemy(enemy);
            }
        }
    }
}

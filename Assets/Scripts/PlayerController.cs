using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject Rocket;

    private Tile _currentTile;
    private Tile _targetTile;
    private float _moveTimer;
    private Vector2 _orientation;
    private Sprite[] _animations;
    private float _animationTimer;
    private int _animationFrame;
    private SpriteRenderer _sr;

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

        // Load animation
        _animations = Resources.LoadAll<Sprite>("Sprites/LittleDwarf");
        _sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Movement();

        if (Input.GetKey(KeyCode.Space))
            Dig();

        if (Input.GetKeyDown(KeyCode.LeftControl))
            Shoot();

        Animation();
    }

    void LateUpdate()
    {
        Camera.main.transform.position = transform.position + Vector3.forward * -10;
    }

    void Movement()
    {

        if (Vector2.Distance(transform.position, _targetTile.Position) >= 0.01f)
            transform.position = Vector3.MoveTowards(transform.position, _targetTile.Position, Time.deltaTime*2);

        if (Vector2.Distance(transform.position, _targetTile.Position) < 0.05f)
            _currentTile = _targetTile;

        if (_currentTile != _targetTile)
            return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if (h == 0 && v == 0)
            return;

        _orientation = new Vector2(h, v);
        if (Mathf.Abs(h) > 0)
            _orientation.y = 0;

        Tile tile = GameController.Instance.GetWallTileAt(transform.position.x + _orientation.x, transform.position.y + _orientation.y);
        if (tile != null && tile.Type == TileType.Wall)
            _targetTile = tile;
    }

    void Dig()
    {
        if (_currentTile != _targetTile)
            return;

        Tile tile = GameController.Instance.GetWallTileAt(_currentTile.X + _orientation.x, _currentTile.Y + _orientation.y);
        if (tile != null && tile.Type == TileType.Empty)
            GameController.Instance.GetWallTileAt(tile.X, tile.Y).Type = TileType.Wall;
    }

    void Shoot()
    {
        if (_currentTile != _targetTile)
            return;

        Instantiate(Rocket, transform.position, Quaternion.LookRotation(transform.forward, _orientation));
    }

    void Animation()
    {
        if (_currentTile == _targetTile)
            _animationFrame = 0;
        else if (_animationTimer <= 0)
        {
            _animationTimer = 0.05f;
            _animationFrame++;
            if (_animationFrame >= _animations.Length)
                _animationFrame = 0;
        }
        else
            _animationTimer -= Time.deltaTime;

        _sr.sprite = _animations[_animationFrame];
    }
}

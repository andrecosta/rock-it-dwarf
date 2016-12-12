using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject Rocket;
    public Vector2 getOrientation()
    {
        return _orientation;
    }

    private Tile _currentTile;
    private Tile _targetTile;
    private float _moveTimer;
    private Vector2 _orientation;
    private Sprite[] _walkingAnimation;
    private Sprite[] _miningAnimation;
    private float _animationTimer;
    private int _animationFrame;
    private SpriteRenderer _sr;
    private float _digTimer;
    private float _shootCooldown;
    private float _lastHorizontalOrientation;

    void Start()
    {
        foreach (var tile in GameController.Instance.TerrainTiles)
        {
            if (tile.Type == TileType.Terrain)
            {
                _currentTile = tile;
                _targetTile = _currentTile;
            }
        }
        transform.position = _currentTile.Position;
        Camera.main.transform.position = transform.position + Vector3.forward * -10;

        // Load animation
        _walkingAnimation = Resources.LoadAll<Sprite>("Sprites/DwarfWalking");
        _miningAnimation = Resources.LoadAll<Sprite>("Sprites/DwarfMining");
        _sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Movement();

        if (Input.GetKey(KeyCode.Space))
        {
            _digTimer -= Time.deltaTime;
            if (_digTimer <= 0)
            {
                Dig();
                _digTimer = 1;
            }

            MiningAnimation();
        }
        else
        {
            _digTimer = 1;

            if (_shootCooldown > 0)
                _shootCooldown -= Time.deltaTime;
            else if (Input.GetKey(KeyCode.LeftArrow))
                Shoot(Vector2.left);
            else if (Input.GetKey(KeyCode.RightArrow))
                Shoot(Vector2.right);
            else if (Input.GetKey(KeyCode.UpArrow))
                Shoot(Vector2.up);
            else if (Input.GetKey(KeyCode.DownArrow))
                Shoot(Vector2.down);

            Animation();
        }
    }

    void LateUpdate()
    {
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, transform.position + Vector3.forward * -10, Time.deltaTime*9);
    }

    void Movement()
    {
        // Move the player towards the target tile
        if (Vector2.Distance(transform.position, _targetTile.Position) >= 0.01f)
            transform.position = Vector3.MoveTowards(transform.position, _targetTile.Position, Time.deltaTime*2);

        // Upon reaching the target tile, set it as the current tile
        if (Vector2.Distance(transform.position, _targetTile.Position) < 0.05f)
            _currentTile = _targetTile;

        // Stop here if the player has not yet reached the target tile
        if (_currentTile != _targetTile)
            return;

        // Read the player input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Stop here if the player is stopped
        if (h == 0 && v == 0)
            return;

        // Store the player's orientation for future use (limiting to only one axis at a time)
        _orientation = new Vector2(h, v);
        if (h != 0)
            _lastHorizontalOrientation = _orientation.x;
        if (Mathf.Abs(h) > 0)
            _orientation.y = 0;

        // Get the new target tile based on the player's intention of movement
        Tile tile = GameController.Instance.GetTileAt(transform.position.x + _orientation.x, transform.position.y + _orientation.y);
        if (tile != null && tile.Type == TileType.Terrain)
            _targetTile = tile;
    }

    void Dig()
    {
        if (_currentTile != _targetTile)
            return;

        Tile tile = GameController.Instance.GetTileAt(_currentTile.X + _orientation.x, _currentTile.Y + _orientation.y);
        if (tile != null && tile.Type == TileType.Empty)
            GameController.Instance.GetTileAt(tile.X, tile.Y).Type = TileType.Terrain;
    }

    void Shoot(Vector2 direction)
    {
        Instantiate(Rocket, transform.position, Quaternion.LookRotation(transform.forward, direction));
        _shootCooldown = 1;
    }

    void Animation()
    {
        if (_currentTile == _targetTile)
            _animationFrame = 0;
        else if (_animationTimer <= 0)
        {
            _animationTimer = 0.05f;
            _animationFrame++;
        }
        else
            _animationTimer -= Time.deltaTime;

        if (_animationFrame >= _walkingAnimation.Length)
            _animationFrame = 0;

        _sr.flipX = _lastHorizontalOrientation < 0;
        _sr.sprite = _walkingAnimation[_animationFrame];
    }

    void MiningAnimation()
    {
        if (_animationTimer <= 0)
        {
            _animationTimer = 0.05f;
            _animationFrame++;
        }
        else
            _animationTimer -= Time.deltaTime;

        if (_animationFrame >= _miningAnimation.Length)
            _animationFrame = 0;

        _sr.flipX = _lastHorizontalOrientation < 0;
        _sr.sprite = _miningAnimation[_animationFrame];
    }
}

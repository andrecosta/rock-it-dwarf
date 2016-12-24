using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rocket RocketPrefab;
    public Vector2 getOrientation()
    {
        return _orientation;
    }

    private Tile _currentTile;
    private Tile _targetTile;
    private float _moveTimer;
    private Vector2 _orientation;
    private float _digTimer;
    private float _shootCooldown;
    private float _lastHorizontalOrientation;
    private Animator _animator;

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
        //Camera.main.transform.position = transform.position + Vector3.forward * -10;

        // Get animator component
        _animator = GetComponent<Animator>();

        StartCoroutine(CheckLava());
    }

    void Update()
    {
        if (!_animator.GetBool("Is Shooting"))
            Movement();

        Tile tile = GameController.Instance.GetTileAt(_currentTile.X + _orientation.x, _currentTile.Y + _orientation.y);
        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && tile != null && tile.Type == TileType.Empty)
        {
            _digTimer -= Time.deltaTime;
            _animator.SetBool("Is Moving", false);
            _animator.SetBool("Is Mining", true);
            if (_digTimer <= 0)
            {
                Dig();
                _digTimer = 1;
            }
        }
        else
        {
            _digTimer = 1;
            _animator.SetBool("Is Mining", false);
        }

        if (_shootCooldown > 0)
        {
            _shootCooldown -= Time.deltaTime;
            if (_shootCooldown < 0.7f)
                _animator.SetBool("Is Shooting", false);
        }
        else
        {
            float shootX = Input.GetAxis("ShootHorizontal");
            float shootY = Input.GetAxis("ShootVertical");

            if (shootX < 0)
                Shoot(Vector2.left);
            else if (shootX > 0)
                Shoot(Vector2.right);
            else if (shootY < 0)
                Shoot(Vector2.down);
            else if (shootY > 0)
                Shoot(Vector2.up);
        }
    }

    void LateUpdate()
    {
        Vector3 targetPosition = transform.position;

        // TODO: Make these values change according to level size and camera bounds
        float maxX = 29f;
        float minX = 10;
        float maxY = 34.5f;
        float minY = 4.5f;

        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPosition + Vector3.forward * -10, Time.deltaTime*9);
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
        {
            _animator.SetBool("Is Moving", false);
            _animator.SetFloat("Speed", 0);
            return;
        }

        // Store the player's orientation for future use (limiting to only one axis at a time)
        _orientation = new Vector2(h, v);
        if (h != 0)
            _lastHorizontalOrientation = _orientation.x;
        if (Mathf.Abs(h) > 0)
            _orientation.y = 0;

        _animator.SetBool("Is Moving", true);
        _animator.SetFloat("Horizontal Velocity", Mathf.Abs(_orientation.x));
        _animator.SetFloat("Vertical Velocity", _orientation.y);
        _animator.SetFloat("Speed", _orientation.magnitude);

        if (_lastHorizontalOrientation < 0)
            transform.localEulerAngles = new Vector3(0, 180, 0);
        else
            transform.localEulerAngles = Vector3.zero;

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
        _animator.SetFloat("Vertical Velocity", direction.y);
        if (direction.x < 0)
            transform.localEulerAngles = new Vector3(0, 180, 0);
        else
            transform.localEulerAngles = Vector3.zero;

        Rocket rocket = Instantiate(RocketPrefab, transform.position, Quaternion.identity);
        rocket.ShootDirection = direction;
        rocket.shotByPlayer = true;
        _shootCooldown = 1;
        _animator.SetBool("Is Moving", false);
        _animator.SetBool("Is Shooting", true);
    }

    IEnumerator CheckLava()
    {
        while (true)
        {
            if (GameController.Instance.GetLavaTileAt(_currentTile.X, _currentTile.Y).Type == TileType.Lava)
               GameController.Instance.PlayerDeath();
                
            yield return new WaitForSeconds(0.1f);
        }
    }
}

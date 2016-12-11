using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {

    public float randomness = 0.2f;

    private Tile _currentTile;
    private GameController _gameController;
    private Tile _targetTile;
    private float _moveTimer;
    private Vector2 _orientation;
    private Sprite[] _animations;
    private float _animationTimer;
    private int _animationFrame;
    private SpriteRenderer _sr;
    private float _lastHorizontalOrientation;
    private bool _deteced_player;
    private float timerForAction = 0;

    public void setTile(Tile currTile) { _currentTile = currTile; }

    void Start()
    {
        // Load animation
        _animations = Resources.LoadAll<Sprite>("Sprites/LittleDwarf");
        _gameController = GameController.Instance;
        _sr = GetComponent<SpriteRenderer>();
        _orientation = getNewDirection();
        _targetTile = _currentTile;
    }
    // Update is called once per frame
    void Update () {
        Animation();

        _deteced_player = checkIfPlayer();
        if (!_deteced_player)
            wander();
        else
            chase();
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

        _sr.flipX = _lastHorizontalOrientation < 0;
        _sr.sprite = _animations[_animationFrame];
    }

    void wander()
    {
        float speed;
        if (_currentTile.Type == TileType.Empty)
            speed = Time.deltaTime;
        else
            speed = Time.deltaTime * 2;

        if (Vector2.Distance(transform.position, _targetTile.Position) >= 0.01f)
            transform.position = Vector3.MoveTowards(transform.position, _targetTile.Position, speed);

        // Upon reaching the target tile, set it as the current tile
        if (Vector2.Distance(transform.position, _targetTile.Position) < 0.05f)
            _currentTile = _targetTile;

        if (_currentTile != _targetTile)
            return;

        //implementing a delay between inputs
        timerForAction -= Time.deltaTime;

        if (timerForAction > 0)
            return;

        Vector2 direction;
        if (Random.Range(1, 100) < randomness * 100)
        {
            //Getting random direction
            direction = getNewDirection();
            getNewDelay();
            _orientation = direction;
        }
        else
            direction = _orientation;

        // Get the new target tile based on the player's intention of movement
        Tile tile = GameController.Instance.GetWallTileAt(transform.position.x + direction.x, transform.position.y + direction.y);
        if (tile != null)
            _targetTile = tile;

    }
    void chase()
    {

    }

    private bool checkIfPlayer()
    {
        //if (Vector3.SqrMagnitude(transform.position - _gameController))
        return false;
    }

    private Vector2 getNewDirection()
    {
        Vector2 directionVector;

        int randomDirection = Random.Range(1, 4);
        switch (randomDirection)
        {
            case 1:
                directionVector = Vector2.up;
                break;
            case 2:
                directionVector = Vector2.right;
                break;
            case 3:
                directionVector = Vector2.down;
                break;
            default:
                directionVector = Vector2.left;
                break;
        }

        return directionVector;
    }

    private void getNewDelay()
    {
        timerForAction = Random.Range(0f, 1f);
    }

    /*  public GameObject Rocket;
    public Vector2 getOrientation()
    {
        return _orientation;
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
        }
        else
            _digTimer = 1;

        if (_shootCooldown > 0)
            _shootCooldown -= Time.deltaTime;
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            Shoot();
            _shootCooldown = 1;
        }

        Animation();
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

        _sr.flipX = _lastHorizontalOrientation < 0;
        _sr.sprite = _animations[_animationFrame];
    }*/
}

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
    private int _groundType = 0;
    private SpriteRenderer _sr;
    private float _lastHorizontalOrientation;
    private bool _deteced_player;
    private float timerForAction = 0;

    public void setTile(Tile currTile) { _currentTile = currTile; }

    void Start()
    {
        // Load animation
        _animations = Resources.LoadAll<Sprite>("Sprites/DwarfWalking");
        _gameController = GameController.Instance;
        _sr = GetComponent<SpriteRenderer>();
        _orientation = getNewDirection();
        _targetTile = _currentTile;
    }
    // Update is called once per frame
    void Update () {
        Animation();

        if (!checkIfPlayer())
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

    bool finishedMovement()
    {
        float speed;

        if (_groundType == 0)
        {
            switch (_targetTile.Type)
            {
                case TileType.Terrain:
                    _groundType = 2;
                    break;
                case TileType.Empty:
                    _groundType = 1;
                    break;
                default:
                    _groundType = 2;
                    break;
            }
        }

        if (_groundType == 1)
            speed = Time.deltaTime * 1 / 2;

        else
            speed = (Time.deltaTime * 3 / 2);

        if (Vector2.Distance(transform.position, _targetTile.Position) >= 0.01f)
            transform.position = Vector3.MoveTowards(transform.position, _targetTile.Position, speed);

        if (Vector2.Distance(transform.position, _targetTile.Position) < 0.05f)
            _currentTile = _targetTile;

        if (Vector2.Distance(transform.position, _targetTile.Position) < 0.4f)
            Dig();

        // Upon reaching the target tile, set it as the current tile
        if (Vector2.Distance(transform.position, _targetTile.Position) < 0.05f)
            _currentTile = _targetTile;

        if (_currentTile != _targetTile)
            return false;
        else
        {
            _groundType = 0;
            return true;
        }
           
    }

    void wander()
    {

        if (!finishedMovement())
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
        Tile tile = GameController.Instance.GetTileAt(transform.position.x + direction.x, transform.position.y + direction.y);
        if (tile != null)
            _targetTile = tile;

    }
    void chase()
    {
        if (!finishedMovement())
            return;
        GameObject player = _gameController.player;
        Vector2 direction;

        float xToPlayer = (player.transform.position.x - transform.position.x);
        float yToPlayer = (player.transform.position.y - transform.position.y);

        if (Vector2.SqrMagnitude(player.transform.position - transform.position) < 0.5f)
            return;

        //getting the most critical direction
        if (Mathf.Abs(yToPlayer) > Mathf.Abs(xToPlayer))
            direction = new Vector2(0, Mathf.Sign(yToPlayer));
        else 
            direction = new Vector2(Mathf.Sign(xToPlayer), 0);
        _orientation = direction;

        Tile tile = GameController.Instance.GetTileAt(transform.position.x + direction.x, transform.position.y + direction.y);
        if (tile != null)
            _targetTile = tile;
    }

    private bool checkIfPlayer()
    {
        if (Vector2.SqrMagnitude(transform.position - _gameController.player.transform.position) < 20)
            return true;
        else return false;
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

    void Dig()
    {
        if (_currentTile == _targetTile)
            return;

        Tile tile = GameController.Instance.GetTileAt(_currentTile.X + _orientation.x, _currentTile.Y + _orientation.y);
        if (tile != null && tile.Type == TileType.Empty)
            GameController.Instance.GetTileAt(tile.X, tile.Y).Type = TileType.Terrain;
    }

    /*
    void Shoot()
    {
        if (_currentTile != _targetTile)
            return;

        Instantiate(RocketPrefab, transform.position, Quaternion.LookRotation(transform.forward, _orientation));
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

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {

    public float randomness;
    public int type;
    public bool hardMode = false;
    public Rocket rocketPrefab;
    public Arrow arrowPrefab;
    [Header("Audio")]
    public AudioClip DrillingSound;
    public AudioClip DigSound;
    public AudioClip ShootSound;

    public bool menu;
    private Tile _currentTile;
    private float _shootCooldown = 1.5f;
    private GameController _gameController;
    private Tile _targetTile;
    private float _moveTimer;
    private Vector2 _orientation;
    private int _groundType = 0;
    private float _lastHorizontalOrientation;
    private bool _deteced_player;
    private float timerForAction = 0;
    private GameObject _player;
    private Animator _animator;
    private AudioSource _audioSource;

    public void setTile(Tile currTile) { _currentTile = currTile; }

    void Start()
    {
        _gameController = GameController.Instance;
        _player = _gameController.player;
        _orientation = getNewDirection();
        _targetTile = _currentTile;

        // Get components
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    void Update ()
    {
        if (GameController.Instance.IsPaused)
            return;

        if (!checkIfPlayer())
            wander();
        else
            alerted();
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
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetTile.Position, speed);

            _lastHorizontalOrientation = _orientation.x;

            if (_targetTile.Type == TileType.Terrain)
            {
                _animator.SetBool("Is Shooting", false);
                _animator.SetBool("Is Mining", false);
                _animator.SetBool("Is Moving", true);
                if (_audioSource.volume > 0f)
                    _audioSource.volume -= Time.deltaTime * 0.1f;
            }
            else
            {
                _animator.SetBool("Is Moving", false);
                _animator.SetBool("Is Mining", true);
                if (_audioSource.volume < 0.01f && Vector2.Distance(transform.position, _player.transform.position) < 5)
                    _audioSource.volume += Time.deltaTime * 0.1f;
            }
            _animator.SetFloat("Horizontal Velocity", Mathf.Abs(_orientation.x));
            _animator.SetFloat("Vertical Velocity", _orientation.y);
            _animator.SetFloat("Speed", _orientation.magnitude);

            if (_lastHorizontalOrientation < 0)
                transform.localEulerAngles = new Vector3(0, 180, 0);
            else
                transform.localEulerAngles = Vector3.zero;
        }

        if (Vector2.Distance(transform.position, _targetTile.Position) < 0.05f)
            _currentTile = _targetTile;

        if (Vector2.Distance(transform.position, _targetTile.Position) < 0.4f)
        {
            //_animator.SetBool("Is Moving", false);
            //_animator.SetBool("Is Mining", true);
            Dig();
        }
        else
        {
            //_animator.SetBool("Is Mining", false);
        }

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
        {
            _animator.SetBool("Is Moving", false);
            _animator.SetFloat("Speed", 0);
            return;
        }

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

        if (!menu && tile != null)
        {
            _targetTile = tile;
            return;
        }
        if (tile == null || (tile.X < 40 && tile.X > 20) && (tile.Y < 30 && tile.Y > 22))
            return;

        _targetTile = tile;
    }

    void alerted()
    {
        if (!finishedMovement())
            return;

        GameObject player = _gameController.player;

        float xToPlayer = (player.transform.position.x - transform.position.x);
        float yToPlayer = (player.transform.position.y - transform.position.y);

        if (Vector2.SqrMagnitude(player.transform.position - transform.position) < 0.5f)
        {
            _gameController.PlayerDeath();
            return;
        }

        if (type == 2)
            chase(xToPlayer, yToPlayer);
        else if (type == 1)
            shoot_bazooka(xToPlayer, yToPlayer);
        else if (type == 3)
            arrowMonster_behaviour(xToPlayer, yToPlayer);
        return;

    }

    // Method for the chaser enemies
    private void chase(float xToPlayer, float yToPlayer)
    {
        Vector2 direction;

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

    private void shoot_bazooka(float xToPlayer, float yToPlayer)
    {
        Vector2 direction = Vector2.zero;
        Tile tile;


        if (Mathf.Abs(yToPlayer) <= 0.5f)
        {
            if (!checkWalls())
            {
                direction = new Vector2(Mathf.Sign(xToPlayer), 0);
                _orientation = direction;

                tile = GameController.Instance.GetTileAt(transform.position.x + direction.x, transform.position.y + direction.y);
                if (tile != null)
                    _targetTile = tile;
                return;
            }
        }
        else if (Mathf.Abs(xToPlayer) <= 0.5f)
        {
            if (!checkWalls())
            {
                direction = new Vector2(0, Mathf.Sign(yToPlayer));
                _orientation = direction;

                tile = GameController.Instance.GetTileAt(transform.position.x + direction.x, transform.position.y + direction.y);
                if (tile != null)
                    _targetTile = tile;
                return;
            }
        }
        else
        {
            if (Mathf.Abs(yToPlayer) > Mathf.Abs(xToPlayer))
                direction = new Vector2(Mathf.Sign(xToPlayer), 0);
            else
                direction = new Vector2(0, Mathf.Sign(yToPlayer));
            _orientation = direction;

            tile = GameController.Instance.GetTileAt(transform.position.x + direction.x, transform.position.y + direction.y);
            if (tile != null)
                _targetTile = tile;
            return;
        }
        
        if (_shootCooldown > 0)
        {
            _animator.SetBool("Is Moving", false);
            _animator.SetFloat("Speed", 0);
            _shootCooldown -= Time.deltaTime;
            if (_shootCooldown < 1.2f)
            {
                _animator.SetBool("Is Shooting", false);
                _animator.SetBool("Is Moving", true);
            }
            return;
        }

        if (Mathf.Abs(yToPlayer) > Mathf.Abs(xToPlayer))
            Shoot(new Vector2(0, Mathf.Sign(yToPlayer)));
        else
            Shoot(new Vector2(Mathf.Sign(xToPlayer), 0));

    }

    private void arrowMonster_behaviour(float xToPlayer, float yToPlayer)
    {
        Tile targetTile;

        if (!checkWalls())
        {
            chase( xToPlayer, yToPlayer);
            return;
        }
        else
        {
            targetTile = GameController.Instance.GetTileAt(_player.transform.position.x, _player.transform.position.y);
        }

        if (_shootCooldown > 0)
        {
            _shootCooldown -= Time.deltaTime;
            _animator.SetBool("Is Moving", false);
            _animator.SetFloat("Speed", 0);
            return;
        }
        shoot_arrow(targetTile);

    }


    private bool checkIfPlayer()
    {
        if (Vector2.SqrMagnitude(transform.position - _gameController.player.transform.position) < 40)
            return true;
        else return false;
    }

    private Vector2 getNewDirection()
    {
        Vector2 directionVector;

        int randomDirection = Random.Range(1, 5);
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
        {
            GameController.Instance.GetTileAt(tile.X, tile.Y).Type = TileType.Terrain;
            if (Vector2.Distance(transform.position, _player.transform.position) < 5)
                AudioSource.PlayClipAtPoint(DigSound, tile.Position, 0.2f);
        }
    }

    void Shoot(Vector2 projectileDirection)
    {
        if (_currentTile != _targetTile)
            return;

        _animator.SetFloat("Vertical Velocity", projectileDirection.y);
        if (projectileDirection.x < 0)
            transform.localEulerAngles = new Vector3(0, 180, 0);
        else
            transform.localEulerAngles = Vector3.zero;

        Rocket rocket = Instantiate(rocketPrefab, transform.position, Quaternion.identity);
        rocket.ShootDirection = projectileDirection;
        rocket.shotByPlayer = false;
        _shootCooldown = 1.5f;
        _animator.SetBool("Is Moving", false);
        _animator.SetBool("Is Shooting", true);
        AudioSource.PlayClipAtPoint(ShootSound, transform.position);
    }

    void shoot_arrow(Tile target)
    {
        if (_currentTile != _targetTile)
            return;

        Arrow arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        arrow.targetTile = target;
        _shootCooldown = 1.5f;
    }

    private bool checkWalls()
    {
        int playerX = (int)(_player.transform.position.x + 0.5f);
        int playerY = (int)(_player.transform.position.y + 0.5f);
        int enemyX = (int)(transform.position.x + 0.5f);
        int enemyY = (int)(transform.position.y + 0.5f);

        Tile testTile;

        bool visible = true;

        // Line drawing algorithm
        int dx = (enemyX - playerX);
        int dy = (enemyY - playerY);
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (dx < 0) dx1 = -1; else if (dx > 0) dx1 = 1;
        if (dy < 0) dy1 = -1; else if (dy > 0) dy1 = 1;
        if (dx < 0) dx2 = -1; else if (dx > 0) dx2 = 1;
        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if (!(longest > shortest))
        {
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);
            if (dy < 0) dy2 = -1; else if (dy > 0) dy2 = 1;
            dx2 = 0;
        }

        int numerator = longest >> 1;

        //int D = 2 * dy - dx;
        int y = playerY;
        int x = playerX;

        for (int i = 0; i <= longest; i++)
        {

            testTile = GameController.Instance.GetTileAt(x, y);
            numerator += shortest;
            if (!(numerator < longest))
            {
                numerator -= longest;
                x += dx1;
                y += dy1;
            }
            else
            {
                x += dx2;
                y += dy2;
            }
            if (testTile.Type == TileType.Empty)
                visible = false;
        }
        return visible;
    }
}

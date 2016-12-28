using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    // Singleton
    public static GameController Instance { get; private set; }

    //Map Generation Variables
    public bool menu;
    public int mapSize, mapTunnelAmmount, mapRoomAmmount;
    public float mapRandomness, mapEmptyArea;
    public GameObject player;
    public GameObject goalObject;
    public Tile[,] TerrainTiles;
    public Tile[,] LavaTiles;
    public List<Tile> EmptyTiles;
    public List<Tile> goalTiles;
    public List<GameObject> enemyList;
    public Tile goalTile;

    public bool IsGameOver { get; private set; }
    public bool IsGameVictory { get; private set; }
    public bool IsPaused { get; private set; }
    public Action<Tile> CallbackTileChanged { get; set; }
    private MapGenerator _map;
    public GameObject[] enemies;
    public int enemyAmmount;
    public int existingEnemies;

    // Events
    public Action OnPause;
    public Action OnUnpause;

    private float enemyTimer;

    void Awake()
    {
        // Singleton
        if (Instance != null)
        {
            Debug.LogError("Only one GameController allowed!");
            Destroy(gameObject);
        }
        Instance = this;

        Time.timeScale = 1;
        _map = new MapGenerator(mapSize, mapTunnelAmmount, mapRoomAmmount, mapEmptyArea, mapRandomness, menu);
        TerrainTiles = _map.getTerrainTiles();
        LavaTiles = _map.getLavaTiles();
        EmptyTiles = _map.getEmptyTiles();
        goalTiles = _map.goalTiles;
        goalTile = _map.goalTile;
        instantiateGoal();

        existingEnemies = 0;
        enemyTimer = 15;

        InstantiateEnemies();
    }

    void Update()
    {
        if (menu)
        {
            if (Input.anyKeyDown)
                SceneManager.LoadScene("Game");
        }

        if (existingEnemies != enemyAmmount)
        {
            enemyTimer -= Time.deltaTime;
            if (enemyTimer < 0)
            {
                InstantiateEnemies();
                enemyTimer = 15;
            }
        }

        // Pausing
        if (Input.GetButtonUp("Pause") && !IsPaused)
        {
            IsPaused = true;
            if (OnPause != null)
                OnPause();
        }
        else if (Input.GetButtonUp("Pause") && IsPaused)
        {
            IsPaused = false;
            if (OnUnpause != null)
                OnUnpause();
        }
    }

    public Tile GetTileAt(int x, int y)
    {
        if (x >= mapSize || x < 0 || y >= mapSize || y < 0)
            return null;

        return TerrainTiles[x, y];
    }

    public Tile GetLavaTileAt(int x, int y)
    {
        if (x >= mapSize || x < 0 || y >= mapSize || y < 0)
            return null;

        return LavaTiles[x, y];
    }
    
    public Tile GetTileAt(float x, float y)
    {
        return GetTileAt((int)Mathf.Round(x), (int)Math.Round(y));
    }

    public Tile GetLavaTileAt(float x, float y)
    {
        return GetLavaTileAt((int)Mathf.Round(x), (int)Math.Round(y));
    }

    public void OnTileChanged(Tile tile)
    {
        if (CallbackTileChanged != null)
            CallbackTileChanged(tile);
    }
   

    private void InstantiateEnemies()
    {
        while (enemyAmmount - existingEnemies > 0)
        {
            int random = Random.Range(0, (EmptyTiles.Count - 1));
            Tile startingTile = EmptyTiles[random];

            GameObject enemy = enemies[Random.Range(0, enemies.Length)];

            GameObject currEnemy = Instantiate(enemy, new Vector3(startingTile.X, startingTile.Y, 0), Quaternion.identity);
            AIController enemyAI = currEnemy.GetComponent<AIController>();
            enemyAI.menu = menu;
            enemyAI.setTile(startingTile);
            enemyList.Add(currEnemy);
            existingEnemies++;
        }
    }

    public void KillEnemy(GameObject enemy)
    {
        //enemyList.Remove(enemy);
        existingEnemies--;
        Destroy(enemy);
    }

    private void instantiateGoal()
    {
        if (goalObject.transform.position.x != 0)
            Instantiate(goalObject, new Vector3(goalTile.X, goalTile.Y, 0), Quaternion.identity);
    }

    public void PlayerDeath()
    {
        Debug.Log("Dead Player");
        StartCoroutine(SlowDown());
        IsGameOver = true;
    }
    public void PlayerVictory()
    {
        Debug.Log("Victory");
        StartCoroutine(SlowDown());
        IsGameVictory = true;
    }

    IEnumerator SlowDown()
    {
        while (Time.timeScale > 0)
        {
            Camera.main.GetComponent<CameraShake>().ShakeCamera(Time.deltaTime * 0.1f, Time.deltaTime * 0.1f);
            Time.timeScale -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}

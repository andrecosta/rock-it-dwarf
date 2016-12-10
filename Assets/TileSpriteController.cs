using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpriteController : MonoBehaviour {

    public Dictionary<Tile, GameObject> GeneratedTiles { get; private set; }

    public Sprite _floorSprite;

    void Awake()
    {
        // Load tileset
        //_floorSprite = Resources.Load<Sprite>("Square");
    }

    void Start()
    {
        // Instantiate the floor tile GameObjects
        GeneratedTiles = new Dictionary<Tile, GameObject>();
        foreach (Tile tile in GameController.Instance.Tiles)
        {
            GameObject go = new GameObject("TILE [" + tile.X + ", " + tile.Y + "]");
            go.transform.SetParent(transform);
            go.transform.localPosition = tile.Position;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = "Tiles";
            sr.sprite = _floorSprite;
            
            // Register callbacks
            //tile.CallbackTileTypeChanged += OnTileTypeChanged;

            GeneratedTiles.Add(tile, go);
        }

        // Map callback
        GameController.Instance.CallbackTileChanged += OnTileChanged;
    }

    void OnTileChanged(Tile tile)
    {
        if (GeneratedTiles.ContainsKey(tile) == false)
        {
            // Nope
            return;
        }

        var sr = GeneratedTiles[tile].GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            // Nope
            return;
        }

        switch (tile.Type)
        {
            case Tile.TileType.Floor:
                sr.sprite = _floorSprite;
                break;
            default:
                sr.sprite = null;
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

    GameController _gc;

	// Use this for initialization
	void Start () {
        _gc = GameController.Instance;
	}
	
	// Update is called once per frame
	void Update () {
        if (Vector3.Distance(_gc.player.transform.position, transform.position) < 0.5f)
        {
            _gc.PlayerVictory();
        }
	}
}

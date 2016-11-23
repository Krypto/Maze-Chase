using UnityEngine;
using System.Collections;
using System;

public class RandomAI : MonoBehaviour, AI {
    private Enemy enemy;
 
    public float GetHorizontalInput() {
        Vector2 d = enemy.CurrentDirection();
        if (d == Vector2.zero || UnityEngine.Random.value > 0.975f) {
            return UnityEngine.Random.Range(-1f, 1f);
        }else {
            return d.x;
        }
    }

    public float GetVerticalInput() {
        Vector2 d = enemy.CurrentDirection();
        if (d == Vector2.zero || UnityEngine.Random.value > 0.975f) {
            return UnityEngine.Random.Range(-1f, 1f);
        } else {
            return d.y;
        }
    }

    public void SetEnemy(Enemy enemy) {
        this.enemy = enemy;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

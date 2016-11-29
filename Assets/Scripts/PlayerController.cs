using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {
    Player player;
    Vector2[] directions = { new Vector2(1, 0), new Vector2(0, 1),
        new Vector2(-1, 0), new Vector2(0, -1) };

    List<Vector2> canGo = new List<Vector2>();

    public Vector2 GetDirection() {
        canGo.Clear();
        for (int i = 0; i < directions.Length; i++) {
            if (player.CanGo(directions[i])) {
                canGo.Add(directions[i]);
            }
        }
        if (canGo.Count == 0) {
            Debug.LogError("player is stuck!");
            Destroy(player.gameObject);
            FindObjectOfType<Game>().SpawnPlayer();
            return Vector2.zero;
        }
        Vector2 d = player.CurrentDirection();
        if (d == Vector2.zero || !player.CanGo(d)) {
            return GetInputDirection(canGo);
        }
        Vector2 d1 = GetInputDirection(canGo);
        if (d1 != Vector2.zero) {
            return d1;
        }
        if (canGo.Contains(d)) {
            return d;
        }
        return Vector2.zero;
    }

    Vector2 GetInputDirection(List<Vector2> canGo) {
        float dx = CrossPlatformInputManager.GetAxis("Horizontal");
        float dy = CrossPlatformInputManager.GetAxis("Vertical");
        float idx = (dx > 0) ? 1 : (dx < 0) ? -1 : 0;
        float idy = (dy > 0) ? 1 : (dy < 0) ? -1 : 0;
        if (Mathf.Abs(dx) > Mathf.Abs(dy)) {
            dy = 0;
        }else {
            dx = 0;
        }
        Vector2 d = new Vector2(idx, idy);
        if (canGo.Contains(d)) {
            return d;
        }
        if (dx != 0 && Mathf.Abs(dx) > Mathf.Abs(dy)) {
            d = new Vector2(idx, 0);
            if (canGo.Contains(d)) {
                return d;
            }
            d = new Vector2(0, idy);
            if (canGo.Contains(d)) {
                return d;
            }
            return Vector2.zero;
        }
        d = new Vector2(0, idy);
        if (canGo.Contains(d)) {
            return d;
        }
        d = new Vector2(idx, 0);
        if (canGo.Contains(d)) {
            return d;
        }
        return Vector2.zero;
    }

    public void SetPlayer(Player player) {
        this.player = player;
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}

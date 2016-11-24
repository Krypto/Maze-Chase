using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomAI : MonoBehaviour, AI {
    private Enemy enemy;
    Vector2[] directions = { new Vector2(1, 0), new Vector2(0, 1),
        new Vector2(-1, 0), new Vector2(0, -1) };

    List<Vector2> canGo = new List<Vector2>();

    public Vector2 GetDirection() {
        canGo.Clear();

        Vector2 d = enemy.CurrentDirection();
        if (d == Vector2.zero) {
            for (int i = 0; i < directions.Length; i++) {
                if (enemy.CanGo(directions[i])) {
                    canGo.Add(directions[i]);
                }
            }
            return canGo[UnityEngine.Random.Range(0, canGo.Count)];
        }
        if (enemy.IsAtNode() || UnityEngine.Random.value > .99f) {
            for (int i = 0; i < directions.Length; i++) {
                if (enemy.CanGo(directions[i]) && directions[i] != d && directions[i] != -d) {
                    canGo.Add(directions[i]);
                }
                if (canGo.Count == 0 || UnityEngine.Random.value > .99f ) {
                    if (enemy.CanGo(directions[i]) && directions[i] != d) {
                        canGo.Add(directions[i]);
                    }
                    if (canGo.Count == 0 || UnityEngine.Random.value > .99f) {
                        if (enemy.CanGo(directions[i])) {
                            canGo.Add(directions[i]);
                        }
                    }
                }
            }
            if (UnityEngine.Random.value > 0.5f || !canGo.Contains(d)) {
                return canGo[UnityEngine.Random.Range(0, canGo.Count)];
            }
        }
        return Vector2.zero;
    }


    public void SetEnemy(Enemy enemy) {
        this.enemy = enemy;
    }


}

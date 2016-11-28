using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomAI : MonoBehaviour, AI {

    public bool favorDots = false;

    private Enemy enemy;
    private Game game;
    private Board board;

    Vector2[] directions = { new Vector2(1, 0), new Vector2(0, 1),
        new Vector2(-1, 0), new Vector2(0, -1) };

    List<Vector2> canGo = new List<Vector2>();

    public Vector2 GetDirection() {
        if (game == null) {
            game = FindObjectOfType<Game>();
        }
        if (board == null) {
            board = FindObjectOfType<Board>();
        }

        canGo.Clear();
        for (int i = 0; i < directions.Length; i++) {
            if (enemy.CanGo(directions[i])) {
                canGo.Add(directions[i]);
            }
        }
        if (canGo.Count == 0) {
            Debug.LogError("enemy is stuck!");
            return Vector2.zero;
        }
        Vector2 d = enemy.CurrentDirection();

        if (favorDots) {
            if (d == Vector2.zero || !enemy.CanGo(d) || WouldCollide(d) || !IsDot(d)) {
                Vector2 dd = canGo[UnityEngine.Random.Range(0, canGo.Count)];
                if (!IsDot(dd)) {
                    dd = canGo[UnityEngine.Random.Range(0, canGo.Count)];
                }
                if (!IsDot(dd)) {
                    dd = canGo[UnityEngine.Random.Range(0, canGo.Count)];
                }
                if (IsDot(dd)) {
                    return dd;
                }
            }

        }
        
        if (d == Vector2.zero || !enemy.CanGo(d) || WouldCollide(d)) {
            Vector2 dd =  canGo[UnityEngine.Random.Range(0, canGo.Count)];
            if (WouldCollide(dd)) {
                dd = canGo[UnityEngine.Random.Range(0, canGo.Count)];
            }
            if (WouldCollide(dd)) {
                dd = canGo[UnityEngine.Random.Range(0, canGo.Count)];
            }
            if (!WouldCollide(dd)) {
                return dd;
            }
        }

        Vector2 d1 = new Vector2(d.y, d.x);
        Vector2 d2 = new Vector2(-d.y, -d.x);

        if ((enemy.CanGo(d1) || enemy.CanGo(d2)) && Random.value > .95f) {
            Vector2 dd = canGo[UnityEngine.Random.Range(0, canGo.Count)];
            if (WouldCollide(dd)) {
                dd = canGo[UnityEngine.Random.Range(0, canGo.Count)];
            }
            if (WouldCollide(dd)) {
                dd = canGo[UnityEngine.Random.Range(0, canGo.Count)];
            }
            if (!WouldCollide(dd)) {
                return dd;
            }
        }

        if (enemy.CanGo(d) && !WouldCollide(d)) {
            return d;
        }
        return Vector2.zero;
    }


    public void SetEnemy(Enemy enemy) {
        this.enemy = enemy;
    }

    bool IsDot(Vector2 d) {
        int dx = (int)Mathf.Round(d.x);
        int dy = (int)Mathf.Round(d.y);
        int x = (int)Mathf.Round(enemy.transform.position.x) + dx;
        int y = (int)Mathf.Round(enemy.transform.position.y) + dy;
        if (board[x, y] == Board.CellType.DOT) {
            return true;
        }
        return false;
    }

    bool WouldCollide(Vector2 d) {
        int dx = (int)Mathf.Round(d.x);
        int dy = (int)Mathf.Round(d.y);
        int x = (int)Mathf.Round(enemy.transform.position.x) + dx;
        int y = (int)Mathf.Round(enemy.transform.position.y) + dy;
        foreach (Enemy e in game.GetEnemies()) {
            if (e != enemy) {
                int ex = (int)Mathf.Round(e.transform.position.x);
                int ey = (int)Mathf.Round(e.transform.position.y);
                if (ex == x && ey == y) {
                    return true;
                }
            }
        }

        return false;
    }

}

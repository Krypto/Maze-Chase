using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointAI : MonoBehaviour, AI {

    private WaypointGrid grid;

    private Enemy enemy;
    private Player player = null;
    private Navigation navigation = null;
    private Board board = null;
    private Game game;

    Vector2[] directions = { new Vector2(1, 0), new Vector2(0, 1),
        new Vector2(-1, 0), new Vector2(0, -1) };

    List<Vector2> canGo = new List<Vector2>();

    private float moveRandomlyTime = 0f;

    public Vector2 GetDirection() {

        if (board == null) {
            board = FindObjectOfType<Board>();
        }

        if (game == null) {
            game = FindObjectOfType<Game>();
        }

        if (player == null) {
            player = FindObjectOfType<Player>();
        }
        if (navigation == null) {
            navigation = FindObjectOfType<Navigation>();
        }

        if (moveRandomlyTime > 0) {
            return GetRandomDirection();
        }

        if (grid == null) {
            grid = (Instantiate(navigation.waypointGridPrefab, transform) as GameObject).GetComponent<WaypointGrid>();
            grid.waypointX = (int)Mathf.Round(player.transform.position.x);
            grid.waypointY = (int)Mathf.Round(player.transform.position.y);
            Debug.Log("Waypoint grid created");
        }

        if (Random.value > 0.995f) {
            ResetWaypoint();
        }

        Navigation.Pair p = navigation.DirectionToWaypoint(grid,
                    (int)Mathf.Round(enemy.transform.position.x),
                    (int)Mathf.Round(enemy.transform.position.y));

        if (p.x == 0 && p.y == 0) {
            ResetWaypoint();
            p = navigation.DirectionToWaypoint(grid,
                    (int)Mathf.Round(enemy.transform.position.x),
                    (int)Mathf.Round(enemy.transform.position.y));
        }

        if (WouldCollide(p) || (p.x == 0 && p.y == 0)) {
            moveRandomlyTime = 5f;
            return GetRandomDirection();
        }

        return new Vector2(p.x, p.y);
    }

    bool WouldCollide(Navigation.Pair p) {
        int x = (int)Mathf.Round(enemy.transform.position.x) + p.x;
        int y = (int)Mathf.Round(enemy.transform.position.y) + p.y;
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

    public Vector2 GetRandomDirection() {
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
        Navigation.Pair p = new Navigation.Pair((int)Mathf.Round(d.x), (int)Mathf.Round(d.y));

        if (d == Vector2.zero || !enemy.CanGo(d) || WouldCollide(p)) {
            Vector2 dd = canGo[UnityEngine.Random.Range(0, canGo.Count)];
            p = new Navigation.Pair((int)Mathf.Round(dd.x), (int)Mathf.Round(dd.y));
            if (WouldCollide(p)) {
                dd = canGo[UnityEngine.Random.Range(0, canGo.Count)];
                p = new Navigation.Pair((int)Mathf.Round(dd.x), (int)Mathf.Round(dd.y));
            }
            if (WouldCollide(p)) {
                dd = canGo[UnityEngine.Random.Range(0, canGo.Count)];
                p = new Navigation.Pair((int)Mathf.Round(dd.x), (int)Mathf.Round(dd.y));
            }
            if (!WouldCollide(p)) {
                return dd;
            }
        }

        Vector2 d1 = new Vector2(d.y, d.x);
        Vector2 d2 = new Vector2(-d.y, -d.x);

        if ((enemy.CanGo(d1) || enemy.CanGo(d2)) && Random.value > .95f) {
            Vector2 dd = canGo[UnityEngine.Random.Range(0, canGo.Count)];
            p = new Navigation.Pair((int)Mathf.Round(dd.x), (int)Mathf.Round(dd.y));
            if (WouldCollide(p)) {
                dd = canGo[UnityEngine.Random.Range(0, canGo.Count)];
                p = new Navigation.Pair((int)Mathf.Round(dd.x), (int)Mathf.Round(dd.y));
            }
            if (WouldCollide(p)) {
                dd = canGo[UnityEngine.Random.Range(0, canGo.Count)];
                p = new Navigation.Pair((int)Mathf.Round(dd.x), (int)Mathf.Round(dd.y));
            }
            if (!WouldCollide(p)) {
                return dd;
            }
        }
        p = new Navigation.Pair((int)Mathf.Round(d.x), (int)Mathf.Round(d.y));
        if (enemy.CanGo(d) && !WouldCollide(p)) {
            return d;
        }
        return Vector2.zero;
    }

    public void ResetWaypoint() {
        grid.waypointX = (int)Mathf.Round(player.transform.position.x);
        grid.waypointY = (int)Mathf.Round(player.transform.position.y);
        Debug.Log("Waypoint grid recreated");
    }

    public void SetEnemy(Enemy enemy) {
        this.enemy = enemy;
    }

    private void Update() {
        if (moveRandomlyTime > 0) {
            moveRandomlyTime -= Time.deltaTime;
            if (moveRandomlyTime <= 0) {
                moveRandomlyTime = 0;
            }
        }
    }


}

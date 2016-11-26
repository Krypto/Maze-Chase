using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointAI : MonoBehaviour, AI {


    private WaypointGrid grid;

    private Enemy enemy;
    private Player player = null;
    private Navigation navigation = null;


    public Vector2 GetDirection() {
        if (player == null) {
            player = FindObjectOfType<Player>();
        }
        if (navigation == null) {
            navigation = FindObjectOfType<Navigation>();
        }
        if (grid == null) {
            grid = (Instantiate(navigation.waypointGridPrefab, transform) as GameObject).GetComponent<WaypointGrid>();
            grid.waypointX = (int)Mathf.Round(player.transform.position.x);
            grid.waypointY = (int)Mathf.Round(player.transform.position.y);
            Debug.Log("Waypoint grid created");
        }

        if(Random.value > 0.995f) {
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

        return new Vector2(p.x, p.y);
    }

    public void ResetWaypoint() {
        grid.waypointX = (int)Mathf.Round(player.transform.position.x);
        grid.waypointY = (int)Mathf.Round(player.transform.position.y);
        Debug.Log("Waypoint grid recreated");
    }

    public void SetEnemy(Enemy enemy) {
        this.enemy = enemy;
    }


}

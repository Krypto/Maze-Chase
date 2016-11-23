using UnityEngine;
using System.Collections;

public class BigDot : MonoBehaviour {

    private ScoreDisplay scoreDisplay;
    private Board board;

    private void OnTriggerEnter2D(Collider2D collider) {
        //Debug.Log("Trigger entered by " + collider.gameObject);
        if (collider.gameObject.tag == "Player") {
            Gulp(collider.gameObject.GetComponent<Player>());
        }
    }

    void Start() {
        board = FindObjectOfType<Board>();
        scoreDisplay = board.scoreDisplay;
    }

    void Gulp(Player player) {

        //make gulp sound
        scoreDisplay.Advance(50);
        player.AttackMode();
        Destroy(gameObject);
    }
}

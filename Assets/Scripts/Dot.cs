using UnityEngine;
using System.Collections;

public class Dot : MonoBehaviour {

    private ScoreDisplay scoreDisplay;
    private Board board;

    private void OnTriggerEnter2D(Collider2D collider) {
        //Debug.Log("Trigger entered by " + collider.gameObject);
        if (collider.gameObject.tag == "Player") {
            Gulp();
        }
    }

    void Start() {
        board = FindObjectOfType<Board>();
        scoreDisplay = board.scoreDisplay;
    }

    void Gulp() {

        //make gulp sound
        scoreDisplay.Advance(10);
        Destroy(gameObject);
    }
}

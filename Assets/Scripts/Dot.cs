using UnityEngine;
using System.Collections;

public class Dot : MonoBehaviour {

    private ScoreDisplay scoreDisplay;
    private Game game;

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.tag == "Player") {
            Gulp();
        }
    }

    void Start() {
        game = FindObjectOfType<Game>();
        scoreDisplay = game.scoreDisplay;
    }

    void Gulp() {

        //make gulp sound
        scoreDisplay.Advance(10);
        Destroy(gameObject);
    }
}

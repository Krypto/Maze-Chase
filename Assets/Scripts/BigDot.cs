using UnityEngine;
using System.Collections;

public class BigDot : MonoBehaviour {

    private ScoreDisplay scoreDisplay;
    private Game game;

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.tag == "Player") {
            Gulp(collider.gameObject.GetComponent<Player>());
        }
    }

    void Start() {
        game = FindObjectOfType<Game>();
        scoreDisplay = game.scoreDisplay;
    }

    void Gulp(Player player) {

        //make big gulp sound
        scoreDisplay.Advance(50);
        player.AttackMode();
        Destroy(gameObject);
    }
}

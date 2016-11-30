using UnityEngine;
using System.Collections;

public class BonusItem : MonoBehaviour {

    public AudioClip gulpSound;

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

        AudioSource.PlayClipAtPoint(gulpSound, Camera.main.transform.position);
        scoreDisplay.Advance(800);
        Destroy(gameObject);
    }
}

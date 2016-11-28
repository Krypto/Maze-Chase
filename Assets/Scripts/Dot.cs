using UnityEngine;
using System.Collections;

public class Dot : MonoBehaviour {

    public AudioClip gulpSound;

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
        AudioSource.PlayClipAtPoint(gulpSound, Camera.main.transform.position);
        scoreDisplay.Advance(10);
        Destroy(gameObject);
    }
}

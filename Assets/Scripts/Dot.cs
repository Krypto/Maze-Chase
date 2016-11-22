using UnityEngine;
using System.Collections;

public class Dot : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collider) {
        //Debug.Log("Trigger entered by " + collider.gameObject);
        if (collider.gameObject.tag == "Player") {
            Gulp();
        }
    }

    void Gulp() {
        Debug.Log("Gulp");

        //make gulp sound
        //add to score
        Destroy(gameObject);
    }
}

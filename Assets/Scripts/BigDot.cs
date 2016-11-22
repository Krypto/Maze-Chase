using UnityEngine;
using System.Collections;

public class BigDot : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collider) {
        //Debug.Log("Trigger entered by " + collider.gameObject);
        if (collider.gameObject.tag == "Player") {
            Gulp();
        }
    }

    void Gulp() {
        Debug.Log("Big Gulp");

        //make gulp sound
        //add to score
        //switch to attack mode
        Destroy(gameObject);
    }
}

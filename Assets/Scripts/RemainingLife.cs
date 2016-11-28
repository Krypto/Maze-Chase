using UnityEngine;
using System.Collections;

public class RemainingLife : MonoBehaviour {
    public int numLives = 3;
    public GameObject lifePrefab;
    public Vector3 position = new Vector3(-12.5f, 2.5f, -5f);

    int numLivesDisplayed = 0;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if(numLivesDisplayed != numLives) {
            numLivesDisplayed = numLives;
            Clear();
            Show();
        }
    }

    void Show() {
        for (int i = 0; i < numLives; i++) {
            GameObject life = Instantiate(lifePrefab, transform) as GameObject;
            life.transform.position = position + Vector3.up * i;
        }
    }

    void Clear() {
        foreach(Transform t in transform) {
            Destroy(t.gameObject);
        }
    }
}

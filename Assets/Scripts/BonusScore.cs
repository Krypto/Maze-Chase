using UnityEngine;
using System.Collections;

public class BonusScore : MonoBehaviour {
    public Color color = Color.green;
    public float timeToLive = 1f;

    float time = 0;
    SpriteRenderer sr;
    // Use this for initialization
    void Start() {
        sr = GetComponent<SpriteRenderer>();
        sr.color = color;
        transform.localScale = new Vector3(.1f, .1f, 1);
        time = Time.time;
    }

    // Update is called once per frame
    void Update() {
        if (Time.time - time > timeToLive) {
            Destroy(gameObject);
        } else {
            float alpha = 1 - (Time.time - time)/timeToLive;
            float size = 2f - alpha * 2 + .1f;
            sr.color = new Color(color.r, color.g, color.b, alpha);
            transform.localScale = new Vector3(size, size, 1);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreDisplay : MonoBehaviour {

    public string prefixText = "Score: ";
    public int score = 0;
    public Color highlightColor = Color.red;
    public bool highlight;
    public float highlightTime = 1.5f;


    private Text scoreText;
    private Color textColor = Color.green;
    private float highlightAmount = 1f;


    // Use this for initialization
    void Start() {
        scoreText = GetComponentInChildren<Text>();
        scoreText.text = prefixText + score.ToString();
        textColor = scoreText.color;
    }

    // Update is called once per frame
    void Update() {
        scoreText.text = prefixText + score.ToString();
        if (highlight) {
            if (highlightAmount > 0) {
                scoreText.color = textColor*(1f-highlightAmount) + highlightColor*highlightAmount;
                highlightAmount -= Time.deltaTime / highlightTime;
            }else {
                highlight = false;
                highlightAmount = 1f;
                scoreText.color = textColor;
            }
        }
    }

    public void Highlight() {
        highlight = true;
        highlightAmount = 1f;
    }

    public void Advance(int amount) {
        score += amount;
    }

}

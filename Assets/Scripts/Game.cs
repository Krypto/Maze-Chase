using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {
    int level = 0;
    public ScoreDisplay scoreDisplay;
    public ScoreDisplay highScoreDisplay;

    public void StartGame() {
        level = 1;
        scoreDisplay.score = 0;
        highScoreDisplay.prefixText = "High Score: ";
        StartLevel();
    }

    public void StartLevel() {
        //clear board, build board
        RestartLevel();
    }

    public void RestartLevel() {
        //subtract a life
        //Move player and enemies back to start position
    }

    public void LevelUp() {
        level++;
        StartLevel();
    }

    public void GameOver() {

    }

    private void Update() {
        if(scoreDisplay.score > highScoreDisplay.score) {
            highScoreDisplay.score = scoreDisplay.score;
        }
    }

}

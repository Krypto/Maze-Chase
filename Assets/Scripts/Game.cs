using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {
    int level = 0;
    public int randomSeed;
    public ScoreDisplay scoreDisplay;
    public ScoreDisplay highScoreDisplay;

    Player player;
    Board board;

    private void Start() {
        board = FindObjectOfType<Board>();
    }

    public void StartGame() {
        level = 1;
        scoreDisplay.score = 0;
        highScoreDisplay.prefixText = "High Score: ";
        StartLevel(true);
        UnityEngine.Random.InitState(randomSeed);
    }

    public void StartLevel(bool firstTime = false) {
        if (firstTime) {
            //board should already be built
            StartGameSequence();
            Invoke("RestartLevel", 3f);
        }else {
            Invoke("NewBoard", 1f);
            Invoke("RestartLevel", 1.1f);
        }        
    }

    public void NewBoard() {
        board.Clear();
        Enemy.allEnemies.Clear();
        board.Build(level);
    }

    public void StartGameSequence() {

    }

    public void EndTurn() {
        Invoke("RestartLevel", 1f);
    }

    public void RestartLevel() {
        //subtract a life
        player = FindObjectOfType<Player>();
        //Move player and enemies back to start position
    }

    public void LevelUp() {
        level++;
        MaybeCutScene();
    }

    public void MaybeCutScene() {
        bool cutscene = false;
        //if level==3, and several other values, cutscene=true
        if (cutscene) {
            float delay = CutScene();
            Invoke("StartLevel", delay);
        } else {
            StartLevel();
        }
    }

    public float CutScene() {
        return 1f;
    }

    public void GameOver() {
        //save high score
        //delay
        //back to start menu
    }

    private void Update() {
        if(scoreDisplay.score > highScoreDisplay.score) {//watch for high score
            highScoreDisplay.score = scoreDisplay.score;
        }
    }

}

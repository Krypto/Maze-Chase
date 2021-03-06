﻿using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {
    public int randomSeed = 8675309;
    public ScoreDisplay scoreDisplay;
    public ScoreDisplay highScoreDisplay;
    public int initialNumLives = 3;
    public GameObject[] EnemyPrefab;
    public AudioClip dieSound;
    public AudioClip bonusLifeSound;
    public AudioClip startGameSound;
    public GameObject titlePrefab;
    public GameObject[] cutscenePrefab;
    public GameObject bonusItemPrefab;


    public float startGameSequenceTime = 4f;
    public float SpawnPlayerTime = 2f;
    public float deathSequenceTime = 2f;
    public float gameOverTime = 4f;
    public float beforeLevelUpTime = 2f;
    public float levelUpTime = 2f;
    public float ResumePlayTime = 1f;
    public float replayTime = 2f;
    public float startMusicTime = 4f;

    public bool gameIsPlaying {
        get {
            return _gameIsPlaying;
        }
    }

    Player player;
    Board board;
    Navigation navigation;
    Enemy[] enemies;
    int level = 0;
    int numLives;
    bool _gameIsPlaying = false;
    private int nextBonusLife = 10000;

    public Enemy[] GetEnemies() {
        return enemies;
    }

    public void PausePlay() {
        Debug.Log("Pause Play");
        _gameIsPlaying = false;
        player.Pause();
        foreach (Enemy enemy in enemies) {
            enemy.Pause();
        }
    }

    public void ResumePlay() {
        Debug.Log("Resume Play");
        _gameIsPlaying = true;
        player.Resume();
        foreach (Enemy enemy in enemies) {
            enemy.Resume();
        }
    }

    public void EndTurn() {
        StopPlay();
        DeathSequence();
        numLives--;
        if (numLives > 0) {
            Invoke("Replay", deathSequenceTime);
        } else {
            Invoke("GameOver", deathSequenceTime);
        }

    }

    public void LevelUp() {
        StopPlay();
        board.Clear();
        level++;
        float cutsceneTime = MaybeCutScene();
        ShowBoard();
        SpawnPlayer();
        SpawnEnemies();
        Invoke("StartPlay", levelUpTime + cutsceneTime);
    }

    private void Start() {
        nextBonusLife = 10000;
        highScoreDisplay.score = PlayerPrefs.GetInt("HighScore");
        Instantiate(titlePrefab, new Vector3(0, 0, -7f), Quaternion.identity);
        PreGame();
    }

    private void Update() {
        if (Random.value > .999f) {
            if (!FindObjectOfType<BonusItem>()) {
                GameObject pb = Instantiate(bonusItemPrefab, board.GetBonusPosition(), Quaternion.identity) as GameObject;                
                pb.transform.SetParent(board.GetCellParent(Board.CellType.BONUS_PICKUP).transform);
            }
        }
        if (scoreDisplay.score > highScoreDisplay.score) {//watch for high score
            highScoreDisplay.score = scoreDisplay.score;
        }
        if (!board.HasDots() && gameIsPlaying) {
            StopPlay();
            Invoke("LevelUp", beforeLevelUpTime);
        }
        if (scoreDisplay.score > nextBonusLife) {
            numLives++;
            FindObjectOfType<RemainingLife>().numLives = numLives - 1;
            AudioSource.PlayClipAtPoint(bonusLifeSound, Camera.main.transform.position);
            nextBonusLife *= 2;
        }
    }

    void PreGame() {
        Initialize();
        ShowBoard();
        StartGameSequence();
        Invoke("SpawnPlayer", SpawnPlayerTime);
        Invoke("SpawnEnemies", SpawnPlayerTime);
        Invoke("StartPlay", startGameSequenceTime);
        Invoke("StartMusic", startMusicTime);
    }

    void StartMusic() {
        GetComponent<AudioSource>().Play();
    }

    void StopMusic() {
        GetComponent<AudioSource>().Stop();
    }

    void Initialize() {
        UnityEngine.Random.InitState(randomSeed);
        level = 1;
        scoreDisplay.score = 0;
        highScoreDisplay.prefixText = "High Score: ";
        numLives = initialNumLives;
        FindObjectOfType<RemainingLife>().numLives = numLives;
    }

    void ShowBoard() {
        board = FindObjectOfType<Board>();
        if (level == 1) {
            board.setBoardDesignTexture(board.firstBoard);

        } else {
            board.setBoardDesignTexture(board.boards[(level - 1) % board.boards.Length]);
        }
        board.Build();
        board.Show(level);
        navigation = FindObjectOfType<Navigation>();
        navigation.Build(board);
    }

    void StartGameSequence() {
        AudioSource.PlayClipAtPoint(startGameSound, Camera.main.transform.position);
    }

    public void SpawnPlayer() {
        FindObjectOfType<RemainingLife>().numLives = numLives - 1;
        player = (Instantiate(board.GetCellPrefab(Board.CellType.PLAYER),
            board.GetCellParent(Board.CellType.PLAYER).transform) as GameObject).GetComponent<Player>();
        player.transform.position = board.GetPlayerPosition();
    }

    void SpawnEnemies() {
        int i = 0;
        enemies = new Enemy[board.GetEnemyPositions().Count];
        foreach (Vector3 p in board.GetEnemyPositions()) {
            enemies[i] = (Instantiate(EnemyPrefab[i],
board.GetCellParent(Board.CellType.ENEMY).transform) as GameObject).GetComponent<Enemy>();

            enemies[i].transform.position = p;
            i++;
        }
    }

    void StartPlay() {
        ResumePlay();
    }

    void StopPlay() {
        PausePlay();
    }

    float MaybeCutScene() {
        bool cutscene = (level==3) || (level==5) || (level==8) || (level % 7 == 0);       
        if (cutscene) {
            return CutScene();
        }
        return 0f;
    }

    float CutScene() {
        Instantiate(cutscenePrefab[level % cutscenePrefab.Length], new Vector3(0, 0, -7f), Quaternion.identity);
        return 3f;
    }

    void DeathSequence() {
        AudioSource.PlayClipAtPoint(dieSound, Camera.main.transform.position);
        Destroy(player.gameObject);
    }

    void Replay() {
        SpawnPlayer();
        int i = 0;
        foreach (Vector3 p in board.GetEnemyPositions()) {
            enemies[i].transform.position = p;
            i++;
        }
        Invoke("StartPlay", ResumePlayTime);//required or race condition
    }

    void GameOver() {
        PlayerPrefs.SetInt("HighScore", highScoreDisplay.score);
        StopMusic();
        Invoke("MainMenu", gameOverTime);
    }

    void MainMenu() {
        board.Clear();
        Invoke("Start", replayTime);
    }

}

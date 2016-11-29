using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {
    public int randomSeed = 8675309;
    public ScoreDisplay scoreDisplay;
    public ScoreDisplay highScoreDisplay;
    public int initialNumLives = 3;
    public GameObject[] EnemyPrefab;
    public AudioClip dieSound;
    public GameObject titlePrefab;


    public float startGameSequenceTime = 4f;
    public float SpawnPlayerTime = 2f;
    public float deathSequenceTime = 2f;
    public float gameOverTime = 4f;
    public float beforeLevelUpTime = 2f;
    public float levelUpTime = 2f;
    public float ResumePlayTime = 1f;
    public float replayTime = 2f;
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
        highScoreDisplay.score = PlayerPrefs.GetInt("HighScore");
        Instantiate(titlePrefab, new Vector3(0,0,-7f), Quaternion.identity);
        PreGame();
    }

    private void Update() {
        if (scoreDisplay.score > highScoreDisplay.score) {//watch for high score
            highScoreDisplay.score = scoreDisplay.score;
        }
        if (!board.HasDots() && gameIsPlaying) {
            StopPlay();
            Invoke("LevelUp", beforeLevelUpTime);
        }
    }

    void PreGame() {
        Initialize();
        ShowBoard();
        StartGameSequence();
        Invoke("SpawnPlayer", SpawnPlayerTime);
        Invoke("SpawnEnemies", SpawnPlayerTime);
        Invoke("StartPlay", startGameSequenceTime);
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
        board.setBoardDesignTexture(board.boards[(level - 1) % board.boards.Length]);
        board.Build();
        board.Show(level);
        navigation = FindObjectOfType<Navigation>();
        navigation.Build(board);
    }

    void StartGameSequence() {

    }

    void SpawnPlayer() {
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
        bool cutscene = false;
        //if level==3, and several other values, cutscene=true
        if (cutscene) {
            return CutScene();
        }
        return 0f;
    }

    float CutScene() {
        return 1f;
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
        Invoke("MainMenu", gameOverTime);
    }

    void MainMenu() {
        Invoke("Start", replayTime);
    }

}

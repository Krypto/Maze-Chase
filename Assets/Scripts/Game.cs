using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {
    public int randomSeed = 8675309;
    public ScoreDisplay scoreDisplay;
    public ScoreDisplay highScoreDisplay;
    public int initialNumLives = 3;

    public Color[] enemyColors = {
        Color.green,
        Color.magenta,
        Color.cyan,
        Color.blue
    };

    public float startGameSequenceTime = 4f;
    public float SpawnPlayerTime = 2f;
    public float deathSequenceTime = 2f;
    public float gameOverTime = 4f;
    public float levelUpTime = 2f;
    public float ResumePlayTime = 1f;
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
        _gameIsPlaying = false;
        player.Pause();
        foreach(Enemy enemy in enemies) {
            enemy.Pause();
        }
    }

    public void ResumePlay() {
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
        PreGame();
    }

    private void Update() {
        if (scoreDisplay.score > highScoreDisplay.score) {//watch for high score
            highScoreDisplay.score = scoreDisplay.score;
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
    }

    void ShowBoard() {
        board = FindObjectOfType<Board>();
        board.Build();
        board.Show();
        navigation = FindObjectOfType<Navigation>();
        navigation.Build(board);
    }

    void StartGameSequence() {

    }

    void SpawnPlayer() {
        player = (Instantiate(board.GetCellPrefab(Board.CellType.PLAYER),
            board.GetCellParent(Board.CellType.PLAYER).transform) as GameObject).GetComponent<Player>();
        player.transform.position = board.GetPlayerPosition();
    }

    void SpawnEnemies() {
        int i = 0;
        enemies = new Enemy[board.GetEnemyPositions().Count];
        foreach (Vector3 p in board.GetEnemyPositions()) {          
            enemies[i] = (Instantiate(board.GetCellPrefab(Board.CellType.ENEMY),
            board.GetCellParent(Board.CellType.ENEMY).transform) as GameObject).GetComponent<Enemy>();
            enemies[i].transform.position = p;
            enemies[i].GetComponent<SpriteRenderer>().color = enemyColors[i];
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
        //save high score
        Invoke("MainMenu", gameOverTime);
    }

    void MainMenu() {

    }

}

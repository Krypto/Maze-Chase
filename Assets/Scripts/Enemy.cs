using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {
    public bool favorDots = false;

    public float speed = 10f;
    public GameObject aiPrefab;

    public float initialTimeToAwake = 1f;

    private Game game;
    private Board board;
    private Navigation navigation;
    private AI ai;
    float timeToAwake;

    float currentDx = 0;
    float currentDy = 0;

    bool isPlaying = true;

    bool deployed = false;

    public bool Deployed() {
        return deployed;
    }

    public Vector2 CurrentDirection() {
        return new Vector2(currentDx, currentDy);
    }

    public bool CanGo(Vector2 direction) {
        int x = (int)Mathf.Round(transform.position.x);
        int y = (int)Mathf.Round(transform.position.y);

        int dx = (int)direction.x;
        int dy = (int)direction.y;
        if (dx == 0 && dy == -1) {
            return (navigation[x, y] & Navigation.Directions.UP) != 0;
        } else if (dx == 0 && dy == 1) {
            return (navigation[x, y] & Navigation.Directions.DOWN) != 0;
        } else if (dx == -1 && dy == 0) {
            return (navigation[x, y] & Navigation.Directions.LEFT) != 0;
        } else if (dx == 1 && dy == 0) {
            return (navigation[x, y] & Navigation.Directions.RIGHT) != 0;
        } else {
            return (navigation[x, y] & Navigation.Directions.STAY) != 0;
        }
    }

    // Use this for initialization
    void Start() {
        board = FindObjectOfType<Board>();
        navigation = FindObjectOfType<Navigation>();
        game = FindObjectOfType<Game>();
        ai = (Instantiate(aiPrefab, transform.parent) as GameObject).GetComponent<AI>();
        ai.SetEnemy(this);
        if(favorDots && ai is RandomAI) {
            ((RandomAI)ai).favorDots = true;
        }
        timeToAwake = initialTimeToAwake;
        deployed = false;
        Pause();
    }

    // Update is called once per frame
    void Update() {
        if (isPlaying) {
            if (timeToAwake > 0) {
                timeToAwake -= Time.deltaTime;
            } else {
                Move();
            }
        }
    }

    public void Sleep() {
        timeToAwake = initialTimeToAwake;
        SetAnimation(0, 0);
    }

    public void Die() {
        for (int i = 0; i < game.GetEnemies().Length; i++) {
            if (this == game.GetEnemies()[i]) {
                transform.position = board.GetEnemyPositions()[i];
                SetAnimation(0, 0);
                timeToAwake = initialTimeToAwake;
                break;
            }
        }
    }

    void Deploy() {
        deployed = true;
    }

    void Move() {
        Vector2 direction = ai.GetDirection();
        int x = (int)Mathf.Round(transform.position.x);
        int y = (int)Mathf.Round(transform.position.y);
        if (board[x, y] == Board.CellType.ENTRANCE) {
            Invoke("Deploy", 1f);
        }
        SetAnimation(direction.x, direction.y);

        //update the current motion
        currentDx = direction.x;
        currentDy = direction.y;

        //compute next position of player according to current motion
        int targetX = x;
        int targetY = y;

        if (direction.x > 0) {
            targetX++;
        } else if (direction.x < 0) {
            targetX--;
        }
        if (direction.y > 0) {
            targetY++;
        } else if (direction.y < 0) {
            targetY--;
        }

        //compute actual motion toward next position
        float fdx = targetX - transform.position.x;
        float fdy = targetY - transform.position.y;
        if (fdx > .1) {
            fdx = 1;
        } else if (fdx < -.1) {
            fdx = -1;
        }
        if (fdy > .1) {
            fdy = 1;
        } else if (fdy < -.1) {
            fdy = -1;
        }
        //teleport to matching tunnel if at end of a tunnel
        if (board[targetX, targetY] == Board.CellType.TELEPORT) {
            Teleport(targetX, targetY, x, y, direction);
        } else {
            transform.Translate(new Vector3(fdx * speed * Time.deltaTime, fdy * speed * Time.deltaTime, 0), Space.World);
        }
    }


    void Teleport(int targetX, int targetY, int x, int y, Vector2 direction) {
        navigation.FindMatchingTeleport(targetX, targetY, out x, out y);
        targetX = x;
        targetY = y;
        transform.position = new Vector3(targetX + 1.5f * direction.x, targetY + 1.5f * direction.y, transform.position.z);
    }

    void SetAnimation(float dx, float dy) {
        Animator a = GetComponent<Animator>();
        if ((dx == 0 && dy == 0) || !isPlaying || timeToAwake > 0) {
            a.SetBool("Walking", false);
        } else {
            a.SetBool("Walking", true);
            //point cat in proper direction
            if (dy > 0) {
                transform.localEulerAngles = new Vector3(0, 0, 0f);
            } else if (dy < 0) {
                transform.localEulerAngles = new Vector3(0, 0, 180f);
            } else if (dx > 0) {
                transform.localEulerAngles = new Vector3(0, 0, -90f);
            } else {
                transform.localEulerAngles = new Vector3(0, 0, 90f);
            }
        }
    }

    public void Pause() {
        isPlaying = false;
        SetAnimation(0, 0);
    }

    public void Resume() {
        isPlaying = true;
    }
}

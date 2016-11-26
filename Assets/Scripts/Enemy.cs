using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {

    public float speed = 10f;
    public GameObject aiPrefab;

    private Game game;
    private Board board;
    private Navigation navigation;
    private Player player;
    private AI ai;

    float currentDx = 0;
    float currentDy = 0;

    bool isPlaying = true;
    
    public Vector2 CurrentDirection() {
        return new Vector2(currentDx, currentDy);
    }

    public bool CanGo(Vector2 direction) {
        int x = (int)Mathf.Round(transform.position.x);
        int y = (int)Mathf.Round(transform.position.y);

        int dx = (int)direction.x;
        int dy = (int)direction.y;
        if (dx == 0 && dy == -1) {
            return (navigation[x, y] & Navigation.Directions.ENEMY_UP) != 0;
        } else if (dx == 0 && dy == 1) {
            return (navigation[x, y] & Navigation.Directions.ENEMY_DOWN) != 0;
        } else if (dx == -1 && dy == 0) {
            return (navigation[x, y] & Navigation.Directions.ENEMY_LEFT) != 0;
        } else if (dx == 1 && dy == 0) {
            return (navigation[x, y] & Navigation.Directions.ENEMY_RIGHT) != 0;
        } else {
            return (navigation[x, y] & Navigation.Directions.ENEMY_STAY) != 0;
        }
    }
    
    // Use this for initialization
    void Start() {
        board = FindObjectOfType<Board>();
        navigation = FindObjectOfType<Navigation>();
        game = FindObjectOfType<Game>();
        player = FindObjectOfType<Player>();
        ai = (Instantiate(aiPrefab, transform.parent) as GameObject).GetComponent<AI>();
        ai.SetEnemy(this);
        Pause();
    }   

    // Update is called once per frame
    void Update() {
        if (isPlaying) {
            Move();
        }
    }
    
    void Move() {
        Vector2 direction = ai.GetDirection();
        int x = (int)Mathf.Round(transform.position.x);
        int y = (int)Mathf.Round(transform.position.y);
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
            navigation.FindMatchingTeleport(targetX, targetY, out x, out y);
            targetX = x;
            targetY = y;
            //Debug.Log("Teleporting to " + targetX + ", " + targetY);
            transform.position = new Vector3(targetX, targetY, transform.position.z);
        }
        transform.Translate(new Vector3(fdx * speed * Time.deltaTime, fdy * speed * Time.deltaTime, 0), Space.World);
    }

    void SetAnimation(float dx, float dy) {
        Animator a = GetComponent<Animator>();
        if (dx == 0 && dy == 0) {
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

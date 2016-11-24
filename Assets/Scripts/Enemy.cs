using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {

    public static List<Enemy> allEnemies = new List<Enemy>();

    public float speed = 10f;
    public GameObject aiPrefab;

    private Board board;
    private Player player;
    private AI ai;

    float lastDx = 0;
    float lastDy = 0;

    int currentX;
    int currentY;

    public Vector2 CurrentPosition() {
        return new Vector2(currentX, currentY);
    }

    public Vector2 CurrentDirection() {
        return new Vector2(lastDx, lastDy);
    }

    public bool CanGo(Vector2 direction) {
        return !WouldCollide(currentX, currentY, direction.x, direction.y);
    }

    public bool IsAtNode() {
        return board.IsNode(currentX + 15, currentY + 20);
    }

    // Use this for initialization
    void Start() {
        board = FindObjectOfType<Board>();
        player = FindObjectOfType<Player>();
        ai = aiPrefab.GetComponent<AI>();
        ai.SetEnemy(this);
        allEnemies.Add(this);
    }

    // Update is called once per frame
    void Update() {
        Move();
    }

    void Move() {
        //motion is defined in terms of the grid
        currentX = (int)Mathf.Round(transform.position.x);
        currentY = (int)Mathf.Round(transform.position.y);

        //get AI input
        Vector2 d = ai.GetDirection();
        float dx = d.x;
        float dy = d.y;

        if (dx != 0 && dy != 0) {//contradictory motion, try to resolve
            if (dx < dy) {//favor x if possible, it was more recently pressed
                if (!WouldCollide(currentX, currentY, dx, 0)) {
                    dy = 0;
                } else {//favor y instead
                    dx = 0;
                }
            } else {//favor y if possible, it was more recently pressed
                if (!WouldCollide(currentX, currentY, 0, dy)) {
                    dx = 0;
                } else {//favor x instead
                    dy = 0;
                }
            }
        }

        if (dx != 0 && dy != 0) {//still contradictory
            if (dx < dy) {
                dy = 0;
            } else {
                dx = 0;
            }
        }
        if (WouldCollide(currentX, currentY, dx, dy) || (dx == 0 && dy == 0)) {
            /*If input motion results in collision, or if there is no input motion, 
             * revert to current motion*/
            dx = lastDx;
            dy = lastDy;
        }
        if (WouldCollide(currentX, currentY, dx, dy)) {//if still colliding, stop
            dx = 0;
            dy = 0;
        }

        SetAnimation(dx, dy);

        //update the current motion
        lastDx = dx;
        lastDy = dy;

        //compute next position of player according to current motion
        int targetX = currentX;
        int targetY = currentY;

        if (dx > 0) {
            targetX++;
        } else if (dx < 0) {
            targetX--;
        }
        if (dy > 0) {
            targetY++;
        } else if (dy < 0) {
            targetY--;
        }

        //compute actual motion toward next position
        dx = targetX - transform.position.x;
        dy = targetY - transform.position.y;
        if (dx > .1) {
            dx = 1;
        } else if (dx < -.1) {
            dx = -1;
        }
        if (dy > .1) {
            dy = 1;
        } else if (dy < -.1) {
            dy = -1;
        }

        //teleport to matching tunnel if at end of a tunnel
        if (board.Get(targetX, targetY) == Board.CellType.TELEPORT) {
            board.FindMatchingTeleport(targetX, targetY, out currentX, out currentY);
            targetX = currentX;
            targetY = currentY;
            Debug.Log("Teleporting to " + targetX + ", " + targetY);
            transform.position = new Vector3(targetX, targetY, transform.position.z);
        }
        transform.Translate(new Vector3(dx * speed * Time.deltaTime, dy * speed * Time.deltaTime, 0), Space.World);

    }

    //results meaningless if both dx and dy nonzero
    bool WouldCollide(int currentX, int currentY, float dx, float dy) {
        int x = currentX;
        int y = currentY;
        if (dx > 0) {
            x++;
        } else if (dx < 0) {
            x--;
        }
        if (dy > 0) {
            y++;
        } else if (dy < 0) {
            y--;
        }
        if (board.Get(x, y) == Board.CellType.WALL || board.Get(x, y) == Board.CellType.OUT_OF_BOUNDS) {
            return true;
        }

        foreach (Enemy enemy in allEnemies) {
            Vector2 p = enemy.CurrentPosition();
            if (enemy != this && p.x == x && p.y == y) {
                Debug.Log("Enemy " + gameObject + " would collide with enemy " + enemy.gameObject);
                return true;
            }
        }
        return false;
    }
    void SetAnimation(float dx, float dy) {
        //Animator a = GetComponent<Animator>();
        if (dx == 0 && dy == 0) {
            // a.SetBool("Walking", false);
        } else {
            // a.SetBool("Walking", true);
            //point cat in proper direction
            if (dy > 0) {
                transform.localEulerAngles = new Vector3(0, 0, 90f);
            } else if (dy < 0) {
                transform.localEulerAngles = new Vector3(0, 0, -90f);
            } else if (dx > 0) {
                transform.localEulerAngles = new Vector3(0, 0, 0);
            } else {
                transform.localEulerAngles = new Vector3(0, 0, 180f);
            }
        }
    }
}

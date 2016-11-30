using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour {
    public float speed = 10f;
    public float attackModeDuration = 7f;
    public int initialEnemyScore = 200;

    public GameObject bonus200;
    public GameObject bonus400;
    public GameObject bonus800;
    public GameObject bonus1600;


    private Board board;
    private Navigation navigation;
    private Game game;
    private PlayerController controller;
    private int enemyScore;

    float currentDx = 0;
    float currentDy = 0;

    float attackModeTimeRemaining = 0f;
    bool playing = false;

    private void Start() {
        enemyScore = 200;
        game = FindObjectOfType<Game>();
        board = FindObjectOfType<Board>();
        navigation = FindObjectOfType<Navigation>();
        controller = FindObjectOfType<PlayerController>();
        controller.SetPlayer(this);
        Pause();
    }

    // Update is called once per frame
    void Update() {
        if (playing) {
            Move();
            CheckAttackMode();
        }
    }

    public Vector2 CurrentDirection() {
        return new Vector2(currentDx, currentDy);
    }

    public bool CanGo(Vector2 direction) {
        int x = (int)Mathf.Round(transform.position.x);
        int y = (int)Mathf.Round(transform.position.y);
        //Debug.Log("x: " + x + "; y: " + y + "; nav: " + navigation[x,y]);

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

    public bool IsAttackMode() {
        return attackModeTimeRemaining > 0;
    }

    public void AttackMode() {
        attackModeTimeRemaining += attackModeDuration;
        transform.localScale = new Vector3(2, 2, 2);
    }

    public void EndAttackMode() {
        enemyScore = initialEnemyScore;
        attackModeTimeRemaining = 0;
        transform.localScale = new Vector3(1, 1, 1);
    }

    void CheckAttackMode() {
        if (attackModeTimeRemaining > 0) {
            attackModeTimeRemaining -= Time.deltaTime;
            if (attackModeTimeRemaining <= 0) {
                EndAttackMode();
            }
        }
    }


    void Move() {
        Vector2 direction = controller.GetDirection();
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
            Teleport(targetX, targetY, x, y, direction);
        } else {
            transform.Translate(new Vector3(fdx * speed * Time.deltaTime, fdy * speed * Time.deltaTime, 0), Space.World);
        }
    }

    void Teleport(int targetX, int targetY, int x, int y, Vector2 direction) {
        navigation.FindMatchingTeleport(targetX, targetY, out x, out y);
        targetX = x;
        targetY = y;
        transform.position = new Vector3(targetX + direction.x, targetY + direction.y, transform.position.z);

        x = (int)(Mathf.Round(transform.position.x));
        y = (int)(Mathf.Round(transform.position.y));
        if ((navigation[x, y] & Navigation.Directions.STAY) == 0) {
            Debug.LogError("Teleport to nowhere!!!!! " + x + "," + y);

        } else {
            transform.position = new Vector3(x, y, transform.position.z);
        }

    }


    void SetAnimation(float dx, float dy) {
        Animator a = GetComponent<Animator>();
        if (dx == 0 && dy == 0) {
            a.SetBool("Walking", false);
            transform.localEulerAngles = new Vector3(0, 0, 0);
        } else {
            a.SetBool("Walking", true);
            //point rat in proper direction
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

    private void OnTriggerEnter2D(Collider2D collider) {
        BonusItem bonusItem = collider.gameObject.GetComponent<BonusItem>();
        if (bonusItem) {
            Instantiate(bonus800, new Vector3(bonusItem.transform.position.x, bonusItem.transform.position.y, -6), Quaternion.identity);
        }

        Enemy enemy = collider.gameObject.GetComponent<Enemy>();
        if (enemy) {
            if (attackModeTimeRemaining > 0) {
                game.scoreDisplay.score += enemyScore;

                GameObject obj = null;
                switch (enemyScore) {
                    case 200:
                        obj = Instantiate(bonus200, new Vector3(enemy.transform.position.x, enemy.transform.position.y, -6), Quaternion.identity) as GameObject;
                        break;
                    case 400:
                        obj = Instantiate(bonus400, new Vector3(enemy.transform.position.x, enemy.transform.position.y, -6), Quaternion.identity) as GameObject;
                        break;
                    case 800:
                        obj = Instantiate(bonus800, new Vector3(enemy.transform.position.x, enemy.transform.position.y, -6), Quaternion.identity) as GameObject;
                        break;
                    case 1600:
                        obj = Instantiate(bonus1600, new Vector3(enemy.transform.position.x, enemy.transform.position.y, -6), Quaternion.identity) as GameObject;
                        break;
                    default:
                        Debug.LogError("Unaccounted for bonus score " + enemyScore);
                        break;
                }
                obj.GetComponent<BonusScore>().color = enemy.GetComponent<SpriteRenderer>().color;
                enemyScore *= 2;
                if (enemyScore > 1600) {
                    enemyScore = 1600;
                }
                enemy.Die();
            } else {
                Die();
            }
        }
    }

    public void Die() {
        game.EndTurn();
    }

    public void Pause() {
        SetAnimation(0, 0);
        playing = false;
    }

    public void Resume() {
        playing = true;
    }


}
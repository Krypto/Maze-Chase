using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour {

    public enum CellType {
        DOT = 0,
        WALL = 1,
        PLAYER = 2,
        ENEMY = 3,
        BONUS_PICKUP = 4,
        BIG_DOT = 5,
        ENEMY_ENTRANCE = 6,
        TELEPORT = 7,
        SCORE_DISPLAY = 8,
        REMAINING_LIFE = 9,
        EMPTY = 10,
        OUT_OF_BOUNDS = -1,
    }
   
    public GameObject boardDesignImage;
    public GameObject wallBox;
    public GameObject[] cellPrefabs;
    public GameObject[] cellParents;

    public Color[] colors;

    public bool editorShowBoardInScene = true;

    CellType[,] board = new CellType[30, 40];
    bool[,] nodes = new bool[30, 40];// places where AI should consider a direction change

    Texture2D boardDesignTexture;
    // Use this for initialization
    void Start() {
        boardDesignTexture = boardDesignImage.GetComponent<SpriteRenderer>().sprite.texture;
        Build();
    }

    public CellType Get(int x, int y) {
        return board[x + 15, y + 20];
    }

    public void Clear() {

    }

    public void FindMatchingTeleport(int x, int y, out int newX, out int newY) {
        float xWeight = ((x < 0) ? 15f + x : 15f - x) / 15f;
        float yWeight = ((y < 0) ? 20f + y : 20f - y) / 20f;
        if (xWeight < yWeight) {
            //probably a horizontal teleport
            if (x < 0) {
                //start looking on the right
                Debug.Log("Looking for teleport on the right");
                for (newX = 14; newX >= -15; newX--) {
                    for (newY = 19; newY >= -20; newY--) {
                        if (Get(newX, newY) == CellType.TELEPORT) {
                            return;
                        }
                    }
                }
            } else {
                //start looking on the left
                Debug.Log("Looking for teleport on the left");
                for (newX = -15; newX <= 14; newX++) {
                    for (newY = 19; newY >= -20; newY--) {
                        if (Get(newX, newY) == CellType.TELEPORT) {
                            return;
                        }
                    }
                }
            }
        } else {
            //probably a vertical teleport
            if (y < 0) {
                //start looking on the top
                Debug.Log("Looking for teleport on the top");
                for (newY = 19; newY >= -20; newY--) {
                    for (newX = 14; newX >= -15; newX--) {
                        if (Get(newX, newY) == CellType.TELEPORT) {
                            return;
                        }
                    }
                }
            } else {
                //start looking on the bottom
                Debug.Log("Looking for teleport on the bottom");
                for (newY = -20; newY <= 19; newY++) {
                    for (newX = 14; newX >= -15; newX--) {
                        if (Get(newX, newY) == CellType.TELEPORT) {
                            return;
                        }
                    }
                }
            }
        }
        newX = x;
        newY = y;
        Debug.LogError("Matching teleport not found!");
    }

    public void Build(int level = 1) {
        for (int x = 0; x < 30; x++) {
            for (int y = 0; y < 40; y++) {
                Color c = boardDesignTexture.GetPixel(x, y);
                if (c.a < 0.25) {
                    board[x, y] = CellType.OUT_OF_BOUNDS;
                } else {
                    board[x, y] = BuildPosition(x, y, c);
                }
            }
        }

        BuildNodes();
    }

    public void BuildNodes() {
        for (int x = 0; x < 30; x++) {
            for (int y = 0; y < 40; y++) {
                nodes[x, y] = false;
                CellType ct = Get(x, y);
                if(ct != CellType.WALL) {
                    //potential node

                }
            }
        }
    }

    public CellType BuildPosition(int x, int y, Color c) {
        CellType ct = ColorToCellType(c);

        switch (ct) {
            case CellType.DOT:
            case CellType.PLAYER:
            case CellType.ENEMY:
            case CellType.BIG_DOT:
            case CellType.ENEMY_ENTRANCE:
            case CellType.REMAINING_LIFE:
                SpawnSprite(cellPrefabs[(int)ct], x, y, cellParents[(int)ct]);
                break;
            case CellType.WALL:
                SpawnSprite(cellPrefabs[(int)ct], x, y, cellParents[(int)ct]);
                if (boardDesignTexture.GetPixel(x - 1, y) == c) {
                    GameObject box = SpawnSprite(wallBox, x - 0.5f, y, cellParents[(int)ct]);
                    box.transform.Translate(Vector3.back);

                }
                if (boardDesignTexture.GetPixel(x, y - 1) == c) {
                    GameObject box = SpawnSprite(wallBox, x, y - 0.5f, cellParents[(int)ct]);
                    box.transform.Translate(Vector3.back);
                }
                break;
            case CellType.BONUS_PICKUP:
                break;
            case CellType.TELEPORT:
                break;
            case CellType.SCORE_DISPLAY:
                break;
            case CellType.EMPTY:
                break;
            default:
                throw new UnityException("Unrecognized board design pixel value " + c);
        }
        return ct;
    }


    public CellType ColorToCellType(Color c) {
        for (int i = 0; i < colors.Length; i++) {
            if (colors[i] == c) {
                return (CellType)i;
            }
        }
        for (int i = 0; i < colors.Length; i++) {
            if ((colors[i].r - c.r) * (colors[i].r - c.r)
                + (colors[i].g - c.g) * (colors[i].g - c.g)
                + (colors[i].b - c.b) * (colors[i].b - c.b)
                + (colors[i].a - c.a) * (colors[i].a - c.a) < .1) {
                return (CellType)i;
            }

        }
        return (CellType)(-1);
    }

    public GameObject SpawnSprite(GameObject prefab, float x, float y, GameObject parent, float scale = 0f) {
        GameObject sprite = Instantiate(prefab, parent.transform) as GameObject;
        sprite.transform.position = new Vector2(x - 15, y - 20);
        if (scale != 0f) {
            sprite.transform.localScale = new Vector2(scale, scale);
        }
        return sprite;
    }

    private void OnDrawGizmos() {
        if (editorShowBoardInScene) {
            Gizmos.DrawGUITexture(Rect.MinMaxRect(-15, 20, 15, -20), boardDesignImage.GetComponent<SpriteRenderer>().sprite.texture);
        }
    }



}

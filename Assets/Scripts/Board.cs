using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Board : MonoBehaviour {

    public static readonly int minX = -15;
    public static readonly int maxX = 14;
    public static readonly int minY = -20;
    public static readonly int maxY = 19;

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
        OUT_OF_BOUNDS = 11,
    }

    public GameObject boardDesignImage;
    public GameObject wallBox;
    public GameObject[] cellPrefabs;
    public GameObject[] cellParents;


    public bool editorShowBoardInScene = true;

    public Color[] cellColorMap = {
        new Color(1,1,1,1),//white Dot
        new Color(0,0,1,1),//blue Wall
        new Color(1,1,0,1),//yellow Player
        new Color(0,1,0,1),//green Enemy
        new Color(1,0,1,1),//magenta Bonus
        new Color(0.5f,0.5f,0.5f,1),//gray Big Dot
        new Color(0,1,1,1),//cyan Enemy Entrance
        new Color(0.5f,0,0.5f,1),//purple Teleport
        new Color(0.25f,0,0.5f,1),//indigo Score Display
        new Color(0.75f, 0.25f, 0.25f, 1),//brown Remaining Life
        new Color(0,0,0,1),//black Empty
        new Color(1,0,0,1),//red Out Of Bounds
        new Color(1,0.5f,0,1),//orange (future expansion)
    };

    CellType[,] _cells = new CellType[maxX - minX + 1, maxY - minY + 1];

    public CellType this[int x, int y] {
        get {
            return _cells[x - minX, y - minY];
        }

        private set {
            _cells[x - minX, y - minY] = value;
        }
    }

    List<Vector3> enemyPositions = new List<Vector3>();
    Vector3 playerPosition;
    Vector3 bonusPosition;
    List<Vector3> enemyEntrances = new List<Vector3>();
    List<Vector3> teleports = new List<Vector3>();
    Texture2D boardDesignTexture;

    public GameObject GetCellPrefab(CellType t) {
        return cellPrefabs[(int)t];
    }

    public GameObject GetCellParent(CellType t) {
        return cellParents[(int)t];
    }


    public Vector3 GetPlayerPosition() {
        return playerPosition;
    }

    public Vector3 GetBonusPosition() {
        return bonusPosition;
    }

    public List<Vector3> GetEnemyPositions() {
        return enemyPositions;
    }
    public List<Vector3> GetEnemyEntrances() {
        return enemyEntrances;
    }
    public List<Vector3> GetTeleports() {
        return teleports;
    }
    public void setBoardDesignTexture(Texture2D t) {
        boardDesignTexture = t;
    }

    public void Build() {
        for (int x = minX; x <= maxX; x++) {
            for (int y = minY; y <= maxY; y++) {
                this[x, y] = ColorToCellType(getPixelColor(boardDesignTexture, x, y));
            }
        }
    }

    public void Show() {
        for (int x = minX; x <= maxX; x++) {
            for (int y = minY; y <= maxY; y++) {
                switch (this[x, y]) {
                    case CellType.DOT:
                    case CellType.BIG_DOT:
                    case CellType.WALL:
                    case CellType.ENEMY_ENTRANCE:
                        Show(x, y, this[x, y]);
                        break;
                    case CellType.PLAYER:
                        playerPosition = new Vector3(x, y, 0);
                        break;
                    case CellType.ENEMY:
                        enemyPositions.Add(new Vector3(x, y, 0));
                        break;
                    case CellType.BONUS_PICKUP:
                        bonusPosition = new Vector3(x, y, 0);
                        break;
                    case CellType.TELEPORT:
                        teleports.Add(new Vector3(x, y, 0));
                        break;
                    case CellType.SCORE_DISPLAY:
                        break;
                    case CellType.REMAINING_LIFE:
                        break;
                    case CellType.EMPTY:
                        break;
                    case CellType.OUT_OF_BOUNDS:
                        break;
                    default:
                        break;
                }
            }
        }
    }

    CellType ColorToCellType(Color c) {
        int bestIndex = 0;
        float bestDistance = 1000f;
        for (int i = 0; i < cellColorMap.Length; i++) {
            float distance = ColorDistance(cellColorMap[i], c);
            if (distance < bestDistance) {
                bestDistance = distance;
                bestIndex = i;
            }
        }
        return (CellType)bestIndex;
    }

    float ColorDistance(Color x, Color y) {
        float xh, xs, xv, yh, ys, yv;
        Color.RGBToHSV(x, out xh, out xs, out xv);
        Color.RGBToHSV(y, out yh, out ys, out yv);

        float h = xh - yh;
        if (h > 0.5f) {
            h -= 1;
        }
        if (h < 0) {
            h = -h;
        }

        float s = Mathf.Abs(xs - ys);

        float v = Mathf.Abs(xv - yv);

        return 10 * h + 5 * s + v;
    }

    Color getPixelColor(Texture2D t, int x, int y) {
        return t.GetPixel(x - minX, y - minY);
    }

    void Show(int x, int y, CellType t) {
        GameObject sprite = Instantiate(cellPrefabs[(int)t], cellParents[(int)t].transform) as GameObject;
        sprite.transform.position = new Vector2(x, y);
    }

    // Use this for initialization
    void Start() {
        boardDesignTexture = boardDesignImage.GetComponent<SpriteRenderer>().sprite.texture;
    }

    public void Clear() {
        for (int x = minX; x <= maxX; x++) {
            for (int y = minY; y <= maxY; y++) {
                this[x, y] = CellType.EMPTY;
            }
        }
        foreach(GameObject holder in cellParents) {
            foreach(Transform child in holder.transform) {
                Destroy(child.gameObject);
            }
        }
    }

    private void OnDrawGizmos() {
        if (editorShowBoardInScene) {
            Gizmos.DrawGUITexture(Rect.MinMaxRect(minX, maxY + 1, maxX + 1, minY), boardDesignImage.GetComponent<SpriteRenderer>().sprite.texture);
        }
    }



}

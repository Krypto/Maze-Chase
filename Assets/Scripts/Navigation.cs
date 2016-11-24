using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Navigation : MonoBehaviour {
    [Flags]
    public enum Directions {
        None = 0,
        UP = 1,
        DOWN = 2,
        LEFT = 4,
        RIGHT = 8,
        STAY = 16,
        ENEMY_UP = 32,
        ENEMY_DOWN = 64,
        ENEMY_LEFT = 128,
        ENEMY_RIGHT = 256,
        ENEMY_STAY = 512
    }

    public Directions this[int x, int y] {
        get {
            return _navigation[x - Board.minX, y - Board.minY];
        }

        private set {
            _navigation[x - Board.minX, y - Board.minY] = value;
        }
    }

    /*  
     * if i<matchingTeleports.Count/2, then i matches with i + matchingTeleports.Count/2
     */
    List<Vector3> matchingTeleports;

    Directions[,] _navigation = new Directions[Board.maxX - Board.minX + 1, Board.maxY - Board.minY + 1];

    void Build(Board board) {
        // see where actors can stay
        for (int x = Board.minX; x <= Board.maxX; x++) {
            for (int y = Board.minY; y <= Board.maxY; y++) {
                Directions flags = Directions.None;
                Board.CellType t = board[x, y];
                //can player stay here?
                if (t != Board.CellType.WALL && t != Board.CellType.ENEMY_ENTRANCE && t != Board.CellType.OUT_OF_BOUNDS) {
                    flags |= Directions.STAY;
                }
                //can enemy stay here?
                if (t != Board.CellType.WALL && t != Board.CellType.ENEMY_ENTRANCE && t != Board.CellType.OUT_OF_BOUNDS) {
                    flags |= Directions.ENEMY_STAY;
                }
                this[x, y] = flags;
            }
        }

        //see which way actors can move
        for (int x = Board.minX; x <= Board.maxX; x++) {
            for (int y = Board.minY; y <= Board.maxY; y++) {
                Directions flags = this[x, y];

                //can player go up?
                if (y > Board.minY && (this[x, y - 1] & Directions.STAY) == Directions.STAY) {
                    flags |= Directions.UP;
                }
                //can player go down?
                if (y < Board.maxY && (this[x, y + 1] & Directions.STAY) == Directions.STAY) {
                    flags |= Directions.DOWN;
                }
                //can player go left?
                if (x > Board.minX && (this[x - 1, y] & Directions.STAY) == Directions.STAY) {
                    flags |= Directions.LEFT;
                }
                //can player go right?
                if (x < Board.maxX && (this[x + 1, y] & Directions.STAY) == Directions.STAY) {
                    flags |= Directions.RIGHT;
                }

                //TODO deal with one-way enemy entrance walls

                //can enemy go up?
                if (y > Board.minY && (this[x, y - 1] & Directions.ENEMY_STAY) == Directions.ENEMY_STAY) {
                    flags |= Directions.ENEMY_UP;
                }
                //can enemy go down?
                if (y < Board.maxY && (this[x, y + 1] & Directions.ENEMY_STAY) == Directions.ENEMY_STAY) {
                    flags |= Directions.ENEMY_DOWN;
                }
                //can enemy go left?
                if (x > Board.minX && (this[x - 1, y] & Directions.ENEMY_STAY) == Directions.ENEMY_STAY) {
                    flags |= Directions.ENEMY_LEFT;
                }
                //can enemy go right?
                if (x < Board.maxX && (this[x + 1, y] & Directions.ENEMY_STAY) == Directions.ENEMY_STAY) {
                    flags |= Directions.ENEMY_RIGHT;
                }

                this[x, y] = flags;
            }
        }

        //find matching teleports
        matchingTeleports = new List<Vector3>(board.GetTeleports());
        matchingTeleports.Sort(angleCompare);
    }

    private int angleCompare(Vector3 a, Vector3 b) {
        float d = Mathf.Atan2(b.y, b.x) - Mathf.Atan2(a.y, a.x);
        if (d > 0) {
            return 1;
        } else if (d < 0) {
            return -1;
        } else {
            return 0;
        }
    }
}

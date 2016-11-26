using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;



public class Navigation : MonoBehaviour {
    public GameObject waypointGridPrefab;

    public struct Pair {
        public int x;
        public int y;

        public Pair(int x, int y) {
            this.x = x;
            this.y = y;
        }
    }

    [Flags]
    public enum Directions {
        None = 0,
        UP = 1,
        DOWN = 2,
        LEFT = 4,
        RIGHT = 8,
        STAY = 16,
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
     * Teleport no. i matches with (i +   floor(matchingTeleports.Count/2)) % matchingTeleports.Count
     * 
     * Will behave funny if there are an odd number of teleports (if a goes to b, b does not 
     * go back to a; a star-shaped pattern)
     */
    List<Vector3> matchingTeleports;

    Directions[,] _navigation = new Directions[Board.maxX - Board.minX + 1, Board.maxY - Board.minY + 1];

    public void FindMatchingTeleport(int x, int y, out int newX, out int newY) {
        for (int i = 0; i < matchingTeleports.Count; i++) {
            Vector3 p = matchingTeleports[i];
            if (Mathf.Round(p.x) == x && Mathf.Round(p.y) == y) {
                p = matchingTeleports[(i + (int)Mathf.Floor(matchingTeleports.Count / 2)) % matchingTeleports.Count];
                newX = (int)Mathf.Round(p.x);
                newY = (int)Mathf.Round(p.y);
                return;
            }
        }
        Debug.LogError("Matching teleport not found!");
        newX = 0;
        newY = 0;
    }

    public void Build(Board board) {
        Debug.Log("Building board");
        // see where actors can stay
        for (int x = Board.minX; x <= Board.maxX; x++) {
            for (int y = Board.minY; y <= Board.maxY; y++) {
                Directions flags = Directions.None;
                Board.CellType t = board[x, y];
                //can player stay here?
                if (t != Board.CellType.WALL && t != Board.CellType.ENTRANCE && t != Board.CellType.OUT_OF_BOUNDS) {
                    flags |= Directions.STAY;
                }
                //can enemy stay here?
                if (t != Board.CellType.WALL && t != Board.CellType.ENTRANCE && t != Board.CellType.OUT_OF_BOUNDS) {
                    flags |= Directions.STAY;
                }
                this[x, y] = flags;
            }
        }

        //see which way actors can move
        for (int x = Board.minX; x <= Board.maxX; x++) {
            for (int y = Board.minY; y <= Board.maxY; y++) {
                Directions flags = this[x, y];
                Board.CellType t = board[x, y];
                if (t == Board.CellType.WALL) {
                    continue;
                }

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

                //can enemy go up?
                if (y > Board.minY && (this[x, y - 1] & Directions.STAY) == Directions.STAY) {
                    flags |= Directions.UP;
                }
                //can enemy go down?
                if (y < Board.maxY && (this[x, y + 1] & Directions.STAY) == Directions.STAY) {
                    flags |= Directions.DOWN;
                }
                //can enemy go left?
                if (x > Board.minX && (this[x - 1, y] & Directions.STAY) == Directions.STAY) {
                    flags |= Directions.LEFT;
                }
                //can enemy go right?
                if (x < Board.maxX && (this[x + 1, y] & Directions.STAY) == Directions.STAY) {
                    flags |= Directions.RIGHT;
                }

                this[x, y] = flags;
            }
        }

        //Deal with one-way enemy entrance walls
        //breadth-first search algorithm

        //all cells reachible from enemies go here
        HashSet<Pair> reachible = new HashSet<Pair>();

        //start with enemy initial positions
        foreach (Vector3 ep in board.GetEnemyPositions()) {
            Pair p = new Pair();
            p.x = (int)Mathf.Round(ep.x);
            p.y = (int)Mathf.Round(ep.y);
            reachible.Add(p);
        }

        //new cells to add to reachible region
        Queue<Pair> boundary = new Queue<Pair>(reachible);

        while (boundary.Count > 0) {//while there are new cells to check
            Pair p = boundary.Dequeue();//get a cell
            Directions d = this[p.x, p.y];

            //add adjacent cells that are reachible from the chosen cell
            //add it to the reachible set, and if it hasn't already been
            //checked, add it to the boundary as well
            if ((d & Directions.UP) != 0) {
                Pair q = new Pair();
                q.x = p.x;
                q.y = p.y - 1;
                if (reachible.Add(q)) {
                    boundary.Enqueue(q);
                }
            }
            if ((d & Directions.DOWN) != 0) {
                Pair q = new Pair();
                q.x = p.x;
                q.y = p.y + 1;
                if (reachible.Add(q)) {
                    boundary.Enqueue(q);
                }
            }
            if ((d & Directions.LEFT) != 0) {
                Pair q = new Pair();
                q.x = p.x - 1;
                q.y = p.y;
                if (reachible.Add(q)) {
                    boundary.Enqueue(q);
                }
            }
            if ((d & Directions.RIGHT) != 0) {
                Pair q = new Pair();
                q.x = p.x + 1;
                q.y = p.y;
                if (reachible.Add(q)) {
                    boundary.Enqueue(q);
                }
            }

        }

        //now, see which reachible cells touch the enemy entrance, and set 
        //the enemy entrance to be reachible from them
        //as well as the next cell continuing in the same direction. 
        foreach (Pair p in reachible) {
            if (board[p.x, p.y - 1] == Board.CellType.ENTRANCE) {
                this[p.x, p.y] = Directions.UP;
                this[p.x, p.y - 1] = Directions.UP;
            } else if (board[p.x, p.y + 1] == Board.CellType.ENTRANCE) {
                this[p.x, p.y] = Directions.DOWN;
                this[p.x, p.y + 1] = Directions.DOWN;
            } else if (board[p.x - 1, p.y] == Board.CellType.ENTRANCE) {
                this[p.x, p.y] = Directions.LEFT;
                this[p.x - 1, p.y] = Directions.LEFT;
            } else if (board[p.x + 1, p.y] == Board.CellType.ENTRANCE) {
                this[p.x, p.y] = Directions.RIGHT;
                this[p.x + 1, p.y] = Directions.RIGHT;
            }
        }


        //find matching teleports
        // sort them by angle, pair them by having opposite angle
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

    public Pair DirectionToWaypoint(WaypointGrid grid, int startX, int startY) {
        int x = startX;
        int y = startY;
        Pair bestDirection = new Pair(0, 0);
        int bestDistance = grid[x, y];//it's possible best bet is to stay

        //first try up
        Pair direction = new Pair(0, -1);
        int d = grid[x + direction.x, y + direction.y];
        if (d <= bestDistance) {
            bestDistance = d;
            bestDirection = direction;
        }

        //down
        direction = new Pair(0, 1);
        d = grid[x + direction.x, y + direction.y];
        if (d <= bestDistance) {
            bestDistance = d;
            bestDirection = direction;
        }

        //left
        direction = new Pair(-1, 0);
        d = grid[x + direction.x, y + direction.y];
        if (d <= bestDistance) {
            bestDistance = d;
            bestDirection = direction;
        }

        //right
        direction = new Pair(1, 0);
        d = grid[x + direction.x, y + direction.y];
        if (d <= bestDistance) {
            bestDistance = d;
            bestDirection = direction;
        }

        if (bestDistance > 1000) {
            return new Pair(0, 0);
        }

        return bestDirection;
    }

}

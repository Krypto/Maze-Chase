using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointGrid : MonoBehaviour {

    struct Pair {
        public int x;
        public int y;

        public Pair(int x, int y) {
            this.x = x;
            this.y = y;
        }
    }

    public float waypointX, waypointY;


    Navigation navigation;

    float currentX, currentY;

    int[,] _grid = new int[Board.maxX - Board.minX + 1, Board.maxY - Board.minY + 1];

    public int this[int x, int y] {
        get {
            return _grid[x - Board.minX, y - Board.minY];
        }

        private set {
            _grid[x - Board.minX, y - Board.minY] = value;
        }
    }

    // Use this for initialization
    void Start() {
        navigation = FindObjectOfType<Navigation>();
        currentX = waypointX + 1;//force rebuild
    }

    // Update is called once per frame
    void Update() {
        if (waypointX != currentX || waypointY != currentY) {

            currentX = waypointX;
            currentY = waypointY;
            for (int i = Board.minX; i <= Board.maxX; i++) {
                for (int j = Board.minY; j <= Board.maxY; j++) {
                    this[i, j] = int.MaxValue;
                }
            }
            Pair t = new Pair((int)Mathf.Round(currentX), (int)Mathf.Round(currentY));

            this[t.x, t.y] = 0;
            HashSet<Pair> found = new HashSet<Pair>();
            Queue<Pair> boundary = new Queue<Pair>();
            found.Add(t);
            boundary.Enqueue(t);

            while (boundary.Count > 0) {
                t = boundary.Dequeue();
                if ((navigation[t.x, t.y] & Navigation.Directions.UP) != 0) {
                    Pair q = new Pair(t.x, t.y - 1);
                    if (!found.Contains(q)) {
                        boundary.Enqueue(q);
                        found.Add(q);
                        this[q.x, q.y] = this[t.x, t.y] + 1;
                    }
                }
                if ((navigation[t.x, t.y] & Navigation.Directions.DOWN) != 0) {
                    Pair q = new Pair(t.x, t.y + 1);
                    if (!found.Contains(q)) {
                        boundary.Enqueue(q);
                        found.Add(q);
                        this[q.x, q.y] = this[t.x, t.y] + 1;
                    }
                }
                if ((navigation[t.x, t.y] & Navigation.Directions.LEFT) != 0) {
                    Pair q = new Pair(t.x - 1, t.y);
                    if (!found.Contains(q)) {
                        boundary.Enqueue(q);
                        found.Add(q);
                        this[q.x, q.y] = this[t.x, t.y] + 1;
                    }
                }
                if ((navigation[t.x, t.y] & Navigation.Directions.RIGHT) != 0) {
                    Pair q = new Pair(t.x + 1, t.y);
                    if (!found.Contains(q)) {
                        boundary.Enqueue(q);
                        found.Add(q);
                        this[q.x, q.y] = this[t.x, t.y] + 1;
                    }
                }

            }
        }
    }
}
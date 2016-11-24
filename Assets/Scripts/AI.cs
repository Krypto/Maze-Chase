using UnityEngine;

public interface AI {
    Vector2 GetDirection();
    void SetEnemy(Enemy enemy);
}
using UnityEngine;

public class SnakeAI : EnemyAI
{
    // Snake constantly hunts the player with no pause
    protected override void Update()
    {
        base.Update(); // Uses the chase behavior from EnemyAI
    }
}
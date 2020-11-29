using UnityEngine;

public class AsteroidSmall : Asteroid
{
    protected override void OnEnemyCreated()
    {
        var normal = GameController.Player.transform.position - Transform.position;
        MovementDirection = Vector2.Reflect(-MovementDirection, normal).normalized;
    }
    
    protected override void OnEnemyKilled()
    {
        EnemiesFactory.RegisterSmallAsteroidDestruction();
    }
}
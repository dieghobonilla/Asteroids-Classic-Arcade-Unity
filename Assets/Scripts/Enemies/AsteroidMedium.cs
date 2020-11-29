using UnityEngine;

public class AsteroidMedium : Asteroid
{
    protected override void OnEnemyCreated()
    {
        var normal = GameController.Player.transform.position - Transform.position;
        MovementDirection = Vector2.Reflect(MovementDirection, normal).normalized;
    }
    
    protected override void OnEnemyKilled()
    {
        EnemiesFactory.SpawnAsteroids(Transform.position, EnemiesFactory.AsteroidsSize.Small);
    }
}
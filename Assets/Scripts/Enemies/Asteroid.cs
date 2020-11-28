using System.Collections.Generic;
using UnityEngine;

public class Asteroid : Enemy
{
    public AsteroidType AsteroidSize;
    
    public enum AsteroidType
    {
        Big,
        Medium,
        Small
    }
    
    public override void Setup(Vector2 movementDirection, Vector3 position, Queue<Enemy> enemiesQueue)
    {
        if (AsteroidSize != AsteroidType.Big)
        {
            var normal = GameController.Player.transform.position - Transform.position;
            movementDirection = Vector2.Reflect(movementDirection, normal).normalized;
        }
        
        base.Setup(movementDirection, position, enemiesQueue);
    }
    
    protected override void Update()
    {
        base.Update();

        if (AsteroidSize == AsteroidType.Big)
        {
            Transform.Rotate(Vector3.forward, 0.30f);
        }
    }
    
    public override void OnCollisionDetected(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            other.GetComponent<BulletPrefab>().BulletHit();
            FireEventOnEnemyDestroyed();
        }

        if (other.CompareTag("UFO"))
        {
            FireEventOnEnemyDestroyed();
        }
    }
}
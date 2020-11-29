using UnityEngine;

public abstract class Asteroid : Enemy
{
    protected override void OnCollisionDetected(Collider2D other)
    {
        OnAsteroidCollision(other);
    }

    protected virtual void OnAsteroidCollision(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            other.GetComponent<BulletPrefab>().BulletHit();
            OnObjectKilled();
            FireEventOnEnemyDestroyed();
        }

        if (other.CompareTag("UFO"))
        {
            OnObjectKilled();
        }
    }
}
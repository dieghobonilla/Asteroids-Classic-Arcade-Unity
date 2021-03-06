using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class UFO : Enemy
{
    public static event Action<float, Vector3, Vector3> OnFire = delegate { };

    private Coroutine _fireRandomBullets;

    private void OnEnable()
    {
        //FireRandomBullets();
    }

    private void FireRandomBullets()
    {
        if (_fireRandomBullets != null)
        {
            StopCoroutine(_fireRandomBullets);
        }

        _fireRandomBullets = StartCoroutine(FireRandomBulletsCoroutine());

        IEnumerator FireRandomBulletsCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.25f); //GetRandomNumber());
                OnFire?.Invoke(Rigidbody.velocity.magnitude, Transform.position, GetRandomDirection());
            }
        }

        int GetRandomNumber()
        {
            return Random.Range(3, 10);
        }

        Vector2 GetRandomDirection()
        {
            var direction = Vector2.one;
            direction.x = GetRandomNumber();
            direction.x = GetRandomNumber();

            return direction.normalized;
        }
    }

    protected override void OnCollisionDetected(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            other.GetComponent<BulletPrefab>().BulletHit();
            OnObjectKilled();
            FireEventOnEnemyDestroyed();
        }

        if (other.CompareTag("Enemy") || other.CompareTag("UFO"))
        {
            OnObjectKilled();
        }
    }

    protected override void OnEnemyKilled()
    {
    }

    protected override void OnEnemyCreated()
    {
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : GameEntity
{
    public static event Action<Enemy, Queue<Enemy>> OnEnemyDestroyed = delegate(Enemy enemy, Queue<Enemy> queue) { };

    public int KillPoints = 100;
    
    protected Queue<Enemy> Queue;
    protected Vector2 MovementDirection;

    public abstract void OnCollisionDetected(Collider2D other);

    public virtual void Setup(Vector2 movementDirection, Vector3 position, Queue<Enemy> enemiesQueue)
    {
        Transform.position = position;
        MovementDirection = movementDirection;
        Queue = enemiesQueue;
    }

    private void FixedUpdate()
    {
        Rigidbody.velocity = MovementDirection * MovementSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnCollisionDetected(other);
    }

    protected void FireEventOnEnemyDestroyed()
    {
        OnEnemyDestroyed?.Invoke(this, Queue);
    }
}
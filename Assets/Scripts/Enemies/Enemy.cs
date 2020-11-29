using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : GameEntity, IKillable
{
    //public static event Action<Enemy, Queue<Enemy>> OnEnemyDestroyed = delegate { };
    public static event Action<Enemy> OnEnemyDestroyed = delegate { };

    public int KillPoints = 100;
    
    protected Queue<Enemy> EnemyQueue;
    protected EnemiesFactory EnemiesFactory;
    
    private Vector2 MovementDirection;
    private bool _needsRestarting;

    private void OnEnable()
    {
        GameController.OnGameStarted += GameStarted;
        GameController.OnGameOver += GameOver;
    }

    private void OnDisable()
    {
        GameController.OnGameStarted -= GameStarted;
        GameController.OnGameOver -= GameOver;
    }

    private void GameStarted()
    {
        if (!_needsRestarting)
        {
            return;
        }

        _needsRestarting = false;
        EnemiesFactory.QueueEnemy(EnemyQueue, this);
    }

    private void GameOver()
    {
        _needsRestarting = true;
    }

    protected abstract void OnCollisionDetected(Collider2D other);

    public virtual void Setup(Vector2 movementDirection, Vector3 position, Queue<Enemy> enemiesQueue, EnemiesFactory enemiesFactory)
    {
        Transform.position = position;
        MovementDirection = movementDirection;
        EnemyQueue = enemiesQueue;
        EnemiesFactory = enemiesFactory;
    }

    protected virtual void FixedUpdate()
    {
        Rigidbody.velocity = MovementDirection * MovementSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnCollisionDetected(other);
    }

    protected void FireEventOnEnemyDestroyed()
    {
        OnEnemyDestroyed?.Invoke(this);
    }

    public void OnObjectKilled()
    {
        EnemiesFactory.QueueEnemy(EnemyQueue, this);
        EnemiesFactory.OnEnemyDestroyed(this);
    }
}
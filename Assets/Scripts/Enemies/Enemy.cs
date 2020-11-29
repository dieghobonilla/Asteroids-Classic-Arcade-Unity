using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : GameEntity, IKillable
{
    public static event Action<Enemy> OnEnemyDestroyed = delegate { };

    public int KillPoints = 100;
    
    protected EnemiesFactory EnemiesFactory;
    protected Vector2 MovementDirection;
    
    private Queue<Enemy> _enemyQueue;
    private bool _isActive;

    private void OnEnable()
    {
        _isActive = false;
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
        
    }

    private void GameOver()
    {
        if (_isActive)
        {
            EnemiesFactory.QueueEnemy(_enemyQueue, this);
        }
    }

    protected abstract void OnCollisionDetected(Collider2D other);
    protected abstract void OnEnemyKilled();
    protected abstract void OnEnemyCreated();

    public void Setup(Vector2 movementDirection, Vector3 position, Queue<Enemy> enemiesQueue, EnemiesFactory enemiesFactory)
    {
        Transform.position = position;
        MovementDirection = movementDirection;
        _enemyQueue = enemiesQueue;
        EnemiesFactory = enemiesFactory;
        _isActive = true;
        OnEnemyCreated();
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
        _isActive = false;
        EnemiesFactory.QueueEnemy(_enemyQueue, this);
        OnEnemyKilled();
    }
}
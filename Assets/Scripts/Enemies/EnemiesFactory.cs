using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemiesFactory : MonoBehaviour
{
    public event Action OnAllEnemiesInTheLevelDestroyed = delegate { }; 
    
    #region Serialized Variables

    [SerializeField] private Enemy _asteroidBig;
    [SerializeField] private Enemy _asteroidMedium;
    [SerializeField] private Enemy _asteroidSmall;
    [SerializeField] private Enemy _ufoBig;
    [SerializeField] private Enemy _ufoSmall;

    #endregion

    #region Private Variables

    private bool _restartingGame;
    private int _startingAsteroidsCount;
    private int _smallAsteroidsDestroyedCount;
    private int _maxSmallAsteroidsLevelCount;
    private readonly Queue<Enemy> _queueAsteroidsBig = new Queue<Enemy>();
    private readonly Queue<Enemy> _queueAsteroidsMedium = new Queue<Enemy>();
    private readonly Queue<Enemy> _queueAsteroidsSmall = new Queue<Enemy>();
    private readonly Queue<Enemy> _queueUFOBig = new Queue<Enemy>();
    private readonly Queue<Enemy> _queueUFOSmall = new Queue<Enemy>();

    #endregion

    #region Public Methods

    public void LoadNewLevel(int level, int startingAsteroids)
    {
        _startingAsteroidsCount = startingAsteroids;
        _smallAsteroidsDestroyedCount = 0;
        CreateAllEnemies();

        for (var i = 0; i < _startingAsteroidsCount; i++)
        {
            GetEnemy(_queueAsteroidsBig).Setup(GetRandomDirection(), GetAsteroidRandomInitialPosition(), _queueAsteroidsBig, this);
        }
    }

    public void SpawnBigUFO(int quantity = 1)
    {
        for (var i = 0; i < quantity; i++)
        {
            GetEnemy(_queueUFOBig).Setup(GetRandomDirection(), GetUFORandomInitialPosition(), _queueUFOBig, this);
        }
    }

    public void SpawnSmallUFO(int quantity = 1)
    {
        for (var i = 0; i < quantity; i++)
        {
            GetEnemy(_queueUFOSmall).Setup(GetRandomDirection(), GetUFORandomInitialPosition(), _queueUFOSmall, this);
        }
    }

    public void QueueEnemy(Queue<Enemy> queue, Enemy enemy)
    {
        // enemy.gameObject.SetActive(false);
        // enemy.Rigidbody.velocity = Vector2.zero;
        enemy.SetActive(false);
        queue.Enqueue(enemy);
    }

    public void OnEnemyDestroyed(Enemy enemy)
    {
        Queue<Enemy> newQueue = null;

        switch (enemy)
        {
            case Asteroid asteroid when asteroid.AsteroidSize == Asteroid.AsteroidType.Big:
                newQueue = _queueAsteroidsMedium;

                break;

            case Asteroid asteroid when asteroid.AsteroidSize == Asteroid.AsteroidType.Medium:
                newQueue = _queueAsteroidsSmall;

                break;
            
            case Asteroid asteroid when asteroid.AsteroidSize == Asteroid.AsteroidType.Small:
                _smallAsteroidsDestroyedCount++;
                
                if (_smallAsteroidsDestroyedCount >= _maxSmallAsteroidsLevelCount)
                {
                    OnAllEnemiesInTheLevelDestroyed?.Invoke();
                }
                break;
        }

        if (newQueue != null)
        {
            for (var i = 0; i < 2; i++)
            {
                GetEnemy(newQueue).Setup(GetRandomDirection(), enemy.transform.position, newQueue, this);
            }
        }
    }

    #endregion

    #region Private Methods

    private void CreateAllEnemies()
    {
        // big asteroids
        InitializeQueue(_queueAsteroidsBig, _asteroidBig, _startingAsteroidsCount);

        // medium asteroids 2 times more than big asteroids 
        InitializeQueue(_queueAsteroidsMedium, _asteroidMedium, _startingAsteroidsCount * 2);

        // small asteroids 4 times more than big asteroids
        _maxSmallAsteroidsLevelCount = _startingAsteroidsCount * 4;
        InitializeQueue(_queueAsteroidsSmall, _asteroidSmall, _maxSmallAsteroidsLevelCount);

        // big UFO half of big asteroids
        InitializeQueue(_queueUFOBig, _ufoBig, _startingAsteroidsCount / 2);

        // small UFO
        InitializeQueue(_queueUFOSmall, _ufoSmall, _startingAsteroidsCount);
    }

    private void InitializeQueue(Queue<Enemy> queue, Enemy prefab, int maxCount)
    {
        var newCount = maxCount - queue.Count;

        for (var i = 0; i < newCount; i++)
        {
            var enemy = Instantiate(prefab);
            QueueEnemy(queue, enemy);
        }
    }

    private static Enemy GetEnemy(Queue<Enemy> queue)
    {
        if (queue.Count == 0) { }

        var enemy = queue.Dequeue();

        //enemy.gameObject.SetActive(true);
        enemy.SetActive(true);

        return enemy;
    }

    private static Vector2 GetAsteroidRandomInitialPosition()
    {
        var randomPositionX = Random.Range(-GameSettings.ScreenLimits.x, GameSettings.ScreenLimits.x);

        return new Vector2(randomPositionX, GameSettings.ScreenLimits.y);
    }

    private static Vector2 GetUFORandomInitialPosition()
    {
        var randomLeftRight = Random.Range(0, 1);
        var randomPositionX = randomLeftRight == 0 ? -GameSettings.ScreenLimits.x : GameSettings.ScreenLimits.x;
        var randomPositionY = Random.Range(-GameSettings.ScreenLimits.y, GameSettings.ScreenLimits.y);

        return new Vector2(randomPositionX, randomPositionY);
    }

    private static Vector2 GetRandomDirection()
    {
        var randomDirectionX = Random.Range(-GameSettings.ScreenLimits.x, GameSettings.ScreenLimits.x);

        return new Vector2(randomDirectionX, -1f).normalized;
    }

    #endregion
}
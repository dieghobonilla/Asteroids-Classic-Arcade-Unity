using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemiesFactory : MonoBehaviour
{
    public event Action OnAllEnemiesInTheLevelDestroyed = delegate { };

    public enum AsteroidsSize
    {
        Big,
        Medium,
        Small
    }

    public enum UFOSize
    {
        Big,
        Small
    }

    #region Serialized Variables

    [SerializeField] private Enemy _asteroidBig;
    [SerializeField] private Enemy _asteroidMedium;
    [SerializeField] private Enemy _asteroidSmall;
    [SerializeField] private Enemy _ufoBig;
    [SerializeField] private Enemy _ufoSmall;

    #endregion

    #region Private Variables
    
    private delegate Vector2 GetRandomPositionDelegate();

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

    public void LoadNewLevel(int level, int enemiesCount)
    {
        //Debug.LogWarning($"LoadNewLevel level {level} enemiesCount {enemiesCount}");
        _smallAsteroidsDestroyedCount = 0;

        CreateAllEnemies(enemiesCount);
        SpawnEnemies(GetAsteroidRandomInitialPosition, enemiesCount, _queueAsteroidsBig);
    }

    public void SpawnAsteroids(Vector2 position, AsteroidsSize asteroidsSize, int quantity = 2)
    {
        Queue<Enemy> queue = null;

        switch (asteroidsSize)
        {
            case AsteroidsSize.Big:
                queue = _queueAsteroidsBig;

                break;
            case AsteroidsSize.Medium:
                queue = _queueAsteroidsMedium;

                break;
            case AsteroidsSize.Small:
                queue = _queueAsteroidsSmall;

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(asteroidsSize), asteroidsSize, null);
        }

        if (queue != null)
        {
            SpawnEnemies(position, quantity, queue);
        }
    }

    public void SpawnUFOs(UFOSize ufoSize, int quantity = 1)
    {
        Queue<Enemy> queue = null;

        switch (ufoSize)
        {
            case UFOSize.Big:
                queue = _queueUFOBig;

                break;
            case UFOSize.Small:
                queue = _queueUFOSmall;

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(ufoSize), ufoSize, null);
        }

        if (queue != null)
        {
            SpawnEnemies(GetUFORandomInitialPosition, quantity, queue);
        }
    }

    private void SpawnEnemies(GetRandomPositionDelegate randomPositionDelegate, int quantity, Queue<Enemy> queue)
    {
        SpawnEnemies(randomPositionDelegate.Invoke(), quantity, queue);
    }
    
    private void SpawnEnemies(Vector2 position, int quantity, Queue<Enemy> queue)
    {
        for (var i = 0; i < quantity; i++)
        {
            GetEnemy(queue).Setup(GetRandomDirection(), position, queue, this);
        }
    }

    public void RegisterSmallAsteroidDestruction()
    {
        _smallAsteroidsDestroyedCount++;

        if (_smallAsteroidsDestroyedCount >= _maxSmallAsteroidsLevelCount)
        {
            OnAllEnemiesInTheLevelDestroyed?.Invoke();
        }
    }
    
    public void QueueEnemy(Queue<Enemy> queue, Enemy enemy)
    {
        enemy.SetActive(false);
        queue.Enqueue(enemy);
    }

    #endregion

    #region Private Methods

    private void CreateAllEnemies(int enemyCount)
    {
        // big asteroids
        InitializeQueue(_queueAsteroidsBig, _asteroidBig, enemyCount);

        // medium asteroids 2 times more than big asteroids 
        InitializeQueue(_queueAsteroidsMedium, _asteroidMedium, enemyCount * 2);

        // small asteroids 4 times more than big asteroids
        _maxSmallAsteroidsLevelCount = enemyCount * 4;
        InitializeQueue(_queueAsteroidsSmall, _asteroidSmall, _maxSmallAsteroidsLevelCount);

        // big UFO
        InitializeQueue(_queueUFOBig, _ufoBig, enemyCount);

        // small UFO
        InitializeQueue(_queueUFOSmall, _ufoSmall, enemyCount);
    }

    private void InitializeQueue(Queue<Enemy> queue, Enemy prefab, int maxCount)
    {
        if (queue.Count >= maxCount)
        {
            return;
        }
        
        var newCount = maxCount - queue.Count;
        // Debug.Log($"InitializeQueue newCount {newCount} prefab {prefab.name}, max {maxCount} queue.Count {queue.Count}");

        for (var i = 0; i < newCount; i++)
        {
            var enemy = Instantiate(prefab);
            QueueEnemy(queue, enemy);
        }
    }

    private static Enemy GetEnemy(Queue<Enemy> queue)
    {
        if (queue.Count <= 0)
        {
            Debug.LogError($"queue is empty");
        }

        var enemy = queue.Dequeue();
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
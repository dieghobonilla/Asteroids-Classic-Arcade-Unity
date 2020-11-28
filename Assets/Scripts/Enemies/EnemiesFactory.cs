using System.Collections.Generic;
using UnityEngine;

public class EnemiesFactory : MonoBehaviour
{
    [SerializeField] private int _startingAsteroidsCount = 5;

    [SerializeField] private Enemy _asteroidBig;
    [SerializeField] private Enemy _asteroidMedium;
    [SerializeField] private Enemy _asteroidSmall;
    [SerializeField] private Enemy _ufoBig;
    [SerializeField] private Enemy _ufoSmall;

    private readonly Queue<Enemy> _queueAsteroidsBig = new Queue<Enemy>();
    private readonly Queue<Enemy> _queueAsteroidsMedium = new Queue<Enemy>();
    private readonly Queue<Enemy> _queueAsteroidsSmall = new Queue<Enemy>();
    private readonly Queue<Enemy> _queueUFOBig = new Queue<Enemy>();
    private readonly Queue<Enemy> _queueUFOSmall = new Queue<Enemy>();

    private void OnEnable()
    {
        Enemy.OnEnemyDestroyed += EnemyDestroyed;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyDestroyed -= EnemyDestroyed;
    }

    private void EnemyDestroyed(Enemy enemy, Queue<Enemy> queue)
    {
        AddEnemy(queue, enemy);

        Queue<Enemy> newQueue = null;

        switch (enemy)
        {
            case Asteroid asteroid when asteroid.AsteroidSize == Asteroid.AsteroidType.Big:
                newQueue = _queueAsteroidsMedium;
                break;
            
            case Asteroid asteroid when asteroid.AsteroidSize == Asteroid.AsteroidType.Medium:
                newQueue = _queueAsteroidsSmall;
                break;
        }

        if (newQueue != null)
        {
            for (var i = 0; i < 2; i++)
            {
                GetEnemy(newQueue).Setup(GetRandomDirection(), enemy.transform.position, newQueue);
            }
        }
    }

    private void Start()
    {
        CreateAllEnemies();

        //LoadNewLevel();
    }

    private void CreateAllEnemies()
    {
        // big asteroids
        CreateEnemiesInQueue(_queueAsteroidsBig, _asteroidBig, _startingAsteroidsCount);

        // medium asteroids 2 times more than big asteroids 
        CreateEnemiesInQueue(_queueAsteroidsMedium, _asteroidMedium, _startingAsteroidsCount * 2);

        // small asteroids 4 times more than big asteroids
        CreateEnemiesInQueue(_queueAsteroidsSmall, _asteroidSmall, _startingAsteroidsCount * 4);

        // big UFO half of big asteroids
        CreateEnemiesInQueue(_queueUFOBig, _ufoBig, _startingAsteroidsCount / 2);

        // small UFO
        CreateEnemiesInQueue(_queueUFOSmall, _ufoSmall, _startingAsteroidsCount);
    }

    private void CreateEnemiesInQueue(Queue<Enemy> queue, Enemy prefab, int maxCount)
    {
        for (var i = 0; i < maxCount; i++)
        {
            var enemy = Instantiate(prefab);
            AddEnemy(queue, enemy);
        }
    }

    private void AddEnemy(Queue<Enemy> queue, Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
        enemy.Rigidbody.velocity = Vector2.zero;
        queue.Enqueue(enemy);
    }

    private Enemy GetEnemy(Queue<Enemy> queue)
    {
        var enemy = queue.Dequeue();
        enemy.gameObject.SetActive(true);

        return enemy;
    }

    public void LoadNewLevel(int level = 1)
    {
        for (var i = 0; i < _startingAsteroidsCount; i++)
        {
            GetEnemy(_queueAsteroidsBig).Setup(GetRandomDirection(), GetRandomPosition(), _queueAsteroidsBig);
        }
    }

    public void SpawnBigUFO(int quantity)
    {
        for (var i = 0; i < quantity; i++)
        {
            GetEnemy(_queueUFOBig).Setup(GetRandomDirection(), GetRandomPosition(), _queueUFOBig);
        }
    }

    public void SpawnSmallUFO(int quantity)
    {
        for (var i = 0; i < quantity; i++)
        {
            GetEnemy(_queueUFOSmall).Setup(GetRandomDirection(), GetRandomPosition(), _queueUFOSmall);
        }
    }

    private static Vector2 GetRandomPosition()
    {
        var randomPositionX = Random.Range(-GameSettings.ScreenLimits.x, GameSettings.ScreenLimits.x);

        return new Vector2(randomPositionX, GameSettings.ScreenLimits.y);
    }

    private static Vector2 GetRandomDirection()
    {
        var randomDirectionX = Random.Range(-GameSettings.ScreenLimits.x, GameSettings.ScreenLimits.x);

        return new Vector2(randomDirectionX, -1f).normalized;
    }
}
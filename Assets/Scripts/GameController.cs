using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class GameController : MonoBehaviour
{
    public static event Action<int> OnPlayerDestroyed = delegate { };
    public static event Action<int> OnLevelCleared = delegate { };
    public static event Action OnGameStarted = delegate { };
    public static event Action OnGameOver = delegate { };

    [SerializeField] private int _initialPlayerShips = 3;

    [SerializeField] private Player _player;
    [SerializeField] private EnemiesFactory _enemiesFactory;

    // Game settings
    public static Camera MainCamera { get; private set; }
    public static Player Player { get; private set; }

    public static int PlayerShips;
    private Coroutine _spawnUFOsCoroutine;

    private void Awake()
    {
        MainCamera = Camera.main;
        Assert.IsNotNull(MainCamera);

        GameSettings.SetScreenLimits(MainCamera);
    }

    private void OnEnable()
    {
        Player.OnPlayerDestroyed += PlayerDestroyed;
    }

    private void OnDisable()
    {
        Player.OnPlayerDestroyed -= PlayerDestroyed;
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        Player = _player;
        PlayerShips = _initialPlayerShips;
        LoadNewLevel();

        OnGameStarted?.Invoke();
    }

    private void PlayerDestroyed()
    {
        PlayerShips--;
        OnPlayerDestroyed?.Invoke(PlayerShips);

        if (PlayerShips <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        StopCoroutine(_spawnUFOsCoroutine);
       
        OnGameOver?.Invoke();
    }

    private void LoadNewLevel(int level = 1)
    {
        _enemiesFactory.LoadNewLevel(level);

        if (_spawnUFOsCoroutine != null)
        {
            StopCoroutine(_spawnUFOsCoroutine);
        }
        
        _spawnUFOsCoroutine = StartCoroutine(SpawnUFOsCoroutine());
       
        IEnumerator SpawnUFOsCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(3);
                _enemiesFactory.SpawnBigUFO(1);

                yield return new WaitForSeconds(3);
                _enemiesFactory.SpawnSmallUFO(1);

                yield return new WaitForSeconds(5);
            }
        }
    }
}
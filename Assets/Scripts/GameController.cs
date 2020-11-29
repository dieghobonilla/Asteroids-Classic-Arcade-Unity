using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class GameController : MonoBehaviour
{
    #region Game Events

    public static event Action<int> OnPlayerDestroyed = delegate { };
    public static event Action<int> OnLevelCleared = delegate { };
    public static event Action OnGameStarted = delegate { };
    public static event Action OnGameOver = delegate { };

    #endregion

    #region Serialized Variables

    [SerializeField] private int _startingPlayerShips = 3;
    [SerializeField] private int _startingAsteroids = 5;
    [SerializeField] private Player _player;
    [SerializeField] private EnemiesFactory _enemiesFactory;

    #endregion

    #region Static Properties

    public static Camera MainCamera { get; private set; }
    public static Player Player { get; private set; }
    public static int StartingAsteroidsCount { get; private set; }

    public static int PlayerShips;

    #endregion

    #region Private Variables

    private Coroutine _spawnUFOsCoroutine;
    private int _currentLevel;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        MainCamera = Camera.main;
        Assert.IsNotNull(MainCamera);

        StartingAsteroidsCount = _startingAsteroids;

        GameSettings.SetScreenLimits(MainCamera);
    }

    private void OnEnable()
    {
        Player.OnPlayerDestroyed += PlayerDestroyed;
        _enemiesFactory.OnAllEnemiesInTheLevelDestroyed += AllEnemiesInTheLevelDestroyed;
    }

    private void OnDisable()
    {
        Player.OnPlayerDestroyed -= PlayerDestroyed;
        _enemiesFactory.OnAllEnemiesInTheLevelDestroyed -= AllEnemiesInTheLevelDestroyed;
    }

    private void Start()
    {
        StartGame();
    }

    #endregion

    #region Public Methods

    public void StartGame()
    {
        Time.timeScale = 1f;

        Player = _player;
        PlayerShips = _startingPlayerShips;

        _currentLevel = 1;

        LoadNewLevel(_currentLevel, _startingAsteroids);
        SpawnUFOs();

        OnGameStarted?.Invoke();
    }

    #endregion

    #region Private Methods

    private void PlayerDestroyed()
    {
        PlayerShips--;
        OnPlayerDestroyed?.Invoke(PlayerShips);

        if (PlayerShips <= 0)
        {
            PlayerShips = 0;
            GameOver();
        }
    }

    private void AllEnemiesInTheLevelDestroyed()
    {
        _currentLevel++;
        var newAsteroidsCount = _currentLevel + _startingAsteroids;
        OnLevelCleared?.Invoke(_currentLevel);
        LoadNewLevel(_currentLevel, newAsteroidsCount);
    }

    private void GameOver()
    {
        if (_spawnUFOsCoroutine != null)
        {
            StopCoroutine(_spawnUFOsCoroutine);
        }

        Time.timeScale = 0;
        OnGameOver?.Invoke();
    }

    private void LoadNewLevel(int level, int startingAsteroids)
    {
        _enemiesFactory.LoadNewLevel(level, startingAsteroids);
    }

    private void SpawnUFOs()
    {
        if (_spawnUFOsCoroutine != null)
        {
            StopCoroutine(_spawnUFOsCoroutine);
        }

        _spawnUFOsCoroutine = StartCoroutine(SpawnUFOsCoroutine());

        IEnumerator SpawnUFOsCoroutine()
        {
            var waitTime = 3.5f - _currentLevel * 0.2f;

            while (true)
            {
                yield return new WaitForSeconds(waitTime);
                _enemiesFactory.SpawnUFOs(EnemiesFactory.UFOSize.Big);

                yield return new WaitForSeconds(waitTime);
                _enemiesFactory.SpawnUFOs(EnemiesFactory.UFOSize.Small);

                yield return new WaitForSeconds(waitTime * 2f);
            }
        }
    }

    #endregion
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Text _level;
    [SerializeField] private Text _playerShips;
    [SerializeField] private Text _score;
    [SerializeField] private Text _gameOverScore;
    [SerializeField] private GameObject _gameOverPanel;

    private int _currentLevel;
    private int _currentShips;
    private int _currentScore;

    private void Awake()
    {
        HideGameOverPanel();
    }

    private void OnEnable()
    {
        GameController.OnGameStarted += GameStarted;
        GameController.OnGameOver += GameOver;
        GameController.OnLevelCleared += UpdateLevel;
        GameController.OnPlayerDestroyed += UpdateShips;
        Enemy.OnEnemyDestroyed += UpdateScore;
    }

    private void OnDisable()
    {
        GameController.OnGameStarted -= GameStarted;
        GameController.OnGameOver -= GameOver;
        GameController.OnLevelCleared -= UpdateLevel;
        GameController.OnPlayerDestroyed -= UpdateShips;
        Enemy.OnEnemyDestroyed -= UpdateScore;
    }

    private void Start()
    {
        _currentScore = 0;
    }

    private void GameStarted()
    {
        HideGameOverPanel();
        ResetScore();
    }

    private void ResetScore()
    {
        _currentScore = 0;
        UpdateLevel(1);
        UpdateShips(GameController.PlayerShips);
    }

    private void HideGameOverPanel()
    {
        _gameOverPanel.SetActive(false);
    }

    private void GameOver()
    {
        _gameOverScore.text = $"Your Score: {_currentScore:0000}";
        _gameOverPanel.SetActive(true);
    }

    private void UpdateLevel(int nextLevel)
    {
        _level.text = $"Level: {nextLevel:0000}";
    }

    private void UpdateShips(int ships)
    {
        _playerShips.text = $"Ships: {ships:0000}";
    }

    private void UpdateScore(Enemy enemy)
    {
        _currentScore += enemy.KillPoints;
        _score.text = $"Score: {_currentScore:0000}";
    }
}
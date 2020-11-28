using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Text _level;
    [SerializeField] private Text _playerShips;
    [SerializeField] private Text _score;
    
    [SerializeField] private GameObject _gameOverPanel;

    private int _currentScore;
    
    private void Awake()
    {
        HideGameOverPanel();
    }

    private void OnEnable()
    {
        GameController.OnGameStarted += GameStarted;
        GameController.OnGameOver += GameOver;
        GameController.OnLevelCleared += LevelCleared;
        GameController.OnPlayerDestroyed += PlayerDestroyed;
        Enemy.OnEnemyDestroyed += EnemyDestroyed;
    }

    private void OnDisable()
    {
        GameController.OnGameStarted -= GameStarted;
        GameController.OnGameOver -= GameOver;
        GameController.OnLevelCleared -= LevelCleared;
        GameController.OnPlayerDestroyed -= PlayerDestroyed;
        Enemy.OnEnemyDestroyed -= EnemyDestroyed;
    }

    private void Start()
    {
        _currentScore = 0;
    }

    private void GameStarted()
    {
        HideGameOverPanel();
    }

    private void HideGameOverPanel()
    {
        _gameOverPanel.SetActive(false);
    }
    private void GameOver()
    {
        _gameOverPanel.SetActive(true);
    }
    
    private void LevelCleared(int nextLevel)
    {
        _level.text = $"Level: {nextLevel:0000}";
    }

    private void PlayerDestroyed(int ships)
    {
        _playerShips.text = $"Ships: {ships:0000}";
    }
    
    private void EnemyDestroyed(Enemy enemy, Queue<Enemy> arg2)
    {
        _currentScore += enemy.KillPoints;
        _score.text = $"Score: {_currentScore:0000}";
    }
}

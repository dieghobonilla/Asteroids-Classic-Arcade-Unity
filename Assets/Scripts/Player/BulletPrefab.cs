using System;
using System.Collections;
using UnityEngine;

public class BulletPrefab : GameEntity
{
    [SerializeField] private float _bulletSpeed = 8f;
    [SerializeField] private float _bulletLifeTime = 0.5f;
    [SerializeField] private BulletsManager _bulletsManager;

    private bool _isActive;
    private Coroutine _fireCoroutine;

    private void OnEnable()
    {
        GameController.OnGameOver += GameOver;
    }

    private void OnDisable()
    {
        GameController.OnGameOver -= GameOver;
    }

    protected override void Start()
    {
        base.Start();
        _bulletsManager = FindObjectOfType<BulletsManager>();
    }

    public void BulletHit()
    {
        DisableBullet();
    }

    public void Fire(float shipSpeed, Vector3 position, Vector3 direction)
    {
        _isActive = true;
        transform.position = position;

        if (_fireCoroutine != null)
        {
            StopCoroutine(_fireCoroutine);
        }

        _fireCoroutine = StartCoroutine(Fire());

        IEnumerator Fire()
        {
            var timer = 0f;

            while (timer < _bulletLifeTime)
            {
                timer += Time.deltaTime;
                transform.position += direction * ((shipSpeed + _bulletSpeed) * Time.deltaTime);

                yield return null;
            }

            DisableBullet();
        }
    }

    public void DisableBullet()
    {
        _isActive = false;
        if (_fireCoroutine != null)
        {
            StopCoroutine(_fireCoroutine);
        }
        
        Transform.position = Vector3.zero;
        _bulletsManager.AddBullet(this);
    }

    private void GameOver()
    {
        if (_isActive)
        {
            DisableBullet();   
        }
    }
}
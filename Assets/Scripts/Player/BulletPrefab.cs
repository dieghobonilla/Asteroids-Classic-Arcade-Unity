using System;
using System.Collections;
using UnityEngine;

public class BulletPrefab : GameEntity
{
    [SerializeField] private float _bulletSpeed = 8f;
    [SerializeField] private float _bulletLifeTime = 0.5f;
    [SerializeField] private BulletsManager _bulletsManager;

    private Coroutine _fireCoroutine;
    
    protected override void Start()
    {
        base.Start();
        _bulletsManager = FindObjectOfType<BulletsManager>();
    }

    public void BulletHit()
    {
        StopCoroutine(_fireCoroutine);
        DisableBullet();
    }

    public void Fire(float shipSpeed, Vector3 position, Vector3 direction)
    {
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

    private void DisableBullet()
    {
        _bulletsManager.AddBullet(this);
    }
}
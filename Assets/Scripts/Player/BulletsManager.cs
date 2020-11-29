using System.Collections.Generic;
using UnityEngine;

public class BulletsManager : MonoBehaviour
{
    [SerializeField] private BulletPrefab _bulletPrefab;
    [SerializeField] private int _initialBulletsBatch = 10;

    private readonly Queue<BulletPrefab> _bulletsStack = new Queue<BulletPrefab>();

    private void OnEnable()
    {
        Player.OnFire += Fire;
        UFO.OnFire += Fire;
    }

    private void OnDisable()
    {
        Player.OnFire -= Fire;
        UFO.OnFire -= Fire;
    }

    private void Start()
    {
        CreateNewBulletsBatch();
    }

    private void CreateNewBulletsBatch()
    {
        // create 20 bullets
        for (var i = 0; i < _initialBulletsBatch; i++)
        {
            AddBullet(Instantiate(_bulletPrefab));
        }
    }

    private void Fire(float shipSpeed, Vector3 position, Vector3 direction)
    {
        GetBullet().Fire(shipSpeed, position, direction);
    }

    private void GameStarted()
    {
        foreach (var bulletPrefab in _bulletsStack)
        {
            bulletPrefab.DisableBullet();
        }
    }

    private BulletPrefab GetBullet()
    {
        if (_bulletsStack.Count <= 0)
        {
            // hopefully we never reach this code
            Debug.LogWarning($"{_initialBulletsBatch} bullets as initial batch was not enough, create more on start");
            CreateNewBulletsBatch();
        }

        var bullet = _bulletsStack.Dequeue();
        bullet.SetActive(true);

        return bullet;
    }

    public void AddBullet(BulletPrefab bullet)
    {
        bullet.SetActive(false);
        _bulletsStack.Enqueue(bullet);
    }
}
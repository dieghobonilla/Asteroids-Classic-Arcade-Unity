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
        Enemy.OnEnemyDestroyed += EnemyDestroyed;
    }

    private void OnDisable()
    {
        Player.OnFire -= Fire;
        UFO.OnFire -= Fire;
        Enemy.OnEnemyDestroyed -= EnemyDestroyed;
    }

    private void EnemyDestroyed(Enemy enemy, Queue<Enemy> queue)
    {
        
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

    private BulletPrefab GetBullet()
    {
        if (_bulletsStack.Count <= 0)
        {
            // hopefully we never reach this code
            Debug.LogWarning($"{_initialBulletsBatch} bullets as initial batch was not enough, create more on start");
            CreateNewBulletsBatch();
        }

        var bullet = _bulletsStack.Dequeue();
        bullet.gameObject.SetActive(true);

        return bullet;
    }

    public void AddBullet(BulletPrefab bullet)
    {
        bullet.gameObject.SetActive(false);
        _bulletsStack.Enqueue(bullet);
    }
}
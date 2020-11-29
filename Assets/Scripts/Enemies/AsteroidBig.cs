using UnityEngine;

public class AsteroidBig : Asteroid
{
    private float _randomRotationAngle;

    protected override void Start()
    {
        base.Start();
        _randomRotationAngle = Random.Range(-30f, 30f); // degrees
    }

    protected override void Update()
    {
        base.Update();
        Transform.Rotate(Vector3.forward, _randomRotationAngle * Time.deltaTime);
    }
    
    protected override void OnEnemyCreated()
    {
        
    }
    
    protected override void OnEnemyKilled()
    {
        EnemiesFactory.SpawnAsteroids(Transform.position, EnemiesFactory.AsteroidsSize.Medium);
    }
}
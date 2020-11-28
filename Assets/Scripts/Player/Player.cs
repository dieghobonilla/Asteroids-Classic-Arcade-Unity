using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : GameEntity
{
    public static event Action OnPlayerDestroyed = delegate { };
    public static event Action<float, Vector3, Vector3> OnFire = delegate { };

    [SerializeField] private float _rotationSpeed = 360f;
    [SerializeField] private float _fireRate = 0.1f;
    [SerializeField] private int _maxBulletsRapidFire = 4;
    [SerializeField] private Sprite _shipPropulsionSprite;
    [SerializeField] private Sprite _normalShipSprite;

    private float _nextBulletTime;
    private bool _shipPropulsion;

    private int _rapidFireBulletsCount;
    private Coroutine _playerResetCoroutine;

    #region Unity Methods

    protected override void Update()
    {
        base.Update();

        ProcessInput();
        SetGraphics();
    }

    private void FixedUpdate()
    {
        Accelerate();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Bullet"))
        {
            PlayerReset();
            OnPlayerDestroyed?.Invoke();
        }
    }

    private void PlayerReset()
    {
        Rigidbody.velocity = Vector2.zero;
        Transform.position = Vector3.zero;

        var boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = false;

        if (_playerResetCoroutine != null)
        {
            StopCoroutine(_playerResetCoroutine);
        }
        
        _playerResetCoroutine = StartCoroutine(PlayerResetCoroutine());

        IEnumerator PlayerResetCoroutine()
        {
            var waitTime = 0.1f;
            var blinkTime = 3f;
            // waitTime * 2 -> 1 off, 1 on
            // how many times you need to wait in a second?
            // multiply that by the blink time
            // 1 second / waitTime (0.2f) = (5)
            // (5) x 3 seconds (blinkTime) = 15 iterations
            var iterations = (1f / (waitTime * 2)) * blinkTime;
            var enableRender = SpriteRenderer.enabled;

            // blinks
            for (var i = 0; i < iterations; i++)
            {
                //enableRender = false;
                SpriteRenderer.enabled = false;
                yield return new WaitForSeconds(0.1f);

                SpriteRenderer.enabled = true;
                yield return new WaitForSeconds(0.1f);
            }

            SpriteRenderer.enabled = true;
            boxCollider.enabled = true;
        }
    }

    #endregion

    private void ProcessInput()
    {
        _shipPropulsion = Input.GetAxis("Vertical") > 0;

        var horizontalInput = Input.GetAxis("Horizontal");

        if (Mathf.Abs(horizontalInput) > 0)
        {
            Rotate(-horizontalInput);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            Fire();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            // release rapid fire
            _rapidFireBulletsCount = 0;
        }
        
        // hyperspace
        if (Input.GetKeyDown(KeyCode.B))
        {
            Hyperspace();
        }
        
        // flip
        if (Input.GetKeyDown(KeyCode.V))
        {
            Flip();
        }
        
        // Shield
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Shield functionality");
        }
    }

    private void Flip()
    {
        Transform.rotation = Quaternion.Euler(0, 0, Transform.rotation.eulerAngles.z + 180f);
    }

    private void Hyperspace()
    {
        var randomX = Random.Range(-GameSettings.ScreenLimits.x, GameSettings.ScreenLimits.x);
        var randomY = Random.Range(-GameSettings.ScreenLimits.y, GameSettings.ScreenLimits.y);
        var newPosition = new Vector2(randomX, randomY);
        Transform.position = newPosition;
    }

    private void SetGraphics()
    {
        SpriteRenderer.sprite = _shipPropulsion ? _shipPropulsionSprite : _normalShipSprite;
    }

    private void Accelerate()
    {
        if (_shipPropulsion)
        {
            Rigidbody.AddForce(transform.up * MovementSpeed);
        }
    }

    private void Rotate(float horizontalInput)
    {
        Transform.Rotate(Vector3.forward * (horizontalInput * _rotationSpeed * Time.deltaTime));
    }

    private void Fire()
    {
        if (Time.time >= _nextBulletTime)
        {
            _nextBulletTime = Time.time + _fireRate;
            OnFire?.Invoke(Rigidbody.velocity.magnitude, Transform.position, Transform.up);

            // play with rapid fire check?
            // RapidFireCheck();
        }
    }

    private void RapidFireCheck()
    {
        _rapidFireBulletsCount++;

        if (_rapidFireBulletsCount > _maxBulletsRapidFire)
        {
            _rapidFireBulletsCount = 0;
            _nextBulletTime = Time.time + 0.5f; // wait for 0.5 seconds
        }
    }
}
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class GameEntity : MonoBehaviour
{
    [SerializeField] protected float MovementSpeed = 50f;

    public Rigidbody2D Rigidbody;
    protected SpriteRenderer SpriteRenderer;
    protected BoxCollider2D BoxCollider;
    
    protected Transform Transform { get; private set; }

    // Avoid creating new Vector //new Vector3(transform.position.x, GameController.ScreenLimits.y);
    private Vector2 _screenLimitsCacheLeft = Vector2.zero;
    private Vector2 _screenLimitsCacheRight = Vector2.zero;
    private Vector2 _screenLimitsCacheUp = Vector2.zero;
    private Vector2 _screenLimitsCacheDown = Vector2.zero;

    private Vector2 _entitySize;
    
    protected virtual void Awake()
    {
        Transform = transform;

        SpriteRenderer = GetComponent<SpriteRenderer>();
        Rigidbody = GetComponent<Rigidbody2D>();
        BoxCollider = GetComponent<BoxCollider2D>();

        Assert.IsNotNull(SpriteRenderer);
        Assert.IsNotNull(Rigidbody);
        Assert.IsNotNull(BoxCollider);

        _entitySize = SpriteRenderer.bounds.extents;
    }

    protected virtual void Start()
    {
        _screenLimitsCacheLeft.x = -GameSettings.ScreenLimits.x + -_entitySize.x;
        _screenLimitsCacheRight.x = GameSettings.ScreenLimits.x + _entitySize.x;
        _screenLimitsCacheUp.y = GameSettings.ScreenLimits.y + _entitySize.y;
        _screenLimitsCacheDown.y = -GameSettings.ScreenLimits.y + -_entitySize.y ;
    }

    protected virtual void Update()
    {
        CheckScreenLimits();
    }

    public void SetActive(bool isActive)
    {
        SpriteRenderer.enabled = isActive;
        BoxCollider.enabled = isActive;

        if (!isActive)
        {
            Rigidbody.velocity = Vector2.zero;
        }
    }

    private void CheckScreenLimits()
    {
        LeftRightLimits();
        UpDownLimits();
    }

    private void LeftRightLimits()
    {
        if (Transform.position.x < (-GameSettings.ScreenLimits.x + -_entitySize.x))
        {
            // new Vector3(GameController.ScreenLimits.x, transform.position.y);
            _screenLimitsCacheRight.y = Transform.position.y;
            Transform.position = _screenLimitsCacheRight;
        }
        else if (Transform.position.x > (GameSettings.ScreenLimits.x + _entitySize.x))
        {
            // new Vector3(-GameController.ScreenLimits.x, transform.position.y);
            _screenLimitsCacheLeft.y = Transform.position.y;
            Transform.position = _screenLimitsCacheLeft;
        }
    }

    private void UpDownLimits()
    {
        if (Transform.position.y > (GameSettings.ScreenLimits.y + _entitySize.y))
        {
            // new Vector3(transform.position.x, -GameController.ScreenLimits.y);
            _screenLimitsCacheDown.x = Transform.position.x;
            Transform.position = _screenLimitsCacheDown;
        }
        else if (Transform.position.y < (-GameSettings.ScreenLimits.y + -_entitySize.y))
        {
            //new Vector3(transform.position.x, GameController.ScreenLimits.y);
            _screenLimitsCacheUp.x = Transform.position.x;
            Transform.position = _screenLimitsCacheUp;
        }
    }
}
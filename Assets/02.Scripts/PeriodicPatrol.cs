using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class PeriodicPatrol : MonoBehaviour
{
    private enum HorizontalDirection
    {
        Left,
        Right
    }

    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");

    [Header("Patrol")]
    [SerializeField, Min(0.1f)] private float _patrolRadius = 3f;
    [SerializeField, Min(0f)] private float _moveSpeed = 2f;
    [SerializeField] private HorizontalDirection _startMovingDirection = HorizontalDirection.Right;

    [Header("Timing")]
    [SerializeField] private Vector2 _walkTimeRange = new Vector2(1.5f, 3f);
    [SerializeField] private Vector2 _idleTimeRange = new Vector2(0.5f, 1.25f);

    [Header("Float Motion")]
    [SerializeField, Min(0f)] private float _floatAmplitude = 0.08f;
    [SerializeField, Min(0f)] private float _floatFrequency = 0.8f;
    [SerializeField, Range(0f, 1f)] private float _floatPhaseOffset;

    [Header("Ledge Safety")]
    [SerializeField] private bool _turnAtLedges = true;
    [SerializeField] private LayerMask _groundLayers;
    [SerializeField, Min(0f)] private float _lookAheadDistance = 0.5f;
    [SerializeField, Min(0f)] private float _ledgeProbeDepth = 1.5f;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private HorizontalDirection _sourceSpriteDirection = HorizontalDirection.Right;

    private Rigidbody2D _body;
    private float _originX;
    private float _originY;
    private float _direction;
    private float _stateTimer;
    private float _floatTime;
    private bool _isMoving;

    private void Reset()
    {
        _body = GetComponent<Rigidbody2D>();
        _body.bodyType = RigidbodyType2D.Kinematic;
        _body.freezeRotation = true;

        _animator = GetComponentInChildren<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _body.bodyType = RigidbodyType2D.Kinematic;
        _body.gravityScale = 0f;
        _body.freezeRotation = true;

        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }

        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void OnEnable()
    {
        _originX = transform.position.x;
        _originY = transform.position.y;
        _floatTime = 0f;
        _direction = _startMovingDirection == HorizontalDirection.Right ? 1f : -1f;
        ApplyFacingDirection();
        SetMoving(true);
    }

    private void OnDisable()
    {
        if (_animator != null)
        {
            _animator.SetBool(IsMovingHash, false);
        }
    }

    private void Update()
    {
        _stateTimer -= Time.deltaTime;
        if (_stateTimer > 0f)
        {
            return;
        }

        SetMoving(!_isMoving);
    }

    private void FixedUpdate()
    {
        _floatTime += Time.fixedDeltaTime;
        float nextX = _body.position.x;

        if (_isMoving)
        {
            if (IsGroundMissingAhead())
            {
                TurnAroundAndWait();
            }
            else
            {
                float leftBoundary = _originX - _patrolRadius;
                float rightBoundary = _originX + _patrolRadius;
                nextX += _direction * _moveSpeed * Time.fixedDeltaTime;

                if (nextX <= leftBoundary || nextX >= rightBoundary)
                {
                    nextX = Mathf.Clamp(nextX, leftBoundary, rightBoundary);
                    TurnAroundAndWait();
                }
            }
        }

        float phase = (_floatTime * _floatFrequency + _floatPhaseOffset) * Mathf.PI * 2f;
        float nextY = _originY + Mathf.Sin(phase) * _floatAmplitude;

        _body.MovePosition(new Vector2(nextX, nextY));
    }

    private void SetMoving(bool isMoving)
    {
        _isMoving = isMoving;
        _stateTimer = GetRandomDuration(isMoving ? _walkTimeRange : _idleTimeRange);

        if (_animator != null)
        {
            _animator.SetBool(IsMovingHash, isMoving);
        }
    }

    private void TurnAroundAndWait()
    {
        _direction *= -1f;
        ApplyFacingDirection();
        SetMoving(false);
    }

    private bool IsGroundMissingAhead()
    {
        if (!_turnAtLedges || _groundLayers.value == 0)
        {
            return false;
        }

        Vector2 rayOrigin = _body.position + (Vector2.right * (_direction * _lookAheadDistance));
        return !Physics2D.Raycast(rayOrigin, Vector2.down, _ledgeProbeDepth, _groundLayers);
    }

    private void ApplyFacingDirection()
    {
        if (_spriteRenderer == null)
        {
            return;
        }

        bool movingRight = _direction > 0f;
        bool sourceFacesRight = _sourceSpriteDirection == HorizontalDirection.Right;
        _spriteRenderer.flipX = sourceFacesRight ? !movingRight : movingRight;
    }

    private static float GetRandomDuration(Vector2 range)
    {
        float minimum = Mathf.Max(0.01f, Mathf.Min(range.x, range.y));
        float maximum = Mathf.Max(minimum, Mathf.Max(range.x, range.y));
        return Random.Range(minimum, maximum);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        float centerX = Application.isPlaying ? _originX : transform.position.x;
        float y = transform.position.y;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(
            new Vector3(centerX - _patrolRadius, y, 0f),
            new Vector3(centerX + _patrolRadius, y, 0f));

        if (!_turnAtLedges)
        {
            return;
        }

        float direction = Application.isPlaying
            ? _direction
            : (_startMovingDirection == HorizontalDirection.Right ? 1f : -1f);
        var rayOrigin = new Vector3(
            transform.position.x + (direction * _lookAheadDistance),
            y,
            transform.position.z);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(rayOrigin, rayOrigin + (Vector3.down * _ledgeProbeDepth));
    }
#endif
}

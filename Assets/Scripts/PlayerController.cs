using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    float _speed = 5f;

    [SerializeField]
    float _rotationSpeed = 360f;

    public float RotationSpeed => _rotationSpeed;

    [SerializeField]
    CameraController _cameraController;

    [Header("Ground Check Settings")]
    [SerializeField]
    float _groundCheckRadius = 0.2f;
    [SerializeField]
    Vector3 _groundCheckOffset;
    [SerializeField]
    LayerMask _groundLayer;

    bool _hasControl;

    Animator _animator;
    CharacterController _characterController;

    Quaternion _targetRotation;

    float _ySpeed;

    bool _inAction;
    public bool InAction => _inAction;

    public bool IsHanging { get; set; }

    private static PlayerController _instance;
    public static PlayerController Instance => _instance;

    private void Awake()
    {
        _instance = this;
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        _hasControl = true;
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        float moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

        Vector3 movePosition = new Vector3(horizontal, 0, vertical).normalized;
        Vector3 moveDirection = _cameraController.PlanerRotation * movePosition;

        if (!_hasControl)
        {
            return;
        }

        if (IsHanging)
        {
            return;
        }

        if (IsGrounded())
        {
            _ySpeed = -0.5f;
        }
        else
        {
            _ySpeed += Physics.gravity.y * Time.deltaTime;
        }

        var velocity = moveDirection * _speed;
        velocity.y = _ySpeed;

        _characterController.Move(velocity * Time.deltaTime);

        if (moveAmount > 0)
        {
            _targetRotation = Quaternion.LookRotation(moveDirection);
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, _rotationSpeed * Time.deltaTime);

        _animator.SetFloat("MoveThreshold", moveAmount, 0.2f, Time.deltaTime);
    }

    public bool IsGrounded()
    {
        return Physics.CheckSphere(transform.TransformPoint(_groundCheckOffset), _groundCheckRadius, _groundLayer);
    }

    public void SetControl(bool hasControl)
    {
        _hasControl = hasControl;
        _characterController.enabled = hasControl;

        if (!hasControl)
        {
            _animator.SetFloat("MoveThreshold", 0f);
            _targetRotation = transform.rotation;
        }
    }

    public IEnumerator DoAction(string animationName, MatchTargetParams matchTargetParams,  Quaternion targetRotation, bool rotate = false)
    {
        _inAction = true;
        _animator.CrossFade(animationName, 0.2f);
        yield return null;

        float timer = 0f;
        var animationState = _animator.GetNextAnimatorClipInfo(0);

        while (timer <= animationState.Length)
        {
            timer += Time.deltaTime;

            if (rotate)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }

            if (matchTargetParams != null)
            {
                MatchTarget(matchTargetParams);
            }

            if (_animator.IsInTransition(0) && timer > 0.5f)
            {
                break;
            }

            yield return null;
        }
    }

    private void MatchTarget(MatchTargetParams matchTargetParams)
    {
        if (_animator.isMatchingTarget)
        {
            return;
        }
        _animator.MatchTarget(matchTargetParams.Position, transform.rotation, matchTargetParams.BodyPart,
            new MatchTargetWeightMask(matchTargetParams.PositionWeight, 0), matchTargetParams.StartTime, matchTargetParams.TargetTime);
    }

    public void SetAction(bool setAction)
    {
        _inAction = setAction;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(_groundCheckOffset), _groundCheckRadius);
    }

}

public class MatchTargetParams
{
    public Vector3 Position;
    public AvatarTarget BodyPart;
    public Vector3 PositionWeight;
    public float StartTime;
    public float TargetTime;

    public MatchTargetParams(Vector3 position, AvatarTarget bodyPart, Vector3 positionWeight, float startTime, float targetTime)
    {
        Position = position;
        BodyPart = bodyPart;
        PositionWeight = positionWeight;
        StartTime = startTime;
        TargetTime = targetTime;
    }

}

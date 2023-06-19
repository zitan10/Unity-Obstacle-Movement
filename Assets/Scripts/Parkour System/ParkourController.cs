using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnvironmentScanner))]
public class ParkourController : MonoBehaviour
{
    [SerializeField]
    List<ParkourAction> _parkourActions;

    EnvironmentScanner _environmentScanner;
    Animator _animator;

    static ParkourController _instance;
    public static ParkourController Instance => _instance;

    private bool _inAction;

    private void Awake()
    {
        _instance = this;
        _environmentScanner = GetComponent<EnvironmentScanner>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetButton("Jump") && !PlayerController.Instance.InAction)
        {
            ObstacleHitData hitData = _environmentScanner.ObstacleCheck();
            if (hitData.ForwardHitFound)
            {
                for(int i = 0; i < _parkourActions.Count; i++)
                {
                    if(_parkourActions[i].CanPerformAction(hitData, transform))
                    {
                        StartCoroutine(DoAction(_parkourActions[i]));
                        break;
                    }
                }
            }
            /*
            else
            {
                StartCoroutine(PlayerController.Instance.DoAction("Jump", null, Quaternion.LookRotation(transform.forward), false));
            }
            */
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && !PlayerController.Instance.InAction && PlayerController.Instance.IsGrounded())
        {
            StartCoroutine(PlayerController.Instance.DoAction("Slide", null, Quaternion.LookRotation(transform.forward), false));
        }
    }

    private IEnumerator DoAction(ParkourAction action)
    {
        PlayerController.Instance.SetControl(false);

        MatchTargetParams matchTargetParams = null;
        if (action.EnableTargetMatching)
        {
            matchTargetParams = new MatchTargetParams(action.MatchPosition, action.MatchBodyPart, action.MatchPosWeight, action.MatchStartTime, action.MatchTargetTime);
        }

        yield return PlayerController.Instance.DoAction(action.AnimationName, matchTargetParams, action.TargetRotation, action.RotateToObstacle);

    }

    public void SetAction(bool setAction)
    {
        _inAction = setAction;
    }

    private void MatchTarget(ParkourAction action)
    {
        if (_animator.isMatchingTarget)
        {
            return;
        }
        _animator.MatchTarget(action.MatchPosition, transform.rotation, action.MatchBodyPart,
            new MatchTargetWeightMask(action.MatchPosWeight, 0), action.MatchStartTime, action.MatchTargetTime);
    }

}

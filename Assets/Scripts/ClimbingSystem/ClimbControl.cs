using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(EnvironmentScanner))]
public class ClimbControl : MonoBehaviour
{

    EnvironmentScanner _environmentScanner;
    ClimbPoint _currentPoint;

    private void Awake()
    {
        _environmentScanner = GetComponent<EnvironmentScanner>();
    }

    private void Update()
    {
        if (!PlayerController.Instance.IsHanging)
        {
            if (Input.GetButton("Jump") && !PlayerController.Instance.InAction)
            {
                if(_environmentScanner.ClimbLedgeCheck(transform.forward, out RaycastHit ledgeHit))
                {
                    _currentPoint = ledgeHit.transform.GetComponent<ClimbPoint>();
                    PlayerController.Instance.SetControl(false);
                    StartCoroutine(JumpToLedge("LedgeHangJump", ledgeHit.transform, 0.41f, 0.54f));
                }
            }
        }
        else
        {
            float h = Mathf.Round(Input.GetAxisRaw("Horizontal"));
            float v = Mathf.Round(Input.GetAxisRaw("Vertical"));
            var inputDir = new Vector2(h, v);

            if (PlayerController.Instance.InAction || inputDir == Vector2.zero) return;

            var neighbour = _currentPoint.GetNeighbour(inputDir);
            if (neighbour == null) return;

            if (neighbour.ConnectionType == ConnectionType.Jump && Input.GetButton("Jump"))
            {
                _currentPoint = neighbour.point;

                if (neighbour.Direction.y == 1)
                    StartCoroutine(JumpToLedge("HopUp", _currentPoint.transform, 0.35f, 0.65f, handOffset: new Vector3(0.25f, 0.08f, 0.15f)));
                else if (neighbour.Direction.y == -1)
                    StartCoroutine(JumpToLedge("HopDown", _currentPoint.transform, 0.31f, 0.65f, handOffset: new Vector3(0.25f, 0.1f, 0.13f)));
                /*
                else if (neighbour.Direction.x == 1)
                    StartCoroutine(JumpToLedge("HopRight", _currentPoint.transform, 0.20f, 0.50f));
                else if (neighbour.Direction.x == -1)
                    StartCoroutine(JumpToLedge("HopLeft", _currentPoint.transform, 0.20f, 0.50f));
                */
            }
        }
    }

    IEnumerator JumpToLedge(string animationName, Transform ledge, float matchStartTime, float matchTargetTime, AvatarTarget hand = AvatarTarget.RightHand, Vector3? handOffset = null)
    {
        var matchParams = new MatchTargetParams(GetHandLedgePosition(ledge), AvatarTarget.RightHand, Vector3.one, matchStartTime, matchTargetTime);

        var TargerRotation = Quaternion.LookRotation(-ledge.forward);
        yield return PlayerController.Instance.DoAction(animationName, matchParams, TargerRotation);

        PlayerController.Instance.IsHanging = true;
    }

    Vector3 GetHandLedgePosition(Transform ledge)
    {
        return ledge.position + ledge.forward * 0.1f - ledge.right * 0.25f;
    }

}

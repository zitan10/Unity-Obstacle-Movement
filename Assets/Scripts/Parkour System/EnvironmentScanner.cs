using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentScanner : MonoBehaviour
{
    [SerializeField]
    Vector3 _forwardRayOffset = new Vector3(0, 0.25f, 0);

    [SerializeField]
    float _forwardRayLength = 0.8f;

    [SerializeField]
    LayerMask _obstacleLayer;

    [SerializeField]
    float _heightRayLength = 5f;

    [SerializeField]
    float _climbLedgeRayLength = 1.5f;

    [SerializeField]
    LayerMask _climbLedgeLayer;

    public ObstacleHitData ObstacleCheck()
    {
        RaycastHit raycastHitInfo;
        var forwardOrigin = transform.position + _forwardRayOffset;

        bool hitFound = 
            Physics.Raycast(forwardOrigin, transform.forward, out raycastHitInfo, _forwardRayLength, _obstacleLayer);

        Debug.DrawRay(forwardOrigin, transform.forward * _forwardRayLength, (hitFound) ? Color.red : Color.white);

        bool heightHitFound = false;
        RaycastHit heightHit = new RaycastHit();

        if (hitFound)
        {
            var heightOrigin = raycastHitInfo.point + Vector3.up * _heightRayLength;
            heightHitFound = Physics.Raycast(heightOrigin, Vector3.down, out heightHit, _heightRayLength, _obstacleLayer);

            Debug.DrawRay(heightOrigin, Vector3.down * _heightRayLength, (heightHitFound) ? Color.red : Color.white);
        }

        return new ObstacleHitData(hitFound, raycastHitInfo, heightHitFound, heightHit);
    }

    public bool ClimbLedgeCheck(Vector3 direction, out RaycastHit ledgehit)
    {
        ledgehit = new RaycastHit();

        if(direction == Vector3.zero)
        {
            return false;
        }

        var origin = transform.position + Vector3.up * 1.5f;
        var offset = new Vector3(0, 0.18f, 0);

        for (int i = 0; i < 10; i++)
        {
            Debug.DrawRay(origin + offset * i, direction);
            if (Physics.Raycast(origin + offset * i, direction, out RaycastHit hit, _climbLedgeRayLength, _climbLedgeLayer))
            {
                ledgehit = hit;
                return true;
            }
        }
        return false;
    }

}

public struct ObstacleHitData
{
    public bool ForwardHitFound;
    public bool HeightHitFound;
    public RaycastHit ForwardHit;
    public RaycastHit HeightHit;

    public ObstacleHitData(bool forwardHitFound, RaycastHit forwardHit, bool heightHitFound, RaycastHit heightHit)
    {
        ForwardHitFound = forwardHitFound;
        ForwardHit = forwardHit;
        HeightHit = heightHit;
        HeightHitFound = heightHitFound;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Parkour System/New Parkour Action")]
public class ParkourAction : ScriptableObject
{
    [SerializeField]
    string _animationName;

    [SerializeField]
    string _obstacleTag;

    public string AnimationName => _animationName;

    [SerializeField]
    float _minHeight;

    [SerializeField]
    float _maxHeight;

    [SerializeField]
    bool _rotateToObstacle;

    [Header("Target Matching")]

    [SerializeField]
    bool _enableTargetMatching = true;

    [SerializeField]
    AvatarTarget _matchBodyPart;

    [SerializeField]
    float _matchStartTime;

    [SerializeField]
    float _matchTargetTime;

    [SerializeField]
    Vector3 _matchPosWeight = new Vector3(0, 1, 0);

    public bool EnableTargetMatching => _enableTargetMatching;
    public AvatarTarget MatchBodyPart => _matchBodyPart;
    public float MatchStartTime => _matchStartTime;
    public float MatchTargetTime => _matchTargetTime;
    public Vector3 MatchPosWeight => _matchPosWeight;

    public Vector3 MatchPosition { get; set; }

    public Quaternion TargetRotation { get; set; }

    public bool RotateToObstacle => _rotateToObstacle;

    public bool CanPerformAction(ObstacleHitData hitData, Transform player)
    {
        if (!string.IsNullOrEmpty(_obstacleTag) && hitData.ForwardHit.transform.tag != _obstacleTag)
        {
            return false;
        }

        float height = hitData.HeightHit.point.y - player.position.y;
        if(height < _minHeight || height > _maxHeight)
        {
            return false;
        }

        if (_rotateToObstacle)
        {
            TargetRotation = Quaternion.LookRotation(-hitData.ForwardHit.normal);
        }

        if (_enableTargetMatching)
        {
            MatchPosition = hitData.HeightHit.point;
        }

        return true;
    }

}

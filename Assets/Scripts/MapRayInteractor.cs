using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Oculus.Interaction.Surfaces;
using Oculus.Interaction;
using Microsoft.Maps.Unity;

public class MapRayInteractor : RayInteractor
{
    [SerializeField] private MapRenderer mapRenderer;
    [SerializeField, Interface(typeof(ISelector))]
    private UnityEngine.Object _selector;

    [SerializeField]
    private Transform _rayOrigin;

    [SerializeField]
    private float _maxRayLength = 5f;

    [SerializeField]
    [Tooltip("(Meters, World) The threshold below which distances to a surface " +
                "are treated as equal for the purposes of ranking.")]
    private float _equalDistanceThreshold = 0.001f;

    private RayCandidateProperties _rayCandidateProperties = null;

    private IMovement _movement;
    private SurfaceHit _movedHit;
    private Pose _movementHitDelta = Pose.identity;

    public SurfaceHit? CollisionInfo { get; protected set; }
    public Ray Ray { get; protected set; }

    protected override void Awake()
    {
        base.Awake();
        Selector = _selector as ISelector;
        _nativeId = 0x52617949746f7220;
    }

    protected override void Start()
    {
        base.Start();
        this.AssertField(Selector, nameof(Selector));
        this.AssertField(_rayOrigin, nameof(_rayOrigin));
    }

    protected override void DoPreprocess()
    {
        Origin = _rayOrigin.transform.position;
        Rotation = _rayOrigin.transform.rotation;
        Forward = Rotation * Vector3.forward;
        Ray = new Ray(Origin, Forward);
    }
    public override object CandidateProperties => _rayCandidateProperties;

    protected override RayInteractable ComputeCandidate()
    {
        CollisionInfo = null;

        RayInteractable closestInteractable = null;
        float closestDist = float.MaxValue;
        Vector3 candidatePosition = Vector3.zero;
        var interactables = RayInteractable.Registry.List(this);

        foreach (RayInteractable interactable in interactables)
        {
            Ray ray = new Ray(_rayOrigin.position, _rayOrigin.forward);
            if (mapRenderer.Raycast(ray, out MapRendererRaycastHit hitInfo))
            {
                bool equal = Mathf.Abs(hitInfo.Distance - closestDist) < _equalDistanceThreshold;
                if ((!equal && hitInfo.Distance < closestDist) ||
                    (equal && ComputeCandidateTiebreaker(interactable, closestInteractable) > 0))
                {
                    closestDist = hitInfo.Distance;
                    closestInteractable = interactable;
                    CollisionInfo = new SurfaceHit()
                    {
                        Point = hitInfo.Point,
                        Normal = hitInfo.Normal,
                        Distance = hitInfo.Distance
                    };
                    candidatePosition = hitInfo.Point;
                }
            }
        }

        float rayDist = (closestInteractable != null ? closestDist : MaxRayLength);
        End = Origin + rayDist * Forward;

        _rayCandidateProperties = new RayCandidateProperties(closestInteractable, candidatePosition);

        return closestInteractable;
    }

    protected override int ComputeCandidateTiebreaker(RayInteractable a, RayInteractable b)
    {
        int result = base.ComputeCandidateTiebreaker(a, b);
        if (result != 0)
        {
            return result;
        }

        return a.TiebreakerScore.CompareTo(b.TiebreakerScore);
    }

    protected override void InteractableSelected(RayInteractable interactable)
    {
        if (interactable != null)
        {
            _movedHit = CollisionInfo.Value;
            Pose hitPose = new Pose(_movedHit.Point, Quaternion.LookRotation(_movedHit.Normal));
            Pose backHitPose = new Pose(_movedHit.Point, Quaternion.LookRotation(-_movedHit.Normal));
            _movement = interactable.GenerateMovement(_rayOrigin.GetPose(), backHitPose);
            if (_movement != null)
            {
                _movementHitDelta = PoseUtils.Delta(_movement.Pose, hitPose);
            }
        }
        base.InteractableSelected(interactable);
    }

    protected override void InteractableUnselected(RayInteractable interactable)
    {
        if (_movement != null)
        {
            _movement.StopAndSetPose(_movement.Pose);
        }
        base.InteractableUnselected(interactable);
        _movement = null;
    }

    protected override void DoSelectUpdate()
    {
        RayInteractable interactable = _selectedInteractable;

        if (_movement != null)
        {
            _movement.UpdateTarget(_rayOrigin.GetPose());
            _movement.Tick();
            Pose hitPoint = PoseUtils.Multiply(_movement.Pose, _movementHitDelta);
            _movedHit.Point = hitPoint.position;
            _movedHit.Normal = hitPoint.forward;
            CollisionInfo = _movedHit;
            End = _movedHit.Point;
            return;
        }

        CollisionInfo = null;
        if (interactable != null &&
            interactable.Raycast(Ray, out SurfaceHit hit, MaxRayLength, true))
        {
            End = hit.Point;
            CollisionInfo = hit;
        }
        else
        {
            End = Origin + MaxRayLength * Forward;
        }
    }

    protected override Pose ComputePointerPose()
    {
        if (_movement != null)
        {
            return _movement.Pose;
        }

        if (CollisionInfo != null)
        {
            Vector3 position = CollisionInfo.Value.Point;
            Quaternion rotation = Quaternion.LookRotation(CollisionInfo.Value.Normal);
            return new Pose(position, rotation);
        }
        return new Pose(Vector3.zero, Quaternion.identity);
    }
}


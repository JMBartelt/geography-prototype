using UnityEngine.Animations;
using UnityEngine;

public class GetMainCamera : MonoBehaviour
{
    [SerializeField] private LookAtConstraint[] lookAts;
    [SerializeField] private AimConstraint[] aimAts;
    void Start()
    {
        foreach(LookAtConstraint lookAt in lookAts)
        {            
            if(lookAt.sourceCount == 0) lookAt.AddSource(new ConstraintSource { sourceTransform = Helpers.Camera.transform, weight = 1 });
            else lookAt.SetSource(0, new ConstraintSource { sourceTransform = Helpers.Camera.transform, weight = 1 });
        }
        foreach(AimConstraint aimAt in aimAts)
        {
            if(aimAt.sourceCount == 0) aimAt.AddSource(new ConstraintSource { sourceTransform = Helpers.Camera.transform, weight = 1 });
            else aimAt.SetSource(0, new ConstraintSource { sourceTransform = Helpers.Camera.transform, weight = 1 });
        }
    }
}

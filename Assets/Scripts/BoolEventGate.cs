using UnityEngine;
using UnityEngine.Events;

public class BoolEventGate : MonoBehaviour
{
    public UnityEvent onTrue;
    public UnityEvent onFalse;

    public void SetBool(bool value)
    {
        if (value)
        {
            onTrue.Invoke();
        }
        else
        {
            onFalse.Invoke();
        }
    }

}

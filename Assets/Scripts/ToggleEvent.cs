using UnityEngine;
using UnityEngine.Events;

public class ToggleEvent : MonoBehaviour
{
    public UnityEvent onTrue;
    public UnityEvent onFalse;
    public bool state = false;

    public void Toggle()
    {
        state = !state;
        
        if(state)
        {
            onTrue.Invoke();
        }
        else
        {
            onFalse.Invoke();
        }
    }

    public void SetBool(bool value)
    {
        state = value;

        if(state)
        {
            onTrue.Invoke();
        }
        else
        {
            onFalse.Invoke();
        }
    }
}

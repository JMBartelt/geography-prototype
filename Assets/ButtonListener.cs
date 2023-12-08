using System.Collections;
using System.Collections.Generic;
using OculusSampleFramework;
using UnityEngine;
using UnityEngine.Events;

public class ButtonListener : MonoBehaviour
{
    [SerializeField] private ButtonController _buttonController;

    [SerializeField] private UnityEvent onButtonPressed = new UnityEvent();
    [SerializeField] private UnityEvent onButtonReleased = new UnityEvent();
    void Start()
    {
        // subscribe to press and unpress events
        _buttonController.InteractableStateChanged.AddListener(OnButtonStateChanged);        
    }

    private void OnButtonStateChanged(InteractableStateArgs obj)
    {
        // if button is pressed, then start listening for voice commands
        if(obj.NewInteractableState == InteractableState.ActionState)
        {
            onButtonPressed.Invoke();
        }
        // if button is unpressed, then stop listening for voice commands
        else
        {
            onButtonReleased.Invoke();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using UnityEngine;
using UnityEngine.Events;

public class HandEvents : MonoBehaviour
{
    [SerializeField] private  Hand _hand;
    public UnityEvent _onPinch;
    public UnityEvent _onUnpinch;
    private bool _isPinching = false;

    void OnEnable()
    {
        _hand.WhenHandUpdated += OnHandUpdated;
    }

    void OnDisable()
    {
        _hand.WhenHandUpdated -= OnHandUpdated;
    }

    void OnHandUpdated()
    {
        if (_hand.GetIndexFingerIsPinching() && !_isPinching)
        {
            _isPinching = true;
            _onPinch.Invoke();
        }
        else if (!_hand.GetIndexFingerIsPinching() && _isPinching)
        {
            _isPinching = false;
            _onUnpinch.Invoke();
        }
    }
}

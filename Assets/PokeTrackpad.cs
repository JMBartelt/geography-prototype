using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using UnityEngine.Events;

public class PokeTrackpad : MonoBehaviour
{
    [SerializeField] private InteractableUnityEventWrapper interactableUnityEventWrapper;
    [SerializeField] private Transform _leftFingertip;
    [SerializeField] private Transform _trackpad;
    [SerializeField] private UnityEvent<Vector2> trackpadEvent;
    [SerializeField] private UnityEvent onTrackpadRelease;

    private bool touching = false;
    void Start()
    {
        interactableUnityEventWrapper.WhenSelect.AddListener(OnSelect);
        interactableUnityEventWrapper.WhenUnselect.AddListener(OnUnselect);
    }

    void Update()
    {
        if(touching)
        {
            // Get fingertip position
            Vector3 fingertipPosition = _leftFingertip.position;
            // Get trackpad position
            Vector3 trackpadPosition = _trackpad.position;
            // Get vector from fingertip to trackpad
            Vector3 vector = fingertipPosition - trackpadPosition;
            // take account of size of trackpad
            Vector3 trackpadSize = _trackpad.localScale;
            vector.x /= trackpadSize.x;
            vector.z /= trackpadSize.y;

            // Send vector to event
            trackpadEvent.Invoke(new Vector2(vector.x, vector.z));
            Debug.Log("Vector: " + vector);
        }
    }

    private void OnSelect()
    {
        touching = true;
    }

    private void OnUnselect()
    {
        touching = false;
        onTrackpadRelease.Invoke();
    }


}

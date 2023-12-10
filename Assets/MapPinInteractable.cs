using System.Collections;
using System.Collections.Generic;
using Microsoft.Maps.Unity;
using ScriptableObjectArchitecture;
using UnityEngine;

public class MapPinInteractable : MonoBehaviour
{
    [SerializeField] private MapPin mapPin;
    [SerializeField] private MapPinGameEvent mapPinGameEvent;
    [SerializeField] private BoolVariable isMeasuringDistance;
    [SerializeField] private BoolVariable isMeasuringDrivingDistance;
    private bool placed = false;
    void Start()
    {
        // only on initial placement, we check if we're measuring distance, if so, we raise the event to pass this map pin to the measure tool
        if(placed) return;

        if (isMeasuringDistance.Value || isMeasuringDrivingDistance.Value)
        {
            transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false; // hide the sphere to make it clear this is not a normal map pin
            mapPinGameEvent.Raise(mapPin); 
        }

        placed = true;              
    }
}

using System.Collections;
using System.Collections.Generic;
using Microsoft.Maps.Unity;
using UnityEngine;

public class MapPinInteractable : MonoBehaviour
{
    [SerializeField] private MapPin mapPin;
    [SerializeField] private MapPinGameEvent mapPinGameEvent;
    void Start()
    {
        mapPinGameEvent.Raise(mapPin);       
    }
}

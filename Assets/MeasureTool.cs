using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Geospatial;
using ScriptableObjectArchitecture;
using Microsoft.Maps.Unity;
using TMPro;

public class MeasureTool : MonoBehaviour
{
    [SerializeField] private MapPinGameEvent _mapPinGameEvent;
    [SerializeField] private DistanceLabel distanceLabelPrefab; // line that will connect two map pins, and has a TMPro text mesh to display the distance
    private List<MapPin> _mapPins = new List<MapPin>();
    private List<GameObject> _distanceLabels = new List<GameObject>();
    private bool _isMeasuring = false;
    void Start()
    {
        _mapPinGameEvent.AddListener(OnMapPinSelected);
    }

    private void OnMapPinSelected(MapPin mapPin)
    {
        if (_isMeasuring)
        {
            _mapPins.Add(mapPin);
            if (_mapPins.Count == 2)
            {
                // draw a line between the two map pins
                var lineRenderer = Instantiate(distanceLabelPrefab);
                _distanceLabels.Add(lineRenderer.gameObject);
                lineRenderer.SetEndpoints(_mapPins[0], _mapPins[1]);
                // calculate the distance between the two map pins
                double distance = DistanceCalculator.CalculateDistance(_mapPins[0].Location, _mapPins[1].Location);
                // set the text of the line renderer to the distance
                lineRenderer.SetText(distance);
                // reset the map pins
                _mapPins.Clear();
            }
        }
    }
    public void BeginMeasuringDistance()
    {
        _isMeasuring = true;

        
        if (_distanceLabels.Count == 0)
        {
            return;
        }
        
        foreach (var distanceLabel in _distanceLabels) // show all distance labels
        {
            distanceLabel.SetActive(true);
        }
    }

    public void EndMeasuringDistance()
    {
        _isMeasuring = false;

        if (_distanceLabels.Count == 0)
        {
            return;
        }

        foreach (var distanceLabel in _distanceLabels) // hide all distance labels
        {
            distanceLabel.SetActive(false);
        }
    }

    public void ToggleMeasureTool()
    {
        if(_isMeasuring)
        {
            EndMeasuringDistance();
        }
        else
        {
            BeginMeasuringDistance();
        }
    }
    
}

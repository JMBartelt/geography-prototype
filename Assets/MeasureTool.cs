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
    private List<MapPin> mapPinPairs = new List<MapPin>();
    private List<MapPin> allMapPins = new List<MapPin>();
    private List<GameObject> _distanceLabels = new List<GameObject>();
    [SerializeField] private BoolVariable _isMeasuring;
    [SerializeField] private BoolVariable _measuringDrivingDistance;
    [SerializeField] private GameObject toolActiveIndicator;
    [SerializeField] private GameObject drivingDistanceToolActiveIndicator;
    [SerializeField] private DrivingDistance drivingDistance;
    private DistanceLabel currentDrivingDistanceLabel;
    void Start()
    {
        _isMeasuring.Value = false;
        _measuringDrivingDistance.Value = false;
        _mapPinGameEvent.AddListener(OnMapPinSelected);
    }

    private void OnMapPinSelected(MapPin mapPin)
    {
        if (_isMeasuring.Value)
        {
            mapPinPairs.Add(mapPin);
            if (mapPinPairs.Count == 2)
            {
                // draw a line between the two map pins
                var lineRenderer = Instantiate(distanceLabelPrefab);
                _distanceLabels.Add(lineRenderer.gameObject);
                lineRenderer.SetEndpoints(mapPinPairs[0], mapPinPairs[1]);
                // calculate the distance between the two map pins
                double distance = DistanceCalculator.CalculateDistance(mapPinPairs[0].Location, mapPinPairs[1].Location);
                // set the text of the line renderer to the distance
                // round distance to nearest whole meter
                lineRenderer.SetText(distance.ToString("F0") + " m");
                // add the pins to the list of all pins
                allMapPins.Add(mapPinPairs[0]);
                allMapPins.Add(mapPinPairs[1]);
                // reset the map pins
                mapPinPairs.Clear();
            }
        }

        if (_measuringDrivingDistance.Value)
        {
            mapPinPairs.Add(mapPin);
            if (mapPinPairs.Count == 2)
            {
                // draw a line between the two map pins
                currentDrivingDistanceLabel = Instantiate(distanceLabelPrefab);
                _distanceLabels.Add(currentDrivingDistanceLabel.gameObject);
                currentDrivingDistanceLabel.SetEndpoints(mapPinPairs[0], mapPinPairs[1]);
                // calculate the distance between the two map pins
                drivingDistance.CalculateDistance(mapPinPairs[0].Location, mapPinPairs[1].Location, OnDrivingDistanceCalculated); // pass a callback to be invoked when the distance is calculated                
            }
        }
    }

    private void OnDrivingDistanceCalculated(float distance)
    {
        // set the text of the line renderer to the distance  
        // convert meters to miles
        float distanceInMiles = distance * 0.000621371f;  
        // round double to 1 decimal place
        currentDrivingDistanceLabel.SetText("Driving distance:\n"+distanceInMiles.ToString("F1") + " mi");
        // set line color to light blue
        currentDrivingDistanceLabel.SetLineColor(new Color(0.0f, 0.75f, 1.0f));
        // add the pins to the list of all pins
        allMapPins.Add(mapPinPairs[0]);
        allMapPins.Add(mapPinPairs[1]);
        // reset the map pins
        mapPinPairs.Clear();
        currentDrivingDistanceLabel = null;
    }
    public void BeginMeasuringDistance()
    {
        EndMeasuringDrivingDistance(); // end the driving distance tool if it's active
        _isMeasuring.Value = true;
        toolActiveIndicator.SetActive(true);
        
        if (_distanceLabels.Count == 0)
        {
            return;
        }
        
        foreach (var distanceLabel in _distanceLabels) // show all distance labels
        {
            distanceLabel.SetActive(true);
        }

        // show the map pins
        foreach (var mapPin in allMapPins)
        {
            // enable the grandchild's meshrenderer (the 'stalk' of the map pin)
            mapPin.gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void EndMeasuringDistance()
    {
        _isMeasuring.Value = false;
        toolActiveIndicator.SetActive(false);

        if (_distanceLabels.Count == 0)
        {
            return;
        }

        foreach (var distanceLabel in _distanceLabels) // hide all distance labels
        {
            distanceLabel.SetActive(false);
        }

        // hide the map pins
        foreach (var mapPin in allMapPins)
        {
            // disable the grandchild's meshrenderer (the 'stalk' of the map pin)
            mapPin.gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        if(mapPinPairs.Count > 0) // if there is a map pin pair that hasn't been connected yet, destroy it
        {
            Destroy(mapPinPairs[0].gameObject);
            mapPinPairs.Clear();
        }
    }

    public void ToggleMeasureTool()
    {
        if(_isMeasuring.Value)
        {
            EndMeasuringDistance();
        }
        else
        {
            BeginMeasuringDistance();
        }
    }

    public void BeginMeasuringDrivingDistance()
    {
        EndMeasuringDistance(); // end the normal measuring distance tool if it's active
        _measuringDrivingDistance.Value = true;
        drivingDistanceToolActiveIndicator.SetActive(true);
        
        if (_distanceLabels.Count == 0)
        {
            return;
        }
        
        foreach (var distanceLabel in _distanceLabels) // show all distance labels
        {
            distanceLabel.SetActive(true);
        }

        // show the map pins
        foreach (var mapPin in allMapPins)
        {
            // enable the grandchild's meshrenderer (the 'stalk' of the map pin)
            mapPin.gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void EndMeasuringDrivingDistance()
    {
        _measuringDrivingDistance.Value = false;
        drivingDistanceToolActiveIndicator.SetActive(false);

        if (_distanceLabels.Count == 0)
        {
            return;
        }

        foreach (var distanceLabel in _distanceLabels) // hide all distance labels
        {
            distanceLabel.SetActive(false);
        }

        // hide the map pins
        foreach (var mapPin in allMapPins)
        {
            // disable the grandchild's meshrenderer (the 'stalk' of the map pin)
            mapPin.gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        if(mapPinPairs.Count > 0) // if there is a map pin pair that hasn't been connected yet, destroy it
        {
            Destroy(mapPinPairs[0].gameObject);
            mapPinPairs.Clear();
        }
    }
    
    public void ToggleDrivingDistanceTool()
    {
        if(_measuringDrivingDistance.Value)
        {
            EndMeasuringDrivingDistance();
        }
        else
        {
            BeginMeasuringDrivingDistance();
        }
    }
}

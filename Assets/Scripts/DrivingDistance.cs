using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Microsoft.Geospatial;

public class DrivingDistance : MonoBehaviour
{
    private readonly string baseUrl = "https://api.openrouteservice.org/v2/directions/driving-car";
    private string apiKey = "5b3ce3597851110001cf62486dba425c92e349ba862848420cf70c43"; // Replace with your API key

    // test method in start
    void Start()
    {
        // start in 35.911026, -78.937137, end in 35.923339, -78.926972
        LatLon start = new LatLon(35.911026, -78.937137);
        LatLon end = new LatLon(35.923339, -78.926972);
        CalculateDistance(start, end, OnDistanceCalculated);
    }
    private void OnDistanceCalculated(float distance) // example callback
    {
        Debug.Log("Calculated Distance: " + distance + " meters");
    }

    // Delegate for the callback
    public delegate void DistanceCalculated(float distance);

    // Method to start the distance calculation
    public void CalculateDistance(LatLon start, LatLon end, DistanceCalculated onDistanceCalculated)
    {
        StartCoroutine(GetDistanceCoroutine(start, end, onDistanceCalculated));
    }

    // Coroutine to make the web request
    private IEnumerator GetDistanceCoroutine(LatLon start, LatLon end, DistanceCalculated onDistanceCalculated)
    {
        string requestUrl = $"{baseUrl}?api_key={apiKey}&start={start.LongitudeInDegrees},{start.LatitudeInDegrees}&end={end.LongitudeInDegrees},{end.LatitudeInDegrees}";
        UnityWebRequest request = UnityWebRequest.Get(requestUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
            yield break;
        }

        // Process the response and invoke the callback
        float distance = ProcessResponse(request.downloadHandler.text);
        onDistanceCalculated?.Invoke(distance);
    }

    // Method to process the API response and return the distance
    private float ProcessResponse(string json)
    {
        try
        {
            ORSResponse orsResponse = JsonUtility.FromJson<ORSResponse>(json);
            if (orsResponse.features != null && orsResponse.features.Length > 0)
            {
                return orsResponse.features[0].properties.summary.distance;
            }
            else
            {
                Debug.LogError("No route information found.");
                return 0;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error parsing JSON: " + ex.Message);
            return 0;
        }
    }
}

[Serializable]
public class ORSResponse
{
    public Feature[] features;
}

[Serializable]
public class Feature
{
    public Properties properties;
}

[Serializable]
public class Properties
{
    public Summary summary;
}

[Serializable]
public class Summary
{
    public float distance; // Distance in meters
    public float duration; // Duration in seconds
}


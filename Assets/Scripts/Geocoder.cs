using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Microsoft.Maps.Unity;

public class Geocoder : MonoBehaviour
{
    private string apiKey = "2381a5e42d024edd82abe1ca7b2c276a";
    private string baseUrl = "https://api.opencagedata.com/geocode/v1/json?q=";

    [SerializeField] private MapRenderer _mapRenderer;

    public void StartSetMapLocation(string locationName)
    {
        StartCoroutine(SetMapLocation(locationName));
    }

    private IEnumerator SetMapLocation(string locationName)
    {
        yield return GetCoordinates(locationName, (Vector2) =>
        {
            if (Vector2 != null)
            {
                Debug.Log("Setting map coordinates to: " + Vector2);
                Microsoft.Geospatial.LatLon newLocation = new Microsoft.Geospatial.LatLon(Vector2.x, Vector2.y);
                _mapRenderer.SetMapScene(new MapSceneOfLocationAndZoomLevel(newLocation, 15));
            }
            else
            {
                Debug.LogError("Failed to get coordinates for location: " + locationName);
            }
        });
    }

    public IEnumerator GetCoordinates(string locationName, System.Action<Vector2> callback)
    {
        string url = $"{baseUrl}{UnityWebRequest.EscapeURL(locationName)}&key={apiKey}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {webRequest.error}");
            }
            else
            {
                ProcessResponse(webRequest.downloadHandler.text, callback);
            }
        }
    }

    private void ProcessResponse(string json, System.Action<Vector2> callback)
    {
        OpenCageData data = JsonUtility.FromJson<OpenCageData>(json);

        if (data.results.Length > 0)
        {
            float lat = data.results[0].geometry.lat;
            float lng = data.results[0].geometry.lng;
            callback(new Vector2(lat, lng));
        }
        else
        {
            Debug.LogError("No results found for: " + json);
        }
    }
}

[System.Serializable]
public class OpenCageData
{
    public Result[] results;
}

[System.Serializable]
public class Result
{
    public Geometry geometry;
}

[System.Serializable]
public class Geometry
{
    public float lat;
    public float lng;
}
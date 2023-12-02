using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Microsoft.Maps.Unity;

public class Geocoding : MonoBehaviour
{
    private string apiKey = "2381a5e42d024edd82abe1ca7b2c276a";
    private string baseUrl = "https://api.opencagedata.com/geocode/v1/json?q=";

    [SerializeField] private MapRenderer _mapRenderer;

    public void StartSetMapLocation(string voiceCommand)
    {
        if(voiceCommand.ToLower().Contains("zoom")) // handle zoom command if present
        {
            // if contains 'in' or 'out' then zoom the map in or out
            if(voiceCommand.ToLower().Contains("in"))
            {
                float zoomLevel = _mapRenderer.ZoomLevel + 1;
                // zoom extra if they say anything like 'zoom in a lot' or 'zoom in a bunch' or 'zoom in a ton' or 'more'
                if(voiceCommand.ToLower().Contains("lot") || voiceCommand.ToLower().Contains("bunch") || voiceCommand.ToLower().Contains("ton") || voiceCommand.ToLower().Contains("more"))
                {
                    zoomLevel += 1.5f;
                }
                // use SetMapScene to zoom so it animates nicely
                _mapRenderer.SetMapScene(new MapSceneOfLocationAndZoomLevel(_mapRenderer.Center, zoomLevel));
            }
            else if(voiceCommand.ToLower().Contains("out"))
            {
                float zoomLevel = _mapRenderer.ZoomLevel - 1;
                // zoom extra if they say anything like 'zoom in a lot' or 'zoom in a bunch' or 'zoom in a ton' or 'more'
                if(voiceCommand.ToLower().Contains("lot") || voiceCommand.ToLower().Contains("bunch") || voiceCommand.ToLower().Contains("ton") || voiceCommand.ToLower().Contains("more"))
                {
                    zoomLevel -= 1.5f;
                }
                // use SetMapScene to zoom so it animates nicely
                _mapRenderer.SetMapScene(new MapSceneOfLocationAndZoomLevel(_mapRenderer.Center, zoomLevel));
            }
        }
        else if(voiceCommand.ToLower().Contains("go") || voiceCommand.ToLower().Contains("move") && (voiceCommand.ToLower().Contains("north") || voiceCommand.ToLower().Contains("south") || voiceCommand.ToLower().Contains("east") || voiceCommand.ToLower().Contains("west")))
        {
            // if contains 'north' or 'south' or 'east' or 'west' then pan the map in that direction
            // use SetMapScene to pan so it animates nicely
            if(voiceCommand.ToLower().Contains("north"))
            {
                _mapRenderer.SetMapScene(new MapSceneOfLocationAndZoomLevel(new Microsoft.Geospatial.LatLon(_mapRenderer.Center.LatitudeInDegrees + 0.01f, _mapRenderer.Center.LongitudeInDegrees), _mapRenderer.ZoomLevel));
            }
            else if(voiceCommand.ToLower().Contains("south"))
            {
                _mapRenderer.SetMapScene(new MapSceneOfLocationAndZoomLevel(new Microsoft.Geospatial.LatLon(_mapRenderer.Center.LatitudeInDegrees - 0.01f, _mapRenderer.Center.LongitudeInDegrees), _mapRenderer.ZoomLevel));
            }
            else if(voiceCommand.ToLower().Contains("east"))
            {
                _mapRenderer.SetMapScene(new MapSceneOfLocationAndZoomLevel(new Microsoft.Geospatial.LatLon(_mapRenderer.Center.LatitudeInDegrees, _mapRenderer.Center.LongitudeInDegrees + 0.01f), _mapRenderer.ZoomLevel));
            }
            else if(voiceCommand.ToLower().Contains("west"))
            {
                _mapRenderer.SetMapScene(new MapSceneOfLocationAndZoomLevel(new Microsoft.Geospatial.LatLon(_mapRenderer.Center.LatitudeInDegrees, _mapRenderer.Center.LongitudeInDegrees - 0.01f), _mapRenderer.ZoomLevel));
            }
        }
        else
        {
            // otherwise, assume its a location, so set the map location to voice command
            StartCoroutine(SetMapLocation(voiceCommand));
        }
    }

    private IEnumerator SetMapLocation(string locationName)
    {
        yield return GetCoordinates(locationName, (Vector2) =>
        {
            if (Vector2 != null)
            {
                Debug.Log("Setting map coordinates to: " + Vector2);
                Microsoft.Geospatial.LatLon newLocation = new Microsoft.Geospatial.LatLon(Vector2.x, Vector2.y);
                _mapRenderer.SetMapScene(new MapSceneOfLocationAndZoomLevel(newLocation, 13f));
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

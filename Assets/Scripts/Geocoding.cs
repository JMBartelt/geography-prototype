using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Microsoft.Maps.Unity;
using TMPro;
using System;

public class Geocoding : MonoBehaviour
{
    private string apiKey = "2381a5e42d024edd82abe1ca7b2c276a";
    private string baseUrl = "https://api.opencagedata.com/geocode/v1/json?q=";
    [SerializeField] private string startupLocation = "";
    [SerializeField] private float startupZoomLevel = 16f;
    [SerializeField] private MapRenderer _mapRenderer;
    [SerializeField] private TextMeshProUGUI _commandText;
    [SerializeField] private TextMeshProUGUI _locationText;

    // Location info texts
    [SerializeField] private TextMeshProUGUI formattedAddressText, timezoneText, currencyText, continentText, countryText, stateText, cityText, postcodeText, driveOnText;

    void Start()
    {
        if(!string.IsNullOrEmpty(startupLocation))
        {
            StartCoroutine(SetMapLocation(startupLocation, startupZoomLevel));
        }
    }
    public void StartSetMapLocation(string voiceCommand)
    {
        if(voiceCommand.ToLower().Contains("zoom")) // handle zoom command if present
        {            
            SetCommandText(voiceCommand);
            // if contains 'in' or 'out' then zoom the map in or out
            if(voiceCommand.ToLower().Contains("in"))
            {
                float zoomLevel = _mapRenderer.ZoomLevel + 1;
                // zoom extra if they say anything like 'zoom in a lot' or 'zoom in a bunch' or 'zoom in a ton' or 'more'
                if(voiceCommand.ToLower().Contains("way") || voiceCommand.ToLower().Contains("lot") || voiceCommand.ToLower().Contains("bunch") || voiceCommand.ToLower().Contains("ton") || voiceCommand.ToLower().Contains("more"))
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
                if(voiceCommand.ToLower().Contains("way") || voiceCommand.ToLower().Contains("lot") || voiceCommand.ToLower().Contains("bunch") || voiceCommand.ToLower().Contains("ton") || voiceCommand.ToLower().Contains("more"))
                {
                    zoomLevel -= 1.5f;
                }
                // use SetMapScene to zoom so it animates nicely
                _mapRenderer.SetMapScene(new MapSceneOfLocationAndZoomLevel(_mapRenderer.Center, zoomLevel));
            }
        }
        else if(voiceCommand.ToLower().Contains("go") || voiceCommand.ToLower().Contains("move") && (voiceCommand.ToLower().Contains("north") || voiceCommand.ToLower().Contains("south") || voiceCommand.ToLower().Contains("east") || voiceCommand.ToLower().Contains("west")))
        {
            SetCommandText(voiceCommand);
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
                _locationText.text = locationName;
                Debug.Log("Setting map coordinates to: " + Vector2);
                Microsoft.Geospatial.LatLon newLocation = new Microsoft.Geospatial.LatLon(Vector2.x, Vector2.y);
                _mapRenderer.SetMapScene(new MapSceneOfLocationAndZoomLevel(newLocation, 13.5f));
            }
            else
            {
                Debug.LogError("Failed to get coordinates for location: " + locationName);
            }
        });
    }

    private IEnumerator SetMapLocation(string locationName, float zoomLevel)
    {
        yield return GetCoordinates(locationName, (Vector2) =>
        {
            if (Vector2 != null)
            {
                _locationText.text = locationName;
                Debug.Log("Setting map coordinates to: " + Vector2);
                Microsoft.Geospatial.LatLon newLocation = new Microsoft.Geospatial.LatLon(Vector2.x, Vector2.y);
                _mapRenderer.SetMapScene(new MapSceneOfLocationAndZoomLevel(newLocation, zoomLevel));
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
            DisplayLocationData(data);
            Debug.Log("Full response: " + json);
            float lat = data.results[0].geometry.lat;
            float lng = data.results[0].geometry.lng;
            callback(new Vector2(lat, lng));
        }
        else
        {
            Debug.LogError("No results found for: " + json);
        }
    }

private void DisplayLocationData(OpenCageData data)
{
    SetTextOrDisable(formattedAddressText, "Address: ", data.results[0].formatted);
    SetTextOrDisable(timezoneText, "Timezone: ", data.results[0].annotations?.timezone?.name);
    SetTextOrDisable(currencyText, "Currency: ", $"{data.results[0].annotations?.currency?.name} ({data.results[0].annotations?.currency?.symbol})");

    // Adding more details
    SetTextOrDisable(continentText, "Continent: ", data.results[0].components?.continent);
    SetTextOrDisable(countryText, "Country: ", $"{data.results[0].components?.country} ({data.results[0].components?.country_code.ToUpper()})");
    SetTextOrDisable(stateText, "State: ", $"{data.results[0].components?.state} ({data.results[0].components?.state_code})");
    SetTextOrDisable(cityText, "City: ", data.results[0].components?.city);
    SetTextOrDisable(postcodeText, "Postcode: ", data.results[0].components?.postcode);

    // Additional interesting details
    SetTextOrDisable(driveOnText, "Drives on the ", $"{data.results[0].annotations?.roadinfo?.drive_on} side of the road");
}

private void SetTextOrDisable(TextMeshProUGUI textComponent, string prefix, string dataValue)
{
    if (!string.IsNullOrEmpty(dataValue) && !string.IsNullOrWhiteSpace(dataValue))
    {
        textComponent.text = prefix + dataValue;
        textComponent.gameObject.SetActive(true);
    }
    else
    {
        textComponent.gameObject.SetActive(false);
    }
}


    private void SetCommandText(string text)
    {
        _commandText.text = text;
        Invoke("ClearCommandText", 3f);
    }

    private void ClearCommandText()
    {
        _commandText.text = "";
    }

    // Utility method to convert Unix timestamp to DateTime
    private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        System.DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dtDateTime;
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
    public string formatted;
    public Annotations annotations;
    public Components components;
    public Geometry geometry;
    // Add other properties from the JSON response as needed
}

[System.Serializable]
public class Annotations
{
    public Timezone timezone;
    public Currency currency;
    public RoadInfo roadinfo;
    public Sun sun;
    public What3Words what3words;
    public int callingcode;
}

[System.Serializable]
public class Components
{
    public string continent;
    public string country;
    public string country_code;
    public string state;
    public string state_code;
    public string city;
    public string postcode;
    public string suburb;
    // Add other components as needed
}

[System.Serializable]
public class Geometry
{
    public float lat;
    public float lng;
}

[System.Serializable]
public class Timezone
{
    public string name;
    // Add other fields as needed
}

[System.Serializable]
public class Currency
{
    public string name;
    public string symbol;
    // Add other fields from the Currency object in the JSON response
}

[System.Serializable]
public class RoadInfo
{
    public string drive_on;
    public string road;
    public string road_type;
    // Add other fields from the RoadInfo object in the JSON response
}

[System.Serializable]
public class Sun
{
    public SunTime rise;
    public SunTime set;
    // Additional fields can be added as needed
}

[System.Serializable]
public class SunTime
{
    public long apparent;
    public long astronomical;
    public long civil;
    public long nautical;
    // Additional fields can be added as needed
}

[System.Serializable]
public class Sunrise
{
    public long apparent; // Assuming Unix timestamp
    // Additional fields can be added as needed
}

[System.Serializable]
public class Sunset
{
    public long apparent; // Assuming Unix timestamp
    // Additional fields can be added as needed
}

[System.Serializable]
public class What3Words
{
    public string words;
    // Add other fields from the What3Words object in the JSON response
}



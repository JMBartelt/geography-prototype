using System.Collections;
using System.Collections.Generic;
using Microsoft.Maps.Unity;
using UnityEngine;

public class MapStyleChanger : MonoBehaviour
{
    [SerializeField] private MapRendererBase mapRendererBase;
    private MapImageryType _currentImageryType = MapImageryType.Aerial;

    public void ChangeMapStyleToAerial()
    {
        DefaultTextureTileLayer defaultTextureTileLayer = mapRendererBase.GetComponent<DefaultTextureTileLayer>();
        if (defaultTextureTileLayer != null)
        {
            _currentImageryType = MapImageryType.Aerial;
            defaultTextureTileLayer.ImageryType = MapImageryType.Aerial;
        }
        else
        {
            Debug.LogError("DefaultTextureTileLayer not found");
        }
    }

    public void ChangeMapStyleToSymbolic()
    {
        DefaultTextureTileLayer tileLayer = mapRendererBase.GetComponent<DefaultTextureTileLayer>();
        if (tileLayer != null)
        {
            _currentImageryType = MapImageryType.Symbolic;
            tileLayer.ImageryType = MapImageryType.Symbolic;
        }
        else
        {
            Debug.LogError("DefaultTextureTileLayer not found");
        }
    }

    public void ShowLabels(bool state)
    {
        DefaultTextureTileLayer tileLayer = mapRendererBase.GetComponent<DefaultTextureTileLayer>();
        if (tileLayer != null)
        {
            tileLayer.AreLabelsEnabled = state;
        }
        else
        {
            Debug.LogError("DefaultTextureTileLayer not found");
        }
    }

    public void ShowTraffic(bool state)
    {
        DefaultTrafficTextureTileLayer trafficLayer = mapRendererBase.GetComponent<DefaultTrafficTextureTileLayer>();
        if (trafficLayer != null)
        {
            trafficLayer.enabled = state;
        }
        else
        {
            Debug.LogError("DefaultTextureTileLayer not found");
        }
    }

    public void ChangeImageryStyle(int i)
    {
        DefaultTextureTileLayer tileLayer = mapRendererBase.GetComponent<DefaultTextureTileLayer>();
        if (tileLayer != null)
        {
            // clamp i between 0 and 7
            i = Mathf.Clamp(i, 0, 7);
            tileLayer.ImageryStyle = (MapImageryStyle) i;
        }
        else
        {
            Debug.LogError("DefaultTextureTileLayer not found");
        }
    }




}

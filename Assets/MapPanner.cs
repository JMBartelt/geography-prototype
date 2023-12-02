using System.Collections;
using System.Collections.Generic;
using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using UnityEngine;

public class MapPanner : MonoBehaviour
{
    [SerializeField] private MapRenderer _mapRenderer;
    [SerializeField] private float panSpeed = 0.01f;

    private Vector2 _panDirection = Vector2.zero;

    public void SetPan(Vector2 direction)
    {
        _panDirection = direction;
    }
    void Update()
    {
        if(_panDirection != Vector2.zero) Pan(_panDirection);
    }

    public void Pan(Vector2 direction)
    {
        LatLon newCenter = new LatLon(_mapRenderer.Center.LatitudeInDegrees + direction.y*panSpeed, _mapRenderer.Center.LongitudeInDegrees + direction.x*panSpeed);
        _mapRenderer.Center = newCenter;
    }


}

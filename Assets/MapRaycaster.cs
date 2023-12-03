using System.Collections;
using System.Collections.Generic;
using Microsoft.Maps.Unity;
using UnityEngine;

public class MapRaycaster : MonoBehaviour
{
    [SerializeField] private MapRenderer mapRenderer;
    [SerializeField] private GameObject reticle;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform rayOrigin;

    void Update()
    {
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
        if (mapRenderer.Raycast(ray, out MapRendererRaycastHit hitInfo))
        {
            var hitPoint = hitInfo.Point;
            // position the reticle
            reticle.transform.position = hitPoint;
            // rotate the reticle to match the map's surface normal
            reticle.transform.rotation = Quaternion.LookRotation(hitInfo.Normal, Vector3.up);
        }        
    }
}

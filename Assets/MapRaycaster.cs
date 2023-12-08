using System.Collections;
using System.Collections.Generic;
using Microsoft.Maps.Unity;
using UnityEngine;
using Microsoft.Geospatial;

public class MapRaycaster : MonoBehaviour
{
    [SerializeField] private MapRenderer mapRenderer;
    [SerializeField] private GameObject reticleR;
    [SerializeField] private GameObject reticleL;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform rightHandRayOrigin;
    [SerializeField] private Transform leftHandRayOrigin;
    [SerializeField] private HandEvents rightHandEvents;
    [SerializeField] private HandEvents leftHandEvents;
    [SerializeField] private MapPin mapPinPrefab;
    [SerializeField] private MapPinLayer mapPinLayer;
    [SerializeField] private AudioSource placementSound;

    private void Start()
    {
        rightHandEvents._onPinch.AddListener(OnRightHandPinch);
        leftHandEvents._onPinch.AddListener(OnLeftHandPinch);
    }

    void Update()
    {
        // Raycast to position the reticles for each hand
        Ray ray = new Ray(rightHandRayOrigin.position, rightHandRayOrigin.forward);
        if (mapRenderer.Raycast(ray, out MapRendererRaycastHit hitInfo))
        {
            var hitPoint = hitInfo.Point;
            reticleR.SetActive(true);
            reticleR.transform.position = hitPoint;
            // rotate the reticle to match the map's surface normal
            reticleR.transform.rotation = Quaternion.LookRotation(hitInfo.Normal, Vector3.up);
        }
        else
        {
            reticleR.SetActive(false);
        }

        Ray ray2 = new Ray(leftHandRayOrigin.position, leftHandRayOrigin.forward);
        if (mapRenderer.Raycast(ray2, out MapRendererRaycastHit hitInfo2))
        {
            var hitPoint2 = hitInfo2.Point;
            reticleL.SetActive(true);
            reticleL.transform.position = hitPoint2;
            // rotate the reticle to match the map's surface normal
            reticleL.transform.rotation = Quaternion.LookRotation(hitInfo2.Normal, Vector3.up);
        }      
        else
        {
            reticleL.SetActive(false);
        }  
    }

    private void OnRightHandPinch()
    {
        Ray ray = new Ray(rightHandRayOrigin.position, rightHandRayOrigin.forward);
        if (mapRenderer.Raycast(ray, out MapRendererRaycastHit hitInfo))
        {
            var hitPoint = hitInfo.Point;
            // add a map pin at that lat/long
            var mapPin = Instantiate(mapPinPrefab);
            mapPin.transform.position = hitPoint;
            mapPin.Location = new LatLon(hitInfo.Location.LatitudeInDegrees, hitInfo.Location.LongitudeInDegrees);
            mapPinLayer.MapPins.Add(mapPin);

            // randomize the color of the mapPin
            var randomColor = new Color(Random.value, Random.value, Random.value);
            mapPin.transform.GetChild(0).GetComponent<Renderer>().material.color = randomColor;

            placementSound.transform.position = hitPoint;
            placementSound.Play();
        }
    }

    private void OnLeftHandPinch()
    {
        Ray ray = new Ray(leftHandRayOrigin.position, leftHandRayOrigin.forward);
        if (mapRenderer.Raycast(ray, out MapRendererRaycastHit hitInfo))
        {
            var hitPoint = hitInfo.Point;
            // add a map pin at that lat/long
            var mapPin = Instantiate(mapPinPrefab);
            mapPin.transform.position = hitPoint;
            mapPin.Location = new LatLon(hitInfo.Location.LatitudeInDegrees, hitInfo.Location.LongitudeInDegrees);
            mapPinLayer.MapPins.Add(mapPin);

            // randomize the color of the mapPin
            var randomColor = new Color(Random.value, Random.value, Random.value);
            mapPin.transform.GetChild(0).GetComponent<Renderer>().material.color = randomColor;

            placementSound.transform.position = hitPoint;
            placementSound.Play();
        }
    }

}

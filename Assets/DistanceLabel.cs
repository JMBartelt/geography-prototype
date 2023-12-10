using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.Maps.Unity;
public class DistanceLabel : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    // endpoint offset
    [SerializeField] private float verticalOffset = 0.0f;
    private List<MapPin> endpoints = new List<MapPin>();
    [SerializeField] private TextMeshProUGUI textMesh;
    private bool active = false;
    public void SetText(double distance)
    {
        textMesh.text = $"{distance:F2}m";
    }
    // overload of SetText that uses string
    public void SetText(string text)
    {
        textMesh.text = text;
    }
    public void SetEndpoints(MapPin position1, MapPin position2)
    {
        endpoints.Add(position1);
        endpoints.Add(position2);
        lineRenderer.SetPosition(0, position1.gameObject.transform.position + Vector3.up * verticalOffset);
        lineRenderer.SetPosition(1, position2.gameObject.transform.position + Vector3.up * verticalOffset);
        active = true;
    }

    public void SetLineColor(Color color)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        // change the Unlit material's base color
        lineRenderer.material.SetColor("_Color", color);
    }

    void Update()
    {
        if (!active) return;

        // if either MapPin is not active, hide the line renderer and text
        if (!endpoints[0].gameObject.activeSelf || !endpoints[1].gameObject.activeSelf)
        {
            lineRenderer.enabled = false;
            textMesh.transform.parent.gameObject.SetActive(false);
            return;
        }
        else
        {
            lineRenderer.enabled = true;
            textMesh.transform.parent.gameObject.SetActive(true);
        }
        // update endpoints
        lineRenderer.SetPosition(0, endpoints[0].gameObject.transform.position + Vector3.up * verticalOffset);
        lineRenderer.SetPosition(1, endpoints[1].gameObject.transform.position + Vector3.up * verticalOffset);
        // update text position
        textMesh.transform.parent.position = (endpoints[0].gameObject.transform.position + endpoints[1].gameObject.transform.position) / 2 + Vector3.up * verticalOffset;
    }
}

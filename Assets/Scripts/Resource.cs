using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public float minXResourcePosition = -40.0f;
    public float maxXResourcePosition = 40.0f;
    public float minZResourcePosition = -40.0f;
    public float maxZResourcePosition = 40.0f;

    private float resourcePosX = 0.0f;
    private float resourcePosZ = 0.0f;

    void putResourceToScene()
    {
        resourcePosX = Random.Range(minXResourcePosition, maxXResourcePosition);
        resourcePosZ = Random.Range(minZResourcePosition, maxZResourcePosition);
        this.transform.position = new Vector3(resourcePosX, 0.0f, resourcePosZ);
    }
}
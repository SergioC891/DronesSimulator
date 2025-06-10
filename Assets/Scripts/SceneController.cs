using System.Collections;
using UnityEngine;


public class SceneController : MonoBehaviour
{
    public GameObject UIControllerObj;

    public float resourceSpawnDelay = 1.0f;
    public int resourceAmount = 10;
    public int dronesAmount = 10;
    public string resourceGameObjectName = "Resource(Clone)";
    public string droneGameObjectName = "Drone(Clone)";

    private float droneSpeed = 1.0f;
    private bool respawnResources = false;


    public void startSimulation()
    {
        dronesAmount = UIControllerObj.GetComponent<UISettings>().getDrones() * 2;
        resourceSpawnDelay = UIControllerObj.GetComponent<UISettings>().getSpawnFreq();
        droneSpeed = UIControllerObj.GetComponent<UISettings>().getSpeed();

        StartCoroutine(createObjects());
    }

    public void stopSimulation()
    {
        activateDrones(false);
    }

    void activateDrones(bool activate, float droneSpeed = 1.0f)
    {
        GameObject[] drones = GameObject.FindGameObjectsWithTag("OnFlight");
        GameObject[] dronesOnBase = GameObject.FindGameObjectsWithTag("OnBase");
        string agentState = (activate) ? "startAgent" : "stopAgent";

        changeDronesAgentState(drones, agentState, droneSpeed);
        changeDronesAgentState(dronesOnBase, agentState, droneSpeed);
    }

    void changeDronesAgentState(GameObject[] drones, string agentState, float droneSpeed)
    {
        if (drones != null)
        {
            foreach (GameObject drone in drones)
            {
                drone.SendMessage(agentState, droneSpeed);
            }
        }
    }

    IEnumerator createObjects()
    {
        yield return new WaitForSeconds(1.0f);

        createDrones();

        StartCoroutine(createResources());
    }

    void createDrones()
    {
        dronesAmountCorrection();

        for (int j = 0; j < dronesAmount; j++)
        {
            createDrone();
        }

        activateDrones(true, droneSpeed);
    }

    IEnumerator createResources()
    {
        for (int i = 0; i < resourceAmount; i++)
        {
            yield return new WaitForSeconds(resourceSpawnDelay);
            createResource();
        }

        respawnResources = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (respawnResources)
        {
            respawnResources = false;

            StartCoroutine(createResources());
        }
    }

    void createResource()
    {
        GameObject resource = ObjectPool.SharedInstance.GetPooledObject(resourceGameObjectName);

        if (resource != null)
        {
            resource.SetActive(true);
            resource.tag = "free";
            resource.SendMessage("putResourceToScene", SendMessageOptions.DontRequireReceiver);
        }
    }

    void createDrone()
    {
        GameObject drone = ObjectPool.SharedInstance.GetPooledObject(droneGameObjectName);

        if (drone != null)
        {
            drone.SetActive(true);
            drone.SendMessage("resourceUpload", false);
            drone.SendMessage("putDroneToScene", SendMessageOptions.DontRequireReceiver);
        }
    }

    void dronesAmountCorrection()
    {
        GameObject[] drones = GameObject.FindGameObjectsWithTag("OnFlight");
        GameObject[] dronesOnBase = GameObject.FindGameObjectsWithTag("OnBase");
        int activeDronesAmount = drones.Length + dronesOnBase.Length;

        if (dronesAmount < activeDronesAmount)
        {
            int diff = activeDronesAmount - dronesAmount;

            for (int i = 0; i < diff; i++)
            {
                ObjectPool.SharedInstance.deactivateLastPooledObject(droneGameObjectName);
            }

            dronesAmount = 0;
        }
        else
        {
            dronesAmount -= activeDronesAmount;
        }
    }
}
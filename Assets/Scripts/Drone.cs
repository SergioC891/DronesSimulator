using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    UnityEngine.AI.NavMeshAgent droneAgent;

    static private bool droneBaseSide = false;
    static private bool freeResourceExists = false;

    public float minXDronePosition = -40.0f;
    public float maxXDronePosition = 40.0f;
    public float minZDronePosition = -40.0f;
    public float maxZDronePosition = 40.0f;

    public float distanceToResource = 3.0f;
    public float distanceToBase = 10.0f;

    private Transform goalTransform;
    private Transform baseTransform;

    private GameObject goalObj;
    private GameObject baseObj;

    private bool moveToBase = false;
    private bool moveToResource = false;
    private bool droneAgentActive = false;
    private bool waiting = false;

    private float dronePosX = 0.0f;
    private float dronePosZ = 0.0f;

    private GameObject UIControllerObj;
    private bool scanSpaceForFreeResource = true;

    // Start is called before the first frame update
    void Start()
    {
        determineBase();
        determineResource();
        StartCoroutine(activateAgent());

        UIControllerObj = GameObject.Find("UIController");
    }

    IEnumerator activateAgent()
    {
        float delay = Random.Range(.1f, .5f);

        yield return new WaitForSeconds(delay);

        droneAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        droneAgent.enabled = true;

        if (droneAgent.isOnNavMesh)
        {
            droneAgent.destination = goalTransform.position;
            droneAgentActive = true;
        }

        putDroneToScene();
    }

    void stopAgent(float droneSpeed = 3.5f)
    {
        droneAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        droneAgent.enabled = false;
        droneAgentActive = false;
    }

    void startAgent(float droneSpeed = 3.5f)
    {
        droneAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        droneAgent.enabled = true;
        droneAgentActive = true;
        droneAgent.speed = droneSpeed;
    }

    void determineBase()
    {
        if (droneBaseSide)
        {
            baseObj = GameObject.Find("Base2");
            droneBaseSide = false;
        }
        else
        {
            baseObj = GameObject.Find("Base1");
            droneBaseSide = true;
        }
    }

    void determineResource()
    {
        GameObject[] resourceObjs = GameObject.FindGameObjectsWithTag("free");

        if (resourceObjs.Length > 0)
        {
            foreach (GameObject resource in resourceObjs)
            {
                if (resource.activeSelf)
                {
                    goalObj = resource;
                    resource.tag = "busy";
                    break;
                }
            }
        }

        if (goalObj != null)
        {
            goalTransform = goalObj.transform;
            moveToResource = true;
            moveToBase = false;
        }
        else
        {
            goalTransform = baseObj.transform;
            moveToBase = true;
            moveToResource = false;
        }

        baseTransform = baseObj.transform;
    }

    void putDroneToScene()
    {
        dronePosX = Random.Range(minXDronePosition, maxXDronePosition);
        dronePosZ = Random.Range(minZDronePosition, maxZDronePosition);
        this.transform.position = new Vector3(dronePosX, 0.0f, dronePosZ);
    }

    void resourceUpload(bool upload = false)
    {
        this.gameObject.GetComponent<ParticleSystem>().enableEmission = upload;
    }

    void makeResourceFree()
    {
        if (goalObj != null)
        {
            StartCoroutine(resourceUploadProcess());
        }
    }

    IEnumerator resourceUploadProcess()
    {
        resourceUpload(true);

        int baseIndex = (baseObj.name == "Base1") ? 1 : 2;
        UIControllerObj.GetComponent<UISettings>().increaseBaseCounter(baseIndex);

        yield return new WaitForSeconds(2.0f);

        goalObj.tag = "free";
        goalObj = null;
        this.gameObject.tag = "OnBase";

        resourceUpload(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (droneAgentActive)
        {
            droneMoveLogic();
        }
    }

    void droneMoveLogic()
    {
        if (droneAgent.remainingDistance < distanceToResource && moveToResource && !waiting)
        {
            StartCoroutine(getResource());
        }
        else if (droneAgent.remainingDistance < distanceToBase && moveToBase && !waiting)
        {
            makeResourceFree();

            StartCoroutine(determineNextGoal());
        }

        if (scanSpaceForFreeResource)
        {
            scanSpaceForFreeResource = false;
            scanSpaceForResources();
        }

        if (this.gameObject.tag == "OnBase" && freeResourceExists)
        {
            determineResource();

            if (goalObj != null)
            {
                freeResourceExists = false;
                this.gameObject.tag = "OnFlight";
                moveToResource = true;
                moveToBase = false;
            }
        }
    }

    void scanSpaceForResources()
    {
        GameObject[] resources = GameObject.FindGameObjectsWithTag("free");
        if (resources.Length > 0)
        {
            freeResourceExists = true;
        }

        StartCoroutine(delayForRescanSpace());
    }

    IEnumerator delayForRescanSpace()
    {
        yield return new WaitForSeconds(1.0f);

        scanSpaceForFreeResource = true;
    }

    IEnumerator determineNextGoal()
    {
        waiting = true;

        yield return new WaitForSeconds(2.0f);

        if (droneAgentActive)
        {
            determineResource();

            droneAgent.destination = goalTransform.position;
        }

        waiting = false;
    }

    IEnumerator getResource()
    {
        waiting = true;
        
        yield return new WaitForSeconds(2.0f);

        if (goalObj != null && goalObj.activeSelf)
        {
            goalObj.SetActive(false);
        }
        else
        {
            this.gameObject.tag = "OnBase";
        }

        if (droneAgentActive)
        {
            droneAgent.destination = baseTransform.position;
            moveToBase = true;
            moveToResource = false;
        }

        waiting = false;
    }
}

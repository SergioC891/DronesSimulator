using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISettings : MonoBehaviour
{
    public GameObject SceneControllerObj;
    public GameObject SimulationStartButtonText;

    public float MinSpawnFrequency = 1.0f;
    public float MaxSpawnFrequency = 50.0f;

    private int drones = 1;
    private float speed = 1.0f;
    private float resourcesSpawnFrequency = 5.0f;
    private bool showPath = false;
    private bool simulationStarted = false;

    public int getDrones()
    {
        return drones;
    }

    public float getSpeed()
    {
        return speed;
    }

    public float getSpawnFreq()
    {
        return resourcesSpawnFrequency;
    }

    public bool getShowPath()
    {
        return showPath;
    }

    public bool getSimulationStarted()
    {
        return simulationStarted;
    }

    public void OnDronesSliderValue(float value)
    {
        drones = (int) value;
    }

    public void OnSpeedSliderValue(float value)
    {
        speed = value;
    }

    public void OnSpawnInputFieldEndEdit(string value)
    {
        float.TryParse(value, out resourcesSpawnFrequency);

        if (resourcesSpawnFrequency < MinSpawnFrequency)
        {
            resourcesSpawnFrequency = MinSpawnFrequency;
        }
        else if (resourcesSpawnFrequency > MaxSpawnFrequency)
        {
            resourcesSpawnFrequency = MaxSpawnFrequency;
        }
        else if (resourcesSpawnFrequency > MinSpawnFrequency && resourcesSpawnFrequency < MaxSpawnFrequency)
        {
            resourcesSpawnFrequency = resourcesSpawnFrequency;
        }
        else
        {
            resourcesSpawnFrequency = (MinSpawnFrequency + MaxSpawnFrequency) / 2;
        }
    }

    public void OnPathToggle(bool value)
    {
        showPath = value;
    }

    public void OnStartSimulationButtonClick()
    {
        simulationStarted = !simulationStarted;

        if (simulationStarted)
        {
            SceneControllerObj.GetComponent<SceneController>().startSimulation();

            SimulationStartButtonText.GetComponent<Text>().text = "Stop Drones Simulator";
        }
        else 
        {
            SceneControllerObj.GetComponent<SceneController>().stopSimulation();

            SimulationStartButtonText.GetComponent<Text>().text = "Start Drones Simulator";
        }
    }

    public void increaseBaseCounter(int baseIndex)
    {
        if (baseIndex > 0)
        {
            GameObject baseScoreCounter = GameObject.Find("Base" + baseIndex + "CounterText");

            string curScoreText = baseScoreCounter.GetComponent<Text>().text;
            int resourceScore = 0;
            int.TryParse(curScoreText, out resourceScore);
            resourceScore++;

            baseScoreCounter.GetComponent<Text>().text = resourceScore.ToString();
        }
    }
}

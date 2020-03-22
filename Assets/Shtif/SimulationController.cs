using System.Collections;
using System.Collections.Generic;
using RuntimeInspectorNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimulationController : MonoBehaviour
{
    public SimulationSettings settings;

    public GraphController graph;
    
    public RuntimeInspector inspector;
    
    public EntityManager entityManager;
    
    public Button simulateButton;
    public TMP_Text simulateButtonText;

    public TMP_Text cleanValue;
    public TMP_Text infectedValue;
    public TMP_Text curedValue;
    public TMP_Text deadValue;
    public TMP_Text timeValue;
    public TMP_Text stateValue;

    public RawImage disableInspector;
    
    private bool _running = false;

    public bool IsRunning()
    {
        return _running;
    }
    
    private float _startupTime = 0;

    public int time { get; private set; } = 0;

    public int infected
    {
        get { return Mathf.RoundToInt(entityManager.GetHealthStatePercentage(HealthState.INFECTED)); }
    }
    
    public int quarantined
    {
        get { return Mathf.RoundToInt(entityManager.GetHealthStatePercentage(HealthState.QUARANTINED)); }
    }
    
    public int dead
    {
        get { return Mathf.RoundToInt(entityManager.GetHealthStatePercentage(HealthState.DEAD)); }
    }
    
    public int cured
    {
        get { return Mathf.RoundToInt(entityManager.GetHealthStatePercentage(HealthState.CURED)); }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        disableInspector.enabled = false;

        simulateButton.onClick.AddListener(SimulateButtonClicked);
        
        inspector.Inspect(settings);
    }

    void Update()
    {
        cleanValue.text = Mathf.RoundToInt(entityManager.GetHealthStatePercentage(HealthState.CLEAN)) + "%";
        infectedValue.text = Mathf.RoundToInt(entityManager.GetHealthStatePercentage(HealthState.INFECTED))+Mathf.RoundToInt(entityManager.GetHealthStatePercentage(HealthState.QUARANTINED)) + "%";
        curedValue.text = Mathf.RoundToInt(entityManager.GetHealthStatePercentage(HealthState.CURED)) + "%";
        deadValue.text = Mathf.RoundToInt(entityManager.GetHealthStatePercentage(HealthState.DEAD)) + "%";
        
        if (_running)
        {
            stateValue.text = "Running";
            stateValue.color = Color.green;
            time = Mathf.FloorToInt(Time.realtimeSinceStartup - _startupTime);
        }
        else
        {
            stateValue.text = "Stopped";
            stateValue.color = Color.red;
        }
        timeValue.text = time + " Days";

        if (entityManager.GetHealthStatePercentage(HealthState.INFECTED) == 0)
        {
            Stop();
        }
    }

    void Stop()
    {
        entityManager.Stop();
        disableInspector.enabled = false;
        _running = false;
        inspector.enabled = true;
        
        simulateButtonText.text = "Simulate";
    }

    void Simulate()
    {
        disableInspector.enabled = true;
        inspector.enabled = false;
        _running = true;
        _startupTime = Time.realtimeSinceStartup;
        entityManager.Generate();
        
        simulateButtonText.text = "Stop";
    }

    void SimulateButtonClicked()
    {
        graph.Clear();

        if (!_running)
        {
            Simulate();
        }
        else
        {
            Stop();
        }
    }
}

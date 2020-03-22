using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public SimulationController simulation;
    public EntityController entityPrefab;
    
    public float boundaryLeft { get; private set; }
    public float boundaryRight { get; private set; }
    public float boundaryTop { get; private set; }
    public float boundaryBottom { get; private set; }
    
    public List<EntityController> quarantined { get; private set; }

    public List<EntityController> entities { get; private set; }

    private RectTransform _rt;

    private int _currentDay = 0;

    public float GetHealthStatePercentage(HealthState p_state)
    {
        int count = 0;
        foreach (EntityController entity in entities)
        {
            if (entity.HasState(p_state)) count++;
        }
        
        return (entities.Count == 0) ? 0 : 100f * count / entities.Count;
    }

    private void CalculateBoundary()
    {
        boundaryLeft = _rt.offsetMin.x;
        boundaryRight = Screen.width + _rt.offsetMax.x;
        boundaryTop = Screen.height + _rt.offsetMax.y;
        boundaryBottom = _rt.offsetMin.y * Screen.height/768;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        quarantined = new List<EntityController>();
        entities = new List<EntityController>();
        
        _rt = GetComponent<RectTransform>();
        CalculateBoundary();
    }

    public void Stop()
    {
        foreach (EntityController entity in entities)
        {
            entity.Stop();
        }
    }

    private void Update()
    {
        CalculateBoundary();

        if (_currentDay != simulation.time && simulation.IsRunning())
        {
            Test();
            _currentDay = simulation.time;
        }
    }
    
    private void Test() 
    {
        for (int i = 0; i < simulation.settings.testingCount; ++i)
        {
            int index = Random.Range(0, entities.Count);
            while (entities[index].HasState(HealthState.QUARANTINED))
            {
                index = Random.Range(0, entities.Count);
            }

            if (entities[index].Test())
            {
                quarantined.Add(entities[index]);
            }
        }
    }

    public void Generate()
    {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
        
        entities = new List<EntityController>();
        
        for (var i = 0; i < simulation.settings.entityCount; ++i)
        {
            EntityController entity = Instantiate(entityPrefab, transform);
            entity.manager = this;
            entities.Add(entity);

            if (i < simulation.settings.initialInfectionCount)
            {
                entity.stationary = false;
                entity.Infect(true);
            } else if (i-simulation.settings.initialInfectionCount < simulation.settings.entityCount * simulation.settings.stationaryRate / 100f)
            {
                entity.stationary = true;
            }
        }
    }
}
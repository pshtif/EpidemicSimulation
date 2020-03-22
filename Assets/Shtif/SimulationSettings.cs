using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationSettings : MonoBehaviour
{
    [Range(0, 100)] public int stationaryRate = 10;

    [Range(0, 50)] public int testingCount = 1;
    
    [Range(10,1000)] public int entityCount = 1000;
    
    [Range(0,100)] public int infectionProbability = 50;

    [Range(1, 50)] public float initialInfectionCount = 1;

    [Range(0, 10)] public int infectionRadius = 0;

    [Range(0, 100)] public int fatalityRate = 3;

    [Range(0, 30)] public int averageDeathTime = 14;

    [Range(0, 30)] public float averageRecoveryTime = 21;
}

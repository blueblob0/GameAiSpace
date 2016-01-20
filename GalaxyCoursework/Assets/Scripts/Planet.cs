using UnityEngine;
using System.Collections.Generic;

public class Planet : Satalite
{
    int numPlanets;
    public List<GameObject> planets = new List<GameObject>();
    public List<float> spheres = new List<float>();
    public string planetPrefabName = "MoonPrefab";
    const float dist = 0.6f;
    const float minDis = 0.3f;



    // Use this for initialization
    void Start () {
        numPlanets = Random.Range(0, 4);
        speed = Random.Range(10,35);
        float hold = 0;
        for (int i = 0; i < numPlanets; i++)
        {
            planets.Add(SpawnSatalite(hold, minDis, dist, planetPrefabName));
            hold = planets[i].GetComponent<Satalite>().distPlanet;

        }
    }
	
	// Update is called once per frame
	
}

using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class Star : CelestialBody
{
    int numPlanets;
    public List<GameObject> planets = new List<GameObject>();
    public List<float> spheres = new List<float>();
    public string planetPrefabName = "PlanetPrefab";
    const float dist = 30f;
    const float minDis = 15f;
    // Use this for initialization
   protected override void Start()
    {

        mass = 1.989f * Mathf.Pow(10, 30); // just putting stars as the mass of the sun for now 
        // the number of planets can be between 0 and 12 ( for now)
        // 40% are between 8 and 10 20% 11 or 12,  20% 5 6 7, 432 12% 1 6% 0   2%
        numPlanets = Random.Range(0, 100);
       
        if (numPlanets < 2)
        {
            numPlanets = 0;
        }
        else if (numPlanets < 8)
        {
            numPlanets = 1;
        }
        else if (numPlanets < 20)
        {
            numPlanets = Random.Range(2, 5); //432
        }
        else if (numPlanets < 40)
        {
            numPlanets = Random.Range(5, 8); //567
        }
        else if (numPlanets < 90)
        {
            numPlanets = Random.Range(8, 11); //8910
        }
        else if (numPlanets < 100)
        {
            numPlanets = Random.Range(11, 14); //11 12 13
        }

        float hold = transform.localScale.x / 2;
        //Debug.Log(hold);
        //hold -= minDis;
        //Debug.Log(hold);
        for (int i = 0; i < numPlanets; i++)
        {

            GameObject plan = SpawnSatalite(hold, minDis, dist, planetPrefabName);

            planets.Add(plan);
            hold = plan.GetComponent<Satalite>().distPlanet;

        }



    }

    // Update is called once per frame
    protected override void Update()
    {







    }


    void OnDrawGizmos()
    {

        foreach (float f in spheres)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, f);

        }


    }
}

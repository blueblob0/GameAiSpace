using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class Star : CelestialBody
{
    int numPlanets;
    public List<GameObject> planets = new List<GameObject>();
    public List<float> spheres = new List<float>();
    public string planetPrefabName = "PlanetPrefab";
    const float dist = 3.5f;
    const float minDis = 1.5f;
    // Use this for initialization
    void Start()
    {
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
        else if (numPlanets < 80)
        {
            numPlanets = Random.Range(8, 11); //8910
        }
        else if (numPlanets < 100)
        {
            numPlanets = Random.Range(11, 14); //11 12 13
        }

        float hold = 0;
        for (int i = 0; i < numPlanets; i++)
        {
            planets.Add(SpawnSatalite(hold,minDis,dist,planetPrefabName));
            hold = planets[i].GetComponent<Satalite>().distPlanet;

        }



    }
    /*
    float SpawnPlanet(float MoveAmount)
    {
        bool move2d = true;
        // star by moving out a bit from the planet 
        float move = MoveAmount + Random.Range(minDis, dist);
        Debug.Log(move);
        spheres.Add(move);
        Vector3 starPos;
        if (move2d)
        {
            starPos = Random.insideUnitCircle.normalized * move;
            //Vector3 starPos = Vector3.one * move;
            starPos.z = starPos.y;
            starPos.y = 0;
        }
        else
        {
            starPos = Random.onUnitSphere * move;
        }

        //Debug.Log(MoveAmount + " " + move + " " + Vector3.Distance(Vector3.zero, starPos));
        GameObject a = Instantiate(Resources.Load(planetPrefabName)) as GameObject;
        a.GetComponent<Planet>().myStar = gameObject;
        a.name = MoveAmount.ToString();
        a.transform.SetParent(gameObject.transform);
        planets.Add(a);
        a.transform.localPosition = starPos;

        return move;
    }
    */
    // Update is called once per frame
    void Update()
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

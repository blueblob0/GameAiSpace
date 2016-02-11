using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class Star: CelestialBody
{
    public int numPlanets;
    public GameObject[] planets;// = new List<GameObject>();
    public SataliteDetails[] planetsLoc;// = new List<SataliteDetails>();
    public GameObject miniSun;
    private bool planetsSpawned;
    public List<float> spheres = new List<float>();
    public string planetPrefabName = "PlanetPrefab";
    const float dist = 0.04f;
    const float minDis = 0.02f;
    public float angle;
    public float spiralAngel;
    // Use this for initialization
    protected override void Start()
    {
        miniSun.SetActive(false);
        base.Start();
        planetsSpawned = false;
        //mass = 100;
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
       // Debug.Log(numPlanets);
        planetsLoc = new SataliteDetails[numPlanets];
        float hold = transform.lossyScale.x / 2;
        hold = 0.05f;
        for (int i = 0; i < numPlanets; i++)
        {
           
            planetsLoc[i] = SataliteLocation(hold,  minDis, dist);
            hold = planetsLoc[i].distFromBody;
        }

        

    }
   
    // Update is called once per frame


    public void IncreaseMass(int plus)
    {
        mass += plus;
        transform.localScale = Vector3.one * mass * CreateGalaxy.starMuti;
    }

    public void SetMass(int newMass)
    {
        mass = newMass;
        transform.localScale = Vector3.one * mass * CreateGalaxy.starMuti;
    }



    void OnDrawGizmos()
    {
        foreach (float f in spheres)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, f);
        }        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag =="MainCamera")
        {
            Debug.Log("hit");
            miniSun.SetActive(true);
            Color c = GetComponent<Renderer>().material.color;
            c.a = 0;
            GetComponent<Renderer>().material.color = c;
            if (planetsSpawned)
            {
                for(int i = 0; i < planets.Length;i++)
                {
                    planets[i].SetActive(true);
                }
            }
            else
            {
                planets = new GameObject[planetsLoc.Length];
                
                for (int i = 0; i < planetsLoc.Length; i++)
                {
                    planets[i] = SpawnSatalite(planetsLoc[i], planetPrefabName);                  
                }
                planetsSpawned = true;
            }
        }
        else if (other.GetComponent<Star>())
        {
            if (!controler)
            {
                controler = FindObjectOfType<CreateGalaxy>();
            }
            int massa = other.GetComponent<Star>().Mass;
            if (massa <= mass)
            {
                controler.stars.Remove(other.gameObject);
                IncreaseMass(massa);
                Destroy(other.gameObject);
                // Debug.Log(mass);
            }
        }
        
    }



    void OnTriggerExit(Collider other)
    {
        if (other.tag == "MainCamera")
        {
            miniSun.SetActive(false);
            Color c = GetComponent<Renderer>().material.color;
            c.a = 1;
            GetComponent<Renderer>().material.color = c;
            for (int i = 0; i < planets.Length; i++)
            {
                planets[i].SetActive(false);
            }
        }


    }



}

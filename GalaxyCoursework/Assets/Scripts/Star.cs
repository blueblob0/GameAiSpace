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
    //public float angle;
    //public float spiralAngel;
    // Use this for initialization
    protected override void Start()
    {
        if(CreateGalaxy.removeName == name)
        {
            return;
        }

        miniSun.SetActive(false);
        base.Start();
        planetsSpawned = false;
        //mass = 100;      

        //for now do this now will change later 
        GeneratePlanets();
    }
   

    /// <summary>
    /// Function for making planets this is called by the genration algrithum when the size of the sun is set
    /// </summary>
    public void GeneratePlanets()
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
        Vector2 circlePos = Random.insideUnitCircle.normalized;
        circlePos = Vector2.one;
        for (int i = 0; i < numPlanets; i++)
        {

            //planetsLoc[i] = SataliteLocation(hold,  minDis, dist);
            planetsLoc[i] = SataliteLocation(hold, minDis, dist, circlePos);

            hold = planetsLoc[i].distFromBody;
        }



    }

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


    /*
    void OnDrawGizmos()
    {
        foreach (float f in spheres)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, f);
        }        
    }
    */
    void OnTriggerEnter(Collider other)
    {
        //if its going to be removed the star should have no effect  
        if (CreateGalaxy.removeName == name)
        {
            return;
        }
        if (other.tag == "MainCamera")
        {
            //Decrease Speed for moving around soloar system
            other.GetComponent<CameraMove>().DecreaseSpeed();

            // other.GetComponent<BoxCollider>().size = new Vector3(1, 1, 1f);
            //other.GetComponent<BoxCollider>().center = new Vector3(0, 0, 0.5f);

           // Debug.Log("hit");
            miniSun.SetActive(true);
            Color c = GetComponent<Renderer>().material.color;
            c.a = 0;
            GetComponent<Renderer>().material.color = c;
            if (planetsSpawned)
            {
                for (int i = 0; i < planets.Length; i++)
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
        /*else if (other.GetComponent<Star>())
        {
            if (!controler)
            {
                controler = FindObjectOfType<CreateGalaxy>();
            }
            int massa = other.GetComponent<Star>().mass;
            if (massa <= mass)
            {
                controler.DestroyStar(other.gameObject);
                IncreaseMass(massa);
            }
        }
        */
        Debug.Log("thing should not hit");
        
    }
    

    void OnTriggerExit(Collider other)
    {
        if (CreateGalaxy.removeName == name)
        {
            return;
        }
        if (other.tag == "MainCamera")
        {
            miniSun.SetActive(false);
            //increase speed for moving around galxy 
            other.GetComponent<CameraMove>().IncreaseSpeed();
            //other.GetComponent<BoxCollider>().size = new Vector3(1, 1, 20f);
            //other.GetComponent<BoxCollider>().center = new Vector3(0, 0, 10f);
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

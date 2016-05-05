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
    //const float dist = 0.05f;//
    const float dist = CreateGalaxy.starMuti;
   // const float minDis = 0.05f;//CreateGalaxy.starMuti;
    const float minDis = CreateGalaxy.starMuti;
    //public float angle;
    //public float spiralAngel;
    // Use this for initialization
    protected override void Start()
    {       
        miniSun.SetActive(false);
        miniSun.transform.SetParent(null);
        miniSun.transform.localScale = Vector3.one * CreateGalaxy.starMuti *2;
        miniSun.transform.SetParent(transform);
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
        int maxplanets = 9; //Mathf.RoundToInt((transform.lossyScale.x /CreateGalaxy.starMuti*2)- (CreateGalaxy.starMuti * 2));

        Debug.Log((transform.lossyScale.x / CreateGalaxy.starMuti * 2) - (CreateGalaxy.starMuti * 2));

        do
        {
            planetsNum();
        } while (numPlanets > maxplanets);
        numPlanets = maxplanets;
        // Debug.Log(numPlanets);
        planetsLoc = new SataliteDetails[numPlanets];
        float hold = CreateGalaxy.starMuti;
        
        Vector2 circlePos;
        circlePos = Vector2.one;
        for (int i = 0; i < numPlanets; i++)
        {          
            //circlePos = Random.insideUnitCircle.normalized;
            //planetsLoc[i] = SataliteLocation(hold,  minDis, dist);
            planetsLoc[i] = SataliteLocation(hold, minDis, dist, circlePos);
            hold = planetsLoc[i].distFromBody + minDis;
        }
    }


    public int planetsNum()
    {

        numPlanets = Random.Range(0, 100);

        if (numPlanets < 2) //2%
        {
            numPlanets = 0;
        }
        else if (numPlanets < 10) //8%
        {
            numPlanets = 1;
        }
        else if (numPlanets < 30)  //20%
        {
            numPlanets = Random.Range(2, 5); //432
        }
        else if (numPlanets < 70) //40%
        {
            numPlanets = Random.Range(5, 8); //567
        }
        else if (numPlanets < 100) //30%
        {
            numPlanets = Random.Range(8, 9); //8910
        }

        return numPlanets;
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


    /// <summary>
    /// spawing satalite planets around the sun 
    /// </summary>
    /// <param name="starPos">The stars position info</param>
    /// <param name="prefabName">String of th prefab to spawn</param>
    /// <returns> the satalite spawned</returns>
    protected override GameObject SpawnSatalite(SataliteDetails starPos, string prefabName)
    {
        GameObject a = Instantiate(Resources.Load(prefabName)) as GameObject;
        Planet holds = a.GetComponent<Planet>();
        holds.orbitingBody = gameObject;
        holds.distPlanet = starPos.distFromBody;
        a.name = starPos.distFromBody.ToString();
        a.transform.SetParent(gameObject.transform);

        a.transform.position = gameObject.transform.position;

        a.transform.position+= starPos.location;

        holds.SetBiomes();

        return a;
    }

    void OnTriggerEnter(Collider other)
    {
        
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
       
        Debug.Log("thing should not hit");
        
    }
    

    void OnTriggerExit(Collider other)
    {
        
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

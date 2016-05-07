//script made by: up651590
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class Star: CelestialBody
{
    //public int[] numPlanets;
    PlanetsInfo[] planetOnStar; //used to hold planets on each star, so that multistar systems have planet around rach star 

    private bool planetsSpawned;
    public List<float> spheres = new List<float>();
    public string planetPrefabName = "PlanetPrefab";
    //const float dist = 0.05f;//
    const float dist = CreateGalaxy.planetMuti*2;
   // const float minDis = 0.05f;//CreateGalaxy.starMuti;
    const float minDis = CreateGalaxy.planetMuti;

    public starType typeOfStar;

    private bool assignedVarables = false;

    //varables for twinStars
    public GameObject[] miniStars;
    public GameObject[] bigStars;
    public Renderer[] theRend;


    //public float angle;
    //public float spiralAngel;
    // Use this for initialization
    protected override void Start()
    {
        if (!assignedVarables)
        {
            AssignVarables();
        }
        base.Start();
        planetsSpawned = false;
        //mass = 100;      
        
        foreach(PlanetsInfo pla in planetOnStar)
        {

            GeneratePlanets(pla);

        }

        
    }

    public void AssignVarables()
    {
        assignedVarables = true;
        theRend = new Renderer[bigStars.Length];
        for (int i = 0; i < bigStars.Length; i++)
        {
            theRend[i] = bigStars[i].GetComponent<Renderer>();
        }

        for (int i = 0; i < miniStars.Length; i++)
        {
            miniStars[i].SetActive(false);
            
            miniStars[i].transform.SetParent(null);
            
            if (typeOfStar == starType.Neutron)
            {
                miniStars[i].transform.localScale = Vector3.one * (CreateGalaxy.starMuti / 10);
            }
            else
            {
                miniStars[i].transform.localScale = Vector3.one * (CreateGalaxy.planetMuti + (CreateGalaxy.planetMuti / 2));
                
            }
            miniStars[i].transform.SetParent(bigStars[i].transform);
        }


        if(starType.BinaryStar == typeOfStar)
        {
            planetOnStar = new PlanetsInfo[2];
            
        }
        else if (starType.Ternarystar == typeOfStar)
        {
            planetOnStar = new PlanetsInfo[3];
        }
        else
        {
            planetOnStar = new PlanetsInfo[1];
        }

        for (int i =0;i< planetOnStar.Length;i++)
        {
            planetOnStar[i] = new PlanetsInfo();

        }



    }


    /// <summary>
    /// Function for making planets this is called by the genration algrithum when the size of the star is set
    /// </summary>
    public void GeneratePlanets(PlanetsInfo planets)
    {
        // the number of planets can be between 0 and 12 ( for now)
        // 40% are between 8 and 10 20% 11 or 12,  20% 5 6 7, 432 12% 1 6% 0   2%
        int maxplanets = Mathf.RoundToInt(((transform.lossyScale.x- (CreateGalaxy.planetMuti * 2)) / CreateGalaxy.planetMuti) * 2)/10; //Mathf.RoundToInt((transform.lossyScale.x /CreateGalaxy.starMuti*2)- (CreateGalaxy.starMuti * 2));
        int numPlanets = 0;
        if (maxplanets > 0)
        {
            do
            {
                numPlanets = planetsNum();
            } while (numPlanets > maxplanets);
        }

        //Debug.Log(planets);

        
        planets.planetsLoc = new SataliteDetails[numPlanets];
        float hold = CreateGalaxy.starMuti;
        
        Vector2 circlePos;
        circlePos = Vector2.one;
        for (int i = 0; i < planets.planetsLoc.Length; i++)
        {          
            circlePos = Random.insideUnitCircle.normalized;
            //planetsLoc[i] = SataliteLocation(hold,  minDis, dist);
            planets.planetsLoc[i] = SataliteLocation(hold, minDis, dist, circlePos, WorkOutLife(i));
            hold = planets.planetsLoc[i].distFromBody + minDis;
        }
    }

    /// <summary>
    /// returns the max number of planets fopr a star
    /// </summary>
    /// <returns></returns>
    private int MaxPlanet()
    {
        if(typeOfStar == starType.Neutron) //neutron stars have a very small change to have a planet around them 
        {
            int testPlanet = Random.Range(0, 100);
            if (testPlanet < 99)
            {
                return 0;
            }
            else
            {
                return 1;
            }
            
        }
        else
        {
            return Mathf.RoundToInt(((transform.lossyScale.x - (CreateGalaxy.planetMuti * 2)) / CreateGalaxy.planetMuti) * 2) / 10;
        }

    }



    /// <summary>
    /// used to work out if a planet can have life
    /// </summary>
    private bool WorkOutLife(int planetNum)
    {
        if (planetNum >2 && planetNum < 5)
        {
            Debug.Log("make life random");
            return true;
        }
        return false;
    }


    public int planetsNum()
    {

       int  numPlanets = Random.Range(0, 100);

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
        SetScale();
    }

   

    protected override void SetScale()
    {
        if (typeOfStar == starType.Neutron)
        {
            transform.localScale = Vector3.one * CreateGalaxy.starMuti * (CreateGalaxy.starMuti/10);
        }else if (typeOfStar == starType.BinaryStar|| typeOfStar == starType.Ternarystar)
        {
            transform.localScale = Vector3.one * mass * CreateGalaxy.starMuti/2;
        }
        else 
        {
            transform.localScale = Vector3.one * mass * CreateGalaxy.starMuti;
        }


    }
   

    /// <summary>
    /// spawing satalite planets around the star 
    /// </summary>
    /// <param name="starPos">The stars position info</param>
    /// <param name="prefabName">name of the prefab to spawn</param>
    /// <returns>the satalite spawned</returns>
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
            other.GetComponent<CameraMove>().DecreaseSpeedStar();
            // other.GetComponent<BoxCollider>().size = new Vector3(1, 1, 1f);
            //other.GetComponent<BoxCollider>().center = new Vector3(0, 0, 0.5f);
            // Debug.Log("hit");
            for(int  i = 0; i < miniStars.Length; i++)
            {
                miniStars[i].SetActive(true);
            }

            
            
            if (planetsSpawned)
            {
                foreach (PlanetsInfo pla in planetOnStar)
                {
                    for (int i = 0; i < pla.planets.Length; i++)
                    {
                        pla.planets[i].SetActive(true);
                    }
                }               
            }
            else
            {
                foreach (PlanetsInfo pla in planetOnStar)
                {

                    pla.planets = new GameObject[pla.planetsLoc.Length];

                    for (int i = 0; i < pla.planetsLoc.Length; i++)
                    {
                        pla.planets[i] = SpawnSatalite(pla.planetsLoc[i], planetPrefabName);
                    }
                }
                planetsSpawned = true;
            }
            StopAllCoroutines(); // use this to stop the current fade if any
            for (int i = 0; i < theRend.Length; i++)
            {
                StartCoroutine(ReduceAlpha(theRend[i]));
            }
        }
       
       // Debug.Log("thing should not hit");
        
    }


    //used to reduce the alpha of the star when the player moves in
    IEnumerator ReduceAlpha( Renderer starRend)
    {
        


        while (starRend.material.color.a > 0f)
        {
            Color c = starRend.material.color;
            c.a -= 0.1f;
            //Debug.Log(theRend.material.color.a);
            starRend.material.color = c;
            yield return new WaitForSeconds(0.1f);
        }
    }

    //used to increase the alpha of the star when the player moves out
    IEnumerator IncreaseAlpha(Renderer starRend)
    {
        while (starRend.material.color.a < 1f)
        {
            Color c = starRend.material.color;
            c.a += 0.1f;
            //Debug.Log(theRend.material.color.a);
            starRend.material.color = c;
            yield return new WaitForSeconds(0.1f);
        }

        for (int i = 0; i < miniStars.Length; i++)
        {
            miniStars[i].SetActive(false);
        }
        foreach (PlanetsInfo pla in planetOnStar)
        {
            for (int i = 0; i < pla.planets.Length; i++)
            {
                pla.planets[i].SetActive(false);
            }
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        
        if (other.tag == "MainCamera")
        {
            
            //increase speed for moving around galxy 
            other.GetComponent<CameraMove>().IncreaseStarSpeed();
            //other.GetComponent<BoxCollider>().size = new Vector3(1, 1, 20f);
            //other.GetComponent<BoxCollider>().center = new Vector3(0, 0, 10f);
            StopAllCoroutines(); // use this to stop the current fade if any

            for (int i = 0; i < theRend.Length; i++)
            {
                StartCoroutine(IncreaseAlpha(theRend[i]));
            }

            
        }

    }





}

/// <summary>
/// for storing the info of all the planets around a star(so that binary and ternary stars have planets)
/// </summary>
public class PlanetsInfo
{

    public GameObject[] planets;// = new List<GameObject>();
    public SataliteDetails[] planetsLoc;// = new List<SataliteDetails>();
    



}
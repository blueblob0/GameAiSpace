//script made by: up651590
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class Star: CelestialBody
{    
    PlanetsInfo[] planetsOnStar; //used to hold planets on each star, so that multistar systems have planets around each star 
    public Text planetListText;
    private bool planetsSpawned;
    public List<float> spheres = new List<float>();
    public string planetPrefabName = "PlanetPrefab";
    public string twinPlanetPrefabName= "TwinPlanetPrefab";
    public string asteroidPrefabName = "AsteroidPrefab";
    const float dist = CreateGalaxy.planetMuti;
    const float minDis = CreateGalaxy.planetMuti*0.75f;

    public starType typeOfStar;

    private bool assignedVarables = false;

    //varables for twinStars
    public GameObject[] miniStars;
    public GameObject[] bigStars;
    public Renderer[] theRend;

    public GameObject[] asteroids;

    
     
    protected override void Start()
    {
        if (!assignedVarables)
        {
            AssignVarables();
        }
        base.Start();
        planetsSpawned = false;
        
        for (int i=0;i< planetsOnStar.Length;i++)
        {           
            GeneratePlanets(planetsOnStar[i], bigStars[i].transform);            
        }


    }

    /// <summary>
    /// used to set key Varables that might need to be set before stat has run
    /// </summary>
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
                miniStars[i].transform.localScale = Vector3.one * (CreateGalaxy.planetMuti *1.5f);
                
            }
            miniStars[i].transform.SetParent(bigStars[i].transform);
        }
        if(starType.BinaryStar == typeOfStar)
        {
            planetsOnStar = new PlanetsInfo[2];
            
        }
        else if (starType.Ternarystar == typeOfStar)
        {
            planetsOnStar = new PlanetsInfo[3];
        }
        else
        {
            planetsOnStar = new PlanetsInfo[1];
        }

        for (int i =0;i< planetsOnStar.Length;i++)
        {
            planetsOnStar[i] = new PlanetsInfo();

        }
    }


    /// <summary>
    /// Function for setting the planets location around its star 
    /// </summary>
    public void GeneratePlanets(PlanetsInfo planets, Transform parentStar)
    {
        int maxplanets = MaxPlanet( parentStar);

        int numPlanets = 0;
        if (maxplanets > 0)
        {
            do
            {
                numPlanets = planetsNum();
            } while (numPlanets > maxplanets);
        }        
        planets.planetsLoc = new SataliteDetails[numPlanets];
        float hold = (CreateGalaxy.planetMuti);
        
        Vector2 circlePos;
        circlePos = Vector2.one;
        List<string> a = new List<string>();
        for (int i = 0; i < planets.planetsLoc.Length; i++)
        {          
            circlePos = Random.insideUnitCircle.normalized;
         
           
            planets.planetsLoc[i] = SataliteLocation(hold, minDis, dist, circlePos, WorkOutLife(i));
            
            hold = planets.planetsLoc[i].distFromBody + CreateGalaxy.planetMuti;
        }
    }


    /// <summary>
    /// Getting the locations to sapwn satalites 
    /// </summary>
    /// <param name="moveAmount"></param>
    /// <param name="minDist"></param>
    /// <param name="maxDist"></param>
    /// <param name="circle"></param>
    /// <param name="life"></param>
    /// <returns></returns>
    protected SataliteDetails SataliteLocation(float moveAmount, float minDist, float maxDist, Vector2 circle, bool life)
    {

        // start by moving out a bit from the planet 
        float distance = moveAmount + Random.Range(minDist, maxDist);
        Vector3 starPos;
        float move = Mathf.Sqrt((distance * distance) / 2);
        starPos = circle * move;
        starPos.z = starPos.y;
        starPos.y = 0;
        SataliteDetails a = new SataliteDetails(starPos, distance, life);
        return a;

    }

    /// <summary>
    /// returns the max number of planets fopr a star
    /// </summary>
    /// <returns></returns>
    private int MaxPlanet(Transform parentStar)
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
            // start with the size of the bigStar and remove the spsace the mini star would take up along with the raioud of a planet so planets cant stick out  
            float disatanceCalc = parentStar.lossyScale.x - (CreateGalaxy.planetMuti * 1.5f) - (CreateGalaxy.planetMuti / 2);
            disatanceCalc /= (4 * (dist)); // then size to the number of planets that cna fit 
            name = disatanceCalc.ToString();
            return Mathf.FloorToInt(disatanceCalc); //Mathf.RoundToInt((transform.lossyScale.x /CreateGalaxy.starMuti*2)- (CreateGalaxy.starMuti * 2));
        }

    }



    /// <summary>
    /// used to work out if a planet can have life
    /// </summary>
    private bool WorkOutLife(int planetNum)
    {
        if (planetNum >2 && planetNum < 6)
        {
            //life is not random in the end as it is already very rare
            return true;
        }
        return false;
    }


    public int planetsNum()
    {
        // the number of planets can be between 0 and 12 ( for now)
        // 40% are between 8 and 10 20% 11 or 12,  20% 5 6 7, 432 12% 1 6% 0   2%
        int numPlanets = Random.Range(0, 100);

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
    /// spawing A planet around the star 
    /// </summary>
    /// <param name="starPos">The stars position info</param>
    /// <param name="prefabName">name of the prefab to spawn</param>
    /// <param name="parent">name of the star to parent the planet to</param>
    /// <returns>the satalite spawned</returns>
    protected GameObject SpawnPlanet(SataliteDetails starPos,GameObject parent)
    {
        GameObject a = Instantiate(Resources.Load(planetPrefabName)) as GameObject;
        Planet holds = a.GetComponent<Planet>();
        holds.orbitingBody = parent;
        holds.distPlanet = starPos.distFromBody;
           

        a = GeneralSpawn(starPos, parent, a);
        holds.haveLife = starPos.haveLifeHold;
        holds.SortBiomes();


        return a;
    }

    protected GameObject GeneralSpawn(SataliteDetails starPos, GameObject parent, GameObject theSpawn)
    {        
        theSpawn.name = starPos.distFromBody.ToString();
        theSpawn.transform.SetParent(parent.transform);

        theSpawn.transform.position = parent.transform.position;

        theSpawn.transform.position += starPos.location;

        theSpawn.transform.LookAt(parent.transform);
        return theSpawn;

    }



    protected GameObject SpawnTwinPlanet(SataliteDetails starPos, GameObject parent)
    {
        GameObject a = Instantiate(Resources.Load(twinPlanetPrefabName)) as GameObject;
        TwinPlanet holds = a.GetComponent<TwinPlanet>();
        holds.orbitingBody = parent;
        holds.distPlanet = starPos.distFromBody;     

       
        a = GeneralSpawn(starPos, parent,a);
        holds.SortBiomes();
        a.transform.LookAt(parent.transform);
        return a;
    }

    protected GameObject SpawnAsteroid(SataliteDetails starPos,  GameObject parent)
    {
        GameObject asteroidHolder = new GameObject();
        asteroidHolder.name = "asteroidHolder";

        asteroidHolder.transform.SetParent(parent.transform);
        asteroidHolder.transform.position = parent.transform.position;
        asteroids = new GameObject[18];
        for (int i =0;i< asteroids.Length;i++)
        {

            asteroids[i] = Instantiate(Resources.Load(asteroidPrefabName)) as GameObject;
            asteroids[i].transform.localScale = (CreateGalaxy.planetMuti / 2) * Vector3.one;
            asteroids[i].transform.SetParent(asteroidHolder.transform);

            asteroids[i].transform.position = asteroidHolder.transform.position;

            asteroids[i].transform.position += starPos.location;

            asteroids[i].transform.RotateAround(asteroidHolder.transform.position, Vector3.up, (i * 20));

        }        
        
      
        
        return asteroidHolder;
    }

    private void SortPlanetSpawning()
    {
        int asteroidMin = 3;
        for (int c = 0; c < planetsOnStar.Length; c++)
        {
            planetsOnStar[c].planets = new GameObject[planetsOnStar[c].planetsLoc.Length];
            bool asteroid = false;
            bool twinWorld = true;
            if (starType.SingeStar == typeOfStar &&planetsOnStar[c].planetsLoc.Length > asteroidMin) // if there is a sngle star and more than 3 planets one ccan be an asteroid
            {
                asteroid = true;
            }

            for (int i = 0; i < planetsOnStar[c].planetsLoc.Length; i++)
            {
                if (asteroid && i >= asteroidMin && Random.Range(0,100)<40)
                {
                    planetsOnStar[c].planets[i] = SpawnAsteroid(planetsOnStar[c].planetsLoc[i], miniStars[c]);
                    asteroid = false;
                }
                else if (twinWorld &&  Random.Range(0, 100) < 10)
                {
                    planetsOnStar[c].planets[i] = SpawnTwinPlanet(planetsOnStar[c].planetsLoc[i], miniStars[c]);
                    twinWorld = false;
                }
                else
                {
                    planetsOnStar[c].planets[i] = SpawnPlanet(planetsOnStar[c].planetsLoc[i], miniStars[c]);
                }
            }
        }
        planetsSpawned = true;


    }



    void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == "MainCamera")
        {
            //Decrease Speed for moving around soloar system
            other.GetComponent<CameraMove>().DecreaseSpeedStar();

            for(int  i = 0; i < miniStars.Length; i++)
            {
                miniStars[i].SetActive(true);
            }
            if (planetsSpawned)
            {
                
                foreach (PlanetsInfo pla in planetsOnStar)
                {
                    for (int i = 0; i < pla.planets.Length; i++)
                    {
                        pla.planets[i].SetActive(true);                    
                    }         
                }
            }
            else
            {
                SortPlanetSpawning();
            }

            StopAllCoroutines(); // use this to stop the current fade if any

            for (int i = 0; i < theRend.Length; i++)
            {
                StartCoroutine(ReduceAlpha(theRend[i]));
            }
            Satalite holdPlan;
            for (int i =0;i< planetsOnStar.Length; i++)
            {
                if (planetsOnStar[i] != null && planetsOnStar[i].planets.Length > 0)
                {
                    holdPlan = planetsOnStar[i].planets[0].GetComponent<Satalite>();
                    if (!planetListText)
                    {
                        planetListText = GameObject.FindGameObjectWithTag("PlanetList").GetComponent<Text>();
                    }
                    StartCoroutine(DisplayText(holdPlan));
                }
            }
        }        
    }

    private IEnumerator DisplayText(Satalite planet)
    {
        while (!planet.startFinish)
        {
            yield return new WaitForEndOfFrame();
        }
        ShowPlanetList();
    }

    private void ShowPlanetList()
    {       
        string hold = "Planet List" + "\n";
        for (int c = 0;c < planetsOnStar.Length;c++)
        {
            for (int i = 0; i < planetsOnStar[c].planets.Length; i++)
            {
                Satalite holdSat = planetsOnStar[c].planets[i].GetComponent<Satalite>();               
                if (holdSat)
                {
                    hold += "star: " + (c + 1);
                    hold += " Planet " + (i + 1);
                    Planet holdPlan = holdSat.GetComponent<Planet>();
                    if (holdPlan)
                    {
                        hold += ", Moons: " + holdPlan.moons.Count;
                        hold += " Life: " + holdPlan.haveLife + "\n";
                    }
                    else
                    {
                        hold += ", TwinPlanet"  + "\n";
                    }
                }
                else
                {
                    hold += "Asteroids " + "\n";

                }
            }
        }
        planetListText.text = hold;
    }

    public void HidePlanetList()
    {
        if (planetListText)
        {
            planetListText.text = "";
        }            
    }

   

    //used to increase the alpha of the star when the player moves out
    protected override IEnumerator IncreaseAlpha(Renderer starRend)
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
        foreach (PlanetsInfo pla in planetsOnStar)
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
            StopAllCoroutines(); // use this to stop the current fade if any

            for (int i = 0; i < theRend.Length; i++)
            {
                StartCoroutine(IncreaseAlpha(theRend[i]));
            }
            HidePlanetList();

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
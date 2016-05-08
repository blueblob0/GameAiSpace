﻿//script made by: up651590
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class Planet : Satalite
{
    public LineRenderer ringLine;
    public LineRenderer ringLine2;

    bool ring1 = false;
    bool ring2 = false;

    public List<GameObject> moons = new List<GameObject>();
    public List<float> spheres = new List<float>();
    public Renderer surface; // The plane on the planet
    public string moonPrefabName = "MoonPrefab";
    const float dist = CreateGalaxy.planetMuti/5;
    const float minDis = CreateGalaxy.planetMuti /10;
    Texture2D planTexture;
    public biomes[] biomeList;

    //For the biome collider----------
    public GameObject biomeCollider;
    //--------------------------------
    public bool haveLife = false;
    public bool startFinish = false;
    

    // Use this for initialization


    protected override void Start () {
        base.Start();
        Transform theParent = transform.parent;
        transform.parent = null;
        transform.localScale= Vector3.one * CreateGalaxy.planetMuti;

        transform.SetParent(theParent);

        

        //set the texture of the surface so we can call it later;
        surface.gameObject.SetActive(false);
        
        CreateOrbit();
        
        int holdRand = Random.Range(0, 100);
        if (holdRand < 10) //10% chance to have a ring
        {
            CreatePlanetRing(1, ringLine);
            ring1 = true;
            if (holdRand < 5)//5% chance to have 2 rings 
            {               
                CreatePlanetRing(1.2f,ringLine2);
                ring2= true;
            }
        }

        GenerateMoons();

        startFinish = true;
    }

    private void GenerateMoons()
    {
        int maxmoon = 4;
        float hold = transform.lossyScale.x / 2;
        if (ring1) // have rings replace moons
        {
            maxmoon--;
            hold += minDis;
        }
        if (ring2)
        {
            maxmoon--;
            hold += minDis;
        }
        int numMoons = Random.Range(0, maxmoon+1);
        if (numMoons > 0)
        {
            Vector2 circlePos = Vector2.one;
            for (int i = 0; i < numMoons; i++)
            {
                circlePos = Random.insideUnitCircle.normalized;
                moons.Add(SpawnSatalite(hold, minDis, dist, moonPrefabName, circlePos));
                hold = moons[i].GetComponent<Satalite>().distPlanet;
            }
        }
       
       

    }

    /// <summary>
    /// creates a ring to show orbit
    /// </summary>
    void CreateOrbit()
    {
        LineRenderer line = gameObject.GetComponent<LineRenderer>();
        int segments =40;
        float radius = (transform.lossyScale.x/2);

        line.SetVertexCount(segments + 1);
        line.useWorldSpace = true;
        
        float x;
        float y =0;
        float z = 0f;

        float angle = 0f;

        for (int i = 0; i < (segments + 1); i++)
        {
            float move = Mathf.Sqrt((distPlanet * distPlanet) / 2);
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * move ;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * move ;
            Vector3 pos = new Vector3(x, y, z);
            pos = transform.parent.position + pos;
            line.SetPosition(i, pos);

            angle += (360f / segments);
        }
    }

    /// <summary>
    /// creaes a planet ring
    /// </summary>
    void CreatePlanetRing(float ringNumber, LineRenderer theRing)
    {
        theRing.gameObject.SetActive(true);
        int segments = 40;
        float radius = (transform.lossyScale.x / 2);
        Debug.Log("ring");
        theRing.SetVertexCount(segments + 1);
        theRing.useWorldSpace = true;

        float x;
        float y = 0;
        float z = 0f;

        float angle = 0f;

        for (int i = 0; i < (segments + 1); i++)
        {
            float move = Mathf.Sqrt((distPlanet * distPlanet) / 2);
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * ((ringNumber * transform.lossyScale.x/2) +(CreateGalaxy.planetMuti/10)) ;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * ((ringNumber * transform.lossyScale.x/2) + (CreateGalaxy.planetMuti / 10));
            Vector3 pos = new Vector3(x, y, z);
            pos = transform.position + pos;
            theRing.SetPosition(i, pos);

            angle += (360f / segments);
        }
    }

    protected override void SetScale()
    {
        transform.localScale = CreateGalaxy.planetMuti * Vector3.one;
    }

    public void SetBiomes()
    {

        Color planColour = Color.white;
        planTexture = new Texture2D(12, 12);
        GetComponent<Renderer>().material.mainTexture = planTexture;
        surface.material.mainTexture = planTexture;
        int numOfChanges = 2;
        
        int biomeChance= Random.Range(0, 100);
        
        if (biomeChance < 10) //10%
        {
            numOfChanges = 1;

        }
        else if (biomeChance < 80) //70%
        {
            numOfChanges = 2;
        }
        else  //20%
        {
            numOfChanges = 3;
        }


        biomeList = new biomes[numOfChanges];

        for(int i =0;i<biomeList.Length;i++)
        {
            //biomes hold = 1;
            biomeList[i] =  (biomes)Random.Range(0, System.Enum.GetValues(typeof(biomes)).Length);
        }

        float oneSect = 0;// texture.height / numOfChanges;
        int count = 0;
        for (int y = 0; y < planTexture.height; y++)
        {
            if (y == oneSect)
            {
                planColour = GetBiomeColour(biomeList[count]);// new Color(Random.Range(0, 1.0f), Random.Range(0, 1.0f), Random.Range(0, 1.0f));
                count++;                
                //Debug.Log(planColour);
                float test = planTexture.height;
                test /= numOfChanges;
                oneSect = Mathf.CeilToInt((test) * count);

                //Placing the biome zones---------------------------------------------------------------------
                //Spawn in the cube collider
                GameObject cube = GameObject.Instantiate(biomeCollider);
                //Set the position
                cube.transform.position = new Vector3(transform.position.x, 0, (transform.position.z - y) + (numOfChanges > 1 ? (test/2) + (numOfChanges == 3 ? 2 : 0) : 0 ));
                //Resize
                cube.transform.localScale = new Vector3(13, 1, 13 / numOfChanges);
                //Let the collider know which biome it is and what parent to set (position & scale fuck up if you set the parent here)
                BiomeColliderScript script = cube.GetComponent<BiomeColliderScript>();
                script.biomeType = biomeList[count - 1]; //-1 because count gets increased
                script.parenToSet = transform;
                script.setUp();
                //--------------------------------------------------------------------------------------------
            }

            for (int x = 0; x < planTexture.width; x++)
            {
                planTexture.SetPixel(x, y, planColour);
            }
        }
        planTexture.Apply();
    }
    
    Color GetBiomeColour(biomes test)
    {
        if(test == biomes.Land)
        {
            return new Color32(43, 214, 43, 1); //Bright Green
        }
        else if(test == biomes.Forest)
        {
            return new Color32(47, 153, 47, 1); //Dark Green
        }
        else if (test == biomes.Desert)
        {
            return new Color32(242, 237, 82, 1); //Light yellow
        }
        else if (test == biomes.Ice)
        {
            return new Color32(77, 232, 217, 1); //Light cyan
        }
        else if (test == biomes.Water)
        {
            return new Color32(21, 60, 214, 1); //Deep blue
        }
        else if (test == biomes.Mountainous)
        {
            return new Color32(212, 107, 32, 1); //Brown
        }
        else if (test == biomes.Lava)
        {
            return new Color32(255, 0, 0, 1); //Red
        }
        return new Color32(43, 214, 43, 1);
    }
    
    // Update is called once per frame

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "minicam")
        {           

            //IF the planet can have life then spawn the surface and creatures
            if (haveLife)
            {
                surface.gameObject.SetActive(true);
                //Decrease Speed for moving around planet surface 
                other.GetComponentInParent<CameraMove>().DecreaseSpeedPlanet();
            }
            
           // Color c = GetComponent<Renderer>().material.color;
           // c.a = 0;
            //GetComponent<Renderer>().material.color = c;
            StopAllCoroutines(); // use this to stop the current fade if any
                        
            StartCoroutine(ReduceAlpha(gameObject.GetComponent<Renderer>()));

        }
        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "minicam")
        {
            if (haveLife)
            {
                surface.gameObject.SetActive(false);
                //increase speed for moving around soloar system
                other.GetComponentInParent<CameraMove>().IncreasePlanetSpeed();
            }
            
            
            StopAllCoroutines(); // use this to stop the current fade if any
            StartCoroutine(IncreaseAlpha(gameObject.GetComponent<Renderer>()));

            //Color c = GetComponent<Renderer>().material.color;
            // c.a = 1;
            // GetComponent<Renderer>().material.color = c;


        }


    }


}


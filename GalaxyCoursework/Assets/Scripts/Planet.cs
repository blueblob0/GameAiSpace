﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class Planet : Satalite
{
    int numMoons;
    public List<GameObject> moons = new List<GameObject>();
    public List<float> spheres = new List<float>();
    public Renderer surface; // The plane on the planet
    public string planetPrefabName = "MoonPrefab";
    const float dist = 0.03f;
    const float minDis = 0.06f;
    Texture2D planTexture;
    public biomes[] biomeList;

    // Use this for initialization


    protected override void Start () {
        base.Start();
        Transform theParent = transform.parent;
        transform.parent = null;
        transform.localScale= Vector3.one * CreateGalaxy.starMuti;

        transform.SetParent(theParent);

        numMoons = Random.Range(0, 4);
        speed = Random.Range(10,35);
        float hold = transform.lossyScale.x/2;
        Vector2 circlePos = Random.insideUnitCircle.normalized;
        circlePos =  Vector2.one;

        for (int i = 0; i < numMoons; i++)
        {
            moons.Add(SpawnSatalite(hold, minDis, dist, planetPrefabName, circlePos));
            hold = moons[i].GetComponent<Satalite>().distPlanet;

        }

        //set the texture of the surface so we can call it later;


      

        surface.gameObject.SetActive(false);
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
                Debug.Log(planColour);
                float test = planTexture.height;
                test /= numOfChanges;
                oneSect = Mathf.CeilToInt((test) * count);

                //Placing the biome zones---------------------------------------------------------------------
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(transform.position.x, 0, (transform.position.z + y) - (test/2) - (numOfChanges == 3 ? 2 : 0));
                cube.name = count + " - " + y;
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
            return Color.grey;
        }
        else if(test == biomes.Forest)
        {
            return Color.yellow;
        }
        else if (test == biomes.Desert)
        {
            return Color.red;
        }
        else if (test == biomes.Ice)
        {
            return Color.cyan;
        }
        else if (test == biomes.Water)
        {

            return Color.blue;

        }
        else if (test == biomes.Mountainous)
        {


            return Color.magenta;
        }
        else if (test == biomes.Lava)
        {

            return Color.black;

        }


        return Color.white;


    }
    
    // Update is called once per frame

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "minicam")
        {
            Debug.Log("hitp" + name);
            surface.gameObject.SetActive(true);
            //Decrease Speed for moving around planet surface 
            other.GetComponentInParent<CameraMove>().DecreaseSpeed();
            Color c = GetComponent<Renderer>().material.color;
            c.a = 0;
            GetComponent<Renderer>().material.color = c;
            //Set the spawning here
            //Here
            //Here
            //Here
            //Here
            //Here
            //Here
        }
        else
        {
           // Debug.Log(other.tag);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "minicam")
        {
            surface.gameObject.SetActive(false);
            //increase speed for moving around soloar system
            other.GetComponentInParent<CameraMove>().IncreaseSpeed();
            Color c = GetComponent<Renderer>().material.color;
            c.a = 1;
            GetComponent<Renderer>().material.color = c;
            //Set the despawning here
            //Here
            //Here
            //Here
            //Here
            //Here
            //Here
        }


    }


}


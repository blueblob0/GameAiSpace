using UnityEngine;
using System.Collections.Generic;

public class Planet : Satalite
{
    int numPlanets;
    public List<GameObject> planets = new List<GameObject>();
    public List<float> spheres = new List<float>();
    public Renderer surface;
    public string planetPrefabName = "MoonPrefab";
    const float dist = 0.03f;
    const float minDis = 0.06f;
    float planetsize = 0.1f;


    // Use this for initialization

        
    protected override void Start () {
        base.Start();
        numPlanets = Random.Range(0, 4);
        speed = Random.Range(10,35);
        float hold = transform.lossyScale.x/2;
        Vector2 circlePos = Random.insideUnitCircle.normalized;
         circlePos =  Vector2.one;
        for (int i = 0; i < numPlanets; i++)
        {
            planets.Add(SpawnSatalite(hold, minDis, dist, planetPrefabName, circlePos));
            hold = planets[i].GetComponent<Satalite>().distPlanet;

        }
        //gameObject.GetComponent<Transform>().localScale = Vector3.one * planetsize * CreateGalaxy.starMuti;
        Texture2D texture = new Texture2D(128, 128);
        gameObject.GetComponent<Transform>().localScale *=  CreateGalaxy.starMuti;
        GetComponent<Renderer>().material.mainTexture = texture;
        surface.material.mainTexture = texture;
        Color color = Color.white;
        int numOfChanges = 6;     
        float oneSect = texture.height / numOfChanges;
        int count = 1;
        for (int y = 0; y < texture.height; y++)
        {
            
            if (y == oneSect )
            {
                count++;
                color = new Color(Random.Range(0, 1.0f), Random.Range(0, 1.0f), Random.Range(0, 1.0f));
                oneSect = (texture.height / numOfChanges) * count;
            }

            for (int x = 0; x < texture.width; x++)
            {                
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
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
            Debug.Log(other.tag);
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

enum biome
{
    Land,
    Forest,
    Desert,
    Radiated,
    Ice,
    Water,
    Mountainous,
    Lava


}
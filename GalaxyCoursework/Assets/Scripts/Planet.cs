using UnityEngine;
using System.Collections.Generic;

public class Planet : Satalite
{
    int numPlanets;
    public List<GameObject> planets = new List<GameObject>();
    public List<float> spheres = new List<float>();
    public string planetPrefabName = "MoonPrefab";
    const float dist = 0.3f;
    const float minDis = 0.6f;



    // Use this for initialization
    void Start () {
        numPlanets = Random.Range(0, 4);
        speed = Random.Range(10,35);
        float hold = transform.lossyScale.x/2;
        for (int i = 0; i < numPlanets; i++)
        {
            planets.Add(SpawnSatalite(hold, minDis, dist, planetPrefabName));
            hold = planets[i].GetComponent<Satalite>().distPlanet;

        }

        Texture2D texture = new Texture2D(128, 128);
        GetComponent<Renderer>().material.mainTexture = texture;
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
	
}

using UnityEngine;
using System.Collections.Generic;

public class Planet : Satalite
{
    int numPlanets;
    public List<GameObject> moons = new List<GameObject>();    
    public string planetPrefabName = "MoonPrefab";
    const float dist = 0.4f;
    const float minDis = 0.2f;



    // Use this for initialization
    protected override void Start () {
        numPlanets = Random.Range(0, 4);
      
        mass = 5.972f * Mathf.Pow(10, 24);
        float hold = transform.lossyScale.x/2;
        for (int i = 0; i < numPlanets; i++)
        {
            moons.Add(SpawnSatalite(hold, minDis, dist, planetPrefabName));
            hold = moons[i].GetComponent<Satalite>().distPlanet;

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

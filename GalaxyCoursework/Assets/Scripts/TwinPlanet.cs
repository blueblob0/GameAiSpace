using UnityEngine;
using System.Collections;

public class TwinPlanet : Satalite
{
    public GameObject[] planets;

    public biomes[] biomeList2;

    protected override void Start()
    {
        base.Start();
        Transform theParent = transform.parent;
        transform.parent = null;
        transform.localScale = Vector3.one * CreateGalaxy.planetMuti;

        transform.SetParent(theParent);

        CreateOrbit();
        startFinish = true;
    }

    /// <summary>
    /// set biomes for twin planets
    /// </summary>
    public override void SortBiomes()
    {
        SetBiomes(biomeList,planets[0]);
        SetBiomes(biomeList2, planets[1]);
    }

    public void SetBiomes(biomes[] abiomeList, GameObject planet)
    {

        Color planColour = Color.white;
        planTexture = new Texture2D(12, 12);
        planet.GetComponent<Renderer>().material.mainTexture = planTexture;

        int numOfChanges = 2;

        int biomeChance = Random.Range(0, 100);

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

        abiomeList = new biomes[numOfChanges];

        for (int i = 0; i < abiomeList.Length; i++)
        {
            //biomes hold = 1;
            abiomeList[i] = (biomes)Random.Range(0, System.Enum.GetValues(typeof(biomes)).Length);
        }

        float oneSect = 0;// texture.height / numOfChanges;
        int count = 0;
        for (int y = 0; y < planTexture.height; y++)
        {
            if (y == oneSect)
            {
                planColour = GetBiomeColour(abiomeList[count]);// new Color(Random.Range(0, 1.0f), Random.Range(0, 1.0f), Random.Range(0, 1.0f));
                count++;
                //Debug.Log(planColour);
                float test = planTexture.height;
                test /= numOfChanges;
                oneSect = Mathf.CeilToInt((test) * count);
            }

            for (int x = 0; x < planTexture.width; x++)
            {
                planTexture.SetPixel(x, y, planColour);
            }
        }
        planTexture.Apply();
    }
}


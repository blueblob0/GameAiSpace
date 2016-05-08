//script made by: up651590
using UnityEngine;

public abstract class Satalite : CelestialBody
{    
    public GameObject orbitingBody;
    public bool startFinish = false;
    public float distPlanet;// distance from planet

    protected Texture2D planTexture;
    protected biomes[] biomeList;
        
    protected override void SetScale()
    {
        transform.localScale = CreateGalaxy.planetMuti * Vector3.one;
    }

    public abstract void SortBiomes();

    /// <summary>
    /// creates a ring to show orbit
    /// </summary>
    protected void CreateOrbit()
    {
        LineRenderer line = gameObject.GetComponent<LineRenderer>();
        int segments = 40;
        float radius = (transform.lossyScale.x / 2);

        line.SetVertexCount(segments + 1);
        line.useWorldSpace = true;

        float x;
        float y = 0;
        float z = 0f;

        float angle = 0f;

        for (int i = 0; i < (segments + 1); i++)
        {
            float move = Mathf.Sqrt((distPlanet * distPlanet) / 2);
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * move;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * move;
            Vector3 pos = new Vector3(x, y, z);
            pos = transform.parent.position + pos;
            line.SetPosition(i, pos);

            angle += (360f / segments);
        }
    }


    

    protected Color GetBiomeColour(biomes test)
    {
        if (test == biomes.Land)
        {
            return new Color32(43, 214, 43, 1); //Bright Green
        }
        else if (test == biomes.Forest)
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

}

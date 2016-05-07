//script made by: up651590
using UnityEngine;
using System.Collections;

public class Noise : MonoBehaviour {
    public int pixWidth;
    public int pixHeight;
    public float xOrg;
    public float yOrg;
    public float scale = 1.0F;
    private Texture2D noiseTex;
    private Color[] pix;
    private Renderer rend;

    void Start()
    {

        for(float i = 0; i < 1; i+=0.01f)
        {
            float sample = Mathf.PerlinNoise(i, i);
            Debug.Log(i +" "+sample);

        }

      
    }
    void CalcNoise()
    {
       
    }
    void Update()
    {
        CalcNoise();
    }
}

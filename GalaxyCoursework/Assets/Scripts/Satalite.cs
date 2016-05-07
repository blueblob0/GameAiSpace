//script made by: up651590
using UnityEngine;

public class Satalite : CelestialBody
{    
    public GameObject orbitingBody;
    public float distPlanet;// distance from planet
    void Update()
    {
       // transform.RotateAround(orbitingBody.transform.position, Vector3.up, speed * Time.deltaTime);
        //Debug.Log(orbitingBody.transform.position);
    }

    protected override void SetScale()
    {
        transform.localScale = CreateGalaxy.planetMuti/10 *Vector3.one;
    }
}

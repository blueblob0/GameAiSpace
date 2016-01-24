using UnityEngine;
using System.Collections;

public abstract class Satalite : CelestialBody
{
    
    public GameObject orbitingBody;
    public float distPlanet;// distance from planet
    protected override void Update()
    {
        transform.RotateAround(orbitingBody.transform.position, Vector3.up, moveAmount * Time.deltaTime);

    }


    protected override abstract void Start();



}

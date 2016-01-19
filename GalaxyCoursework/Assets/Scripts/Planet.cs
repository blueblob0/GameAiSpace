using UnityEngine;
using System.Collections;

public class Planet : CelestialBody
{
   public  GameObject myStar;
  


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(myStar.transform.position, Vector3.up, 20 * Time.deltaTime);
    }
}

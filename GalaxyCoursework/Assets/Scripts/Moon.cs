using UnityEngine;
using System.Collections;

public class Moon : Satalite {
    float holding =0;
    // Use this for initialization
    protected override void Start ()
    {
        mass = 7.34767f * Mathf.Pow(10, 22); // just putting stars as the mass of the sun for now 
        
        //transform.RotateAround(orbitingBody.transform.position, Vector3.up, speed * Time.deltaTime);
    }

  


}

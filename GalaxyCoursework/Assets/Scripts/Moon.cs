using UnityEngine;
using System.Collections;

public class Moon : Satalite {

	// Use this for initialization
	protected  override void Start ()
    {
        base.Start();
        speed = Random.Range(100, 300);
    }
	
	

   
}

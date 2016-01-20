﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* The super class that will handle all the characteristics
 * of the alien creatures that will spawn, all the random spawning variables
 * like arms, legs, heads will happen here too
 */

public class AlienCreature : MonoBehaviour {

    /// <summary>
    /// Will spawn the creature into the game world
    /// </summary>
    protected virtual void Start () {
        //Create the body of the creature
        createBody();
	}
	
	/// <summary>
    /// Update the creature to peform movement ect.
    /// </summary>
	protected virtual void Update () {
	
	}

    /// <summary>
    /// Spawns a body for the creature that is parented to this game object
    /// </summary>
    /// <param name="maxHeads">The maximum amount of hezads the creature can have</param>
    /// <param name="maxArms">The maximum amount of arms the creature can have</param>
    /// <param name="maxLegs">The maximum amount of legs the creature can have</param>
    protected void createBody(ushort maxHeads = 1, ushort maxArms = 2, ushort maxLegs = 2){
        //Create a cube for the body
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //Set the position of the body the gameObject's
        body.transform.position = transform.position;
        //Set the name
        body.name = "Body";

        //Create the heads
        ushort headCount = (ushort)Random.Range(1, maxHeads + 1);
        //Get the position where the heads will spawn
        Vector3 headSpawnPos = body.transform.position;
        headSpawnPos.y += body.GetComponent<Renderer>().bounds.size.y;
        //Parent to hold all of the heads
        GameObject headParent = new GameObject("Head Parent");
        headParent.transform.position = headSpawnPos;
        headParent.transform.SetParent(body.transform);
        //List to kee ptrack of all the heads
        List<GameObject> heads = new List<GameObject>();
        //Loop through the amount of heads and spawn them in
        for(ushort i = 0; i < headCount; i++) {
            //Spawn in the primitive
            GameObject headToSpawn = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //Set the position
            headToSpawn.transform.position = headSpawnPos;
            headSpawnPos.x += headToSpawn.GetComponent<Renderer>().bounds.size.x;
            //Scale the heads down
            headToSpawn.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            //Set the parent
            headToSpawn.transform.SetParent(headParent.transform);
            //Change the name
            headToSpawn.name = "Head " + (i + 1);
            //Add to the list
            heads.Add(headToSpawn);
        }
        //Center the heads
        if(headCount > 1) {
            Vector3 newPos = headParent.transform.position;
            newPos.x -= 0.5f * (headCount - 1);
            headParent.transform.position = newPos;
        }

        //Create the arms

        //Create the legs
    }
}
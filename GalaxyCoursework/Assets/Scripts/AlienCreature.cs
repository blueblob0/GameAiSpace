using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* The super class that will handle all the characteristics
 * of the alien creatures that will spawn, all the random spawning variables
 * like arms, legs, heads will happen here too
 */

public class AlienCreature : MonoBehaviour {

    //Set in inspector
    public string creatureName = "Change my name";
    public ushort maxAmountOfHeads = 1;
    public ushort maxAmountOfArms = 2;
    public ushort maxAmountOfLegs = 2;

    //Make sure the creature doesn't spawn in again
    private bool spawned = false;

    /// <summary>
    /// Will spawn the creature into the game world
    /// </summary>
    protected virtual void Start () {
        //Check that nthoing is out of bounds
        if(maxAmountOfHeads < 1) {
            maxAmountOfHeads = 1;
        }
        if(maxAmountOfArms < 2) {
            maxAmountOfArms = 2;
        }
        if(maxAmountOfLegs < 2) {
            maxAmountOfLegs = 2;
        }

        //Set the name
        name = creatureName;

        //Get the object's rotation
        Quaternion rot = transform.rotation;
        //Reset it to 0
        transform.rotation = new Quaternion();

        //Create the creature
        createCreature(maxAmountOfHeads, maxAmountOfArms, maxAmountOfLegs);

        //Now the creature has been created, re apply the rotation
        transform.rotation = rot;
	}
	
	/// <summary>
    /// Update the creature to peform movement ect.
    /// </summary>
	protected virtual void Update () {
	
	}

    /// <summary>
    /// Spawns in the creature
    /// </summary>
    /// <param name="maxHeads">The maximum amount of hezads the creature can have</param>
    /// <param name="maxArms">The maximum amount of arms the creature can have</param>
    /// <param name="maxLegs">The maximum amount of legs the creature can have</param>
    protected void createCreature(ushort maxHeads = 1, ushort maxArms = 2, ushort maxLegs = 2){
        if(!spawned) {
            //Create a cube for the body
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //Set the position of the body the gameObject's
            body.transform.position = transform.position;
            //Set the name
            body.name = "Body";
            //Make the body a child of this object
            body.transform.SetParent(transform);

            //Create the heads
            ushort headCount = (ushort)Random.Range(1, maxHeads + 1);
            //Get the position where the heads will spawn
            Vector3 headSpawnPos = body.transform.position;
            headSpawnPos.y += body.GetComponent<Renderer>().bounds.size.y;
            //Parent to hold all of the heads
            GameObject headParent = new GameObject("Head Parent");
            headParent.transform.position = headSpawnPos;
            headParent.transform.SetParent(body.transform);
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
            }
            //Center the heads
            if(headCount > 1) {
                Vector3 newPos = headParent.transform.position;
                newPos.x -= 0.5f * (headCount - 1);
                headParent.transform.position = newPos;
            }

            //Create the arms
            ushort armCount = (ushort)Random.Range(2, maxArms + 1);
            //Get the position the arms will spawn
            Vector3 armSpawnPos = body.transform.position;
            armSpawnPos.x += body.GetComponent<Renderer>().bounds.size.x;
            //Parent to hold all of the arms
            GameObject armParent = new GameObject("Arm Parent");
            armParent.transform.position = body.transform.position;
            armParent.transform.SetParent(body.transform);
            //Used to position the arms
            int twoCheck = 2;
            //Loop through the arm count and spawn in the arms
            for(ushort i = 0; i < armCount; i++) {
                //Multiply the position by to put the arms on each side
                short spawnMod;
                //If the number is even we want to spawn the arm on X+ else spawn on X-
                if(isEven(i)) {
                    spawnMod = 1;
                } else {
                    spawnMod = -1;
                }
                //Spawn in the primitive
                GameObject armtoSpawn = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                //Set the position
                armtoSpawn.transform.position = armSpawnPos;
                Vector3 temp = armtoSpawn.transform.position;
                temp.x *= spawnMod;
                armtoSpawn.transform.position = temp;
                if(i == twoCheck - 1) {
                    armSpawnPos.y -= armtoSpawn.GetComponent<Renderer>().bounds.size.x / 2;
                    twoCheck += 2;
                }
                //Scale the arm
                armtoSpawn.transform.localScale = new Vector3(0.25f, 0.75f, 0.25f);
                //rotate
                armtoSpawn.transform.rotation = Quaternion.Euler(0, 0, 90);
                //Set the Parent
                armtoSpawn.transform.SetParent(armParent.transform);
                //Change the name
                if(isEven(i)) {
                    armtoSpawn.name = "Right Arm " + (i + 1);
                } else {
                    armtoSpawn.name = "Left   Arm " + (i + 1);
                }
            }
            
            //Create the legs

            //Creature has now been spawned
            spawned = true;
        }
    }

    /// <summary>
    /// Returns true if the value passed is even
    /// </summary>
    /// <param name="value">Value to pass</param>
    /// <returns></returns>
    bool isEven(int value) {
        return value % 2 == 0;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* The super class that will handle all the characteristics
 * of the alien creatures that will spawn, all the random spawning variables
 * like arms, legs, heads will happen here too
 */

public class AlienCreature : MonoBehaviour {

    //Set in inspector
    public string creatureName = "Name";
    public ushort maxAmountOfHeads = 1;
    public ushort maxAmountOfArms = 2;
    public ushort maxAmountOfLegs = 2;
    public ushort intelligence = 0;
    public ushort strength = 0;
    public ushort speed = 0;

    //Lists to keep track of the limbs
    List<GameObject> heads = new List<GameObject>();
    List<GameObject> arms = new List<GameObject>();
    List<GameObject> legs = new List<GameObject>();

    //Make sure the creature doesn't spawn in again
    private bool spawned = false;

    //The rot val of the arms
    private float rotSpeedArm = 30;
    private float armTotalRot = 0;
    //The rot val of the legs
    private float rotSpeedLeg = 30;
    private float legTotalRot = 0;

    //An array of potential names
    string[] nameParts = {"si", "la", "ti", "aa", "ul", "er", "ta", "ei",
                          "ae", "ui", "lo", "ka", "pi", "cc", "sc", "br",
                          "fj", "or", "nj", "st", "th", "yu", "pt", "kl",
                          "cl", "ph", "pho", "ri", "we", "gh", "io", "ao",
                          "nm", "mm", "nn", "jy", "fv", "vv", "tb", "lk"};

    /// <summary>
    /// Will spawn the creature into the game world
    /// </summary>
    protected virtual void Start() {
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
        creatureName = "";
        int nameLength = Random.Range(2, 6);
        for(int i = 0; i < nameLength; i++) {
            creatureName += nameParts[Random.Range(1, 40)];
        }

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
    protected virtual void Update() {
        //Rotate the arms
        foreach(GameObject arm in arms) {
            arm.transform.Rotate(new Vector3(rotSpeedArm * Time.deltaTime, 0, 0));
        }
        armTotalRot += rotSpeedArm * Time.deltaTime;
        if(armTotalRot > 30) {
            rotSpeedArm *= -1;
        } else if(armTotalRot < -30) {
            rotSpeedArm *= -1;
        }

        //Rotate the legs
        foreach(GameObject leg in legs) {
            leg.transform.Rotate(new Vector3(rotSpeedLeg * Time.deltaTime, 0, 0));
        }
        legTotalRot += rotSpeedLeg * Time.deltaTime;
        if(legTotalRot > 30) {
            rotSpeedLeg *= -1;
        } else if(legTotalRot < -30) {
            rotSpeedLeg *= -1;
        }

    }

    /// <summary>
    /// Spawns in the creature
    /// </summary>
    /// <param name="maxHeads">The maximum amount of heads the creature can have</param>
    /// <param name="maxArms">The maximum amount of arms the creature can have</param>
    /// <param name="maxLegs">The maximum amount of legs the creature can have</param>
    protected void createCreature(ushort maxHeads = 1, ushort maxArms = 2, ushort maxLegs = 2) {
        if(!spawned) {
            //Create a cube for the body
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //Set the position of the body
            body.transform.position = transform.position;
            //Set the scale
            body.transform.localScale = new Vector3(1, 1.5f, 1);
            //Set the name
            body.name = "Body";
            //Make the body a child of this object
            body.transform.SetParent(transform);

            //-----------------Create the heads-----------------\\
            //Get the position where the heads will spawn
            Vector3 headSpawnPos = body.transform.position;
            headSpawnPos.y += body.GetComponent<Renderer>().bounds.size.y / 2;
            //Parent to hold all of the heads
            GameObject headParent = new GameObject("Head Parent");
            headParent.transform.position = headSpawnPos;
            //Offset to line the heads up
            float offset = 0;
            //Loop through the amount of heads and spawn them in
            for(ushort i = 0; i < maxHeads; i++) {
                //Spawn in the primitive
                GameObject headToSpawn = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //Scale the heads down
                headToSpawn.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                //Correct the spawn position
                if(heads.Count == 0) {
                    headSpawnPos.y += headToSpawn.transform.localScale.y / 2;
                }
                //Set the position
                headToSpawn.transform.position = headSpawnPos;
                headSpawnPos.x += headToSpawn.GetComponent<Renderer>().bounds.size.x;
                //Set the parent
                headToSpawn.transform.SetParent(headParent.transform);
                //Change the name
                headToSpawn.name = "Head " + (i + 1);
                //Increase the offset
                if(heads.Count > 0) {
                    offset += headToSpawn.GetComponent<Renderer>().bounds.size.x;
                }
                heads.Add(headToSpawn);
            }
            //Center the heads
            if(maxHeads > 1) {
                Vector3 newPos = headParent.transform.position;
                newPos.x -= offset / 2;
                headParent.transform.position = newPos;
            }

            //----------------Create the arms---------------------\\
            //Get the position the arms will spawn
            Vector3 armSpawnPos = body.transform.position;
            armSpawnPos.x += body.GetComponent<Renderer>().bounds.size.x;
            //Parent to hold all of the arms
            GameObject armParent = new GameObject("Arm Parent");
            armParent.transform.position = body.transform.position;
            //Used to position the arms
            int twoCheck = 2;
            //Loop through the arm count and spawn in the arms
            for(int i = 0; i < maxArms; i += 2) {
                for(int j = 0; j < 4; j++) {
                    if(arms.Count < maxArms) {
                        //Multiply the position by to put the arms on each side
                        short spawnMod;
                        //If the number is even we want to spawn the arm on X+ else spawn on X-
                        if(isEven(j)) {
                            spawnMod = 1;
                        } else {
                            spawnMod = -1;
                        }
                        //Spawn in the primitive
                        GameObject armtoSpawn = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        //Scale the arm
                        armtoSpawn.transform.localScale = new Vector3(0.25f, 0.75f, 0.25f);
                        //Correct the spawn position
                        if(arms.Count == 0) {
                            armSpawnPos.x += armtoSpawn.GetComponent<Renderer>().bounds.size.x;
                            armSpawnPos.y += body.GetComponent<Renderer>().bounds.size.y / 4;
                        }
                        //Set the position
                        armtoSpawn.transform.position = armSpawnPos;
                        Vector3 temp = armtoSpawn.transform.position;
                        temp.x *= spawnMod;
                        armtoSpawn.transform.position = temp;
                        if(j == twoCheck - 1) {
                            armSpawnPos.y -= body.GetComponent<Renderer>().bounds.size.y / 4;
                            twoCheck += 2;
                        }
                        //rotate
                        armtoSpawn.transform.rotation = Quaternion.Euler(0, 0, 90);
                        //Set the Parent
                        armtoSpawn.transform.SetParent(armParent.transform);
                        //Change the name
                        if(isEven(j)) {
                            armtoSpawn.name = "Arm " + (i + j + 1) + " R";
                        } else {
                            armtoSpawn.name = "Arm " + (i + j + 1) + " L";
                        }
                        arms.Add(armtoSpawn);
                    }
                }
                //Reset the position
                armSpawnPos.y += body.GetComponent<Renderer>().bounds.size.y / 2;
                armSpawnPos.z -= body.GetComponent<Renderer>().bounds.size.z;
                //Put this back to 2
                twoCheck = 2;
            }

            //------------------Create the legs----------------------\\
            //Get the position the legs will spawn
            Vector3 legSpawnPos = body.transform.position;
            legSpawnPos.y -= body.GetComponent<Renderer>().bounds.size.y / 2;
            legSpawnPos.x -= body.GetComponent<Renderer>().bounds.size.x / 4;
            if(maxLegs > 2) {
                legSpawnPos.z += body.GetComponent<Renderer>().bounds.size.z / 2;
            }
            //Parent to hold all of the legs
            GameObject legParent = new GameObject("Leg Parent");
            legParent.transform.position = legSpawnPos;
            //Legs get placed in rows of 2
            for(int i = 0; i < maxLegs; i += 2) { //first loop does the group of legs (i.e front two or back two)
                for(int j = 0; j < 2; j++) { //Second loop does the actual legs
                    //Make sure we dont spawn extra legs
                    if(legs.Count < maxLegs) {
                        //Spawn in the primitve
                        GameObject legToSpawn = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        //Scale the legs
                        legToSpawn.transform.localScale = new Vector3(0.25f, 0.75f, 0.25f);
                        //Correct the spawn pos on the first leg
                        if(legs.Count == 0) {
                            legSpawnPos.y -= legToSpawn.GetComponent<Renderer>().bounds.size.y / 2;
                        }
                        //Set the position
                        legToSpawn.transform.position = legSpawnPos;
                        legSpawnPos.x += body.GetComponent<Renderer>().bounds.size.x / 2;
                        //Set the parent
                        legToSpawn.transform.SetParent(legParent.transform);
                        //Change the name
                        legToSpawn.name = "Leg " + (j + i + 1);
                        legs.Add(legToSpawn);
                    }
                }
                //Move the position back
                legSpawnPos.z -= body.GetComponent<Renderer>().bounds.size.z;
                legSpawnPos.x -= body.GetComponent<Renderer>().bounds.size.x;
            }

            //Adjust the depth of the body
            if(maxArms > 8 || maxLegs > 4) {
                //Find which one is the highest value to know how far to move the body back
                ushort higherVal;
                if(maxLegs > maxArms) {
                    higherVal = maxLegs;
                } else {
                    higherVal = maxArms;
                }
                //Increase the depth of the body based on the higher val
                //Vector3 scaleTemp = body.transform.localScale;
                //scaleTemp.z *= higherVal;
                //body.transform.localScale = scaleTemp;
                //Make sure the body is pushed back
                //Vector3 posTemp = body.transform.position;
                //posTemp.z -= body.GetComponent<Renderer>().bounds.size.z / higherVal;
                //body.transform.position = posTemp;
            }

            //Set the parents
            legParent.transform.SetParent(body.transform);
            armParent.transform.SetParent(body.transform);
            headParent.transform.SetParent(body.transform);

            //Creature has now been spawned
            spawned = true;
        }
    }

    /// <summary>
    /// Returns true if the value passed is even
    /// </summary>
    /// <param name="value">Value to pass</param>
    /// <returns></returns>
    private bool isEven(int value) {
        return value % 2 == 0;
    }
}

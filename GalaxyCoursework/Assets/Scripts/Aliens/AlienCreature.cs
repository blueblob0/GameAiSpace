using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* The super class that will handle all the characteristics
 * of the alien creatures that will spawn, all the random spawning variables
 * like arms, legs, heads will happen here too, also all of the basic AI will happen here
 *
 * Each alien will have a unique set of arms/legs/heads (set in inspector) but will always spawn
 * between 1 & 10 for example, then these creatures can be 'copied' to populate a planet
 */

public class AlienCreature : MonoBehaviour {

    //Set in inspector
    public GameObject bodyPrefab;
    public GameObject headPrefab;
    public GameObject armPrefab;
    public GameObject legPrefab;

    public string creatureName = "NAME_SET_ON_START";
    public string creatureType = "NO_TYPE";

    public ushort intelligence = 0;
    public ushort strength = 0;
    public ushort speed = 0;

    //Make sure the creature doesn't spawn in again
    private bool spawned = false;

    //The rot val of the arms
    private float rotSpeedArm = 30;
    private float armTotalRot = 0;
    //The rot val of the legs
    private float rotSpeedLeg = 30;
    private float legTotalRot = 0;

    //The script the body contains
    private AlienBody bodyScript;

    //An array of potential name parts
    string[] nameParts = {"si", "la", "ti", "aa", "ul", "er", "ta", "ei",
                          "ae", "ui", "lo", "ka", "pi", "cc", "sc", "br",
                          "fj", "or", "nj", "st", "th", "yu", "pt", "kl",
                          "cl", "ph", "pho", "ri", "we", "gh", "io", "ao",
                          "nm", "mm", "nn", "jy", "fv", "vv", "tb", "lk",
                          "ri", "ru", "lar", "ij"};

    /// <summary>
    /// Will spawn the creature into the game world
    /// </summary>
    protected virtual void Start() {
        //Set the name
        creatureName = "";
        int nameLength = Random.Range(2, 5);
        for(int i = 0; i < nameLength; i++) {
            creatureName += nameParts[Random.Range(0, nameParts.Length)];
        }
        creatureName = upperCaseFirst(creatureName);
        //Set the name in the inspector
        name = creatureType + ": " + creatureName;

        //Get the object's rotation, easier to spawn everything the same direction
        Quaternion rot = transform.rotation;
        //Reset it to 0
        transform.rotation = new Quaternion();

        //Add the body script
        bodyScript = bodyPrefab.GetComponent<AlienBody>();

        //Get the maximum number of spawn spots
        ushort maxHead = (ushort)bodyScript.getHeadSpotCount();
        ushort maxArm = (ushort)bodyScript.getArmSpotCount();
        ushort maxLeg = (ushort)bodyScript.getLegSpotCount();

        //Create the creature
        createCreature((ushort)Random.Range(1, maxHead + 1), (ushort)Random.Range(2, maxArm + 1), (ushort)Random.Range(2, maxLeg + 1));

        //Now the creature has been created, re apply the rotation
        transform.rotation = rot;
    }

    /// <summary>
    /// Update the creature to peform movement ect.
    /// </summary>
    protected virtual void Update() {
        /*
        //Rotate the arms
        bodyScript.rotateArms(0, rotSpeedArm * Time.deltaTime, 0);
        armTotalRot += rotSpeedArm * Time.deltaTime;
        if(armTotalRot > 30) {
            rotSpeedArm *= -1;
        } else if(armTotalRot < -30) {
            rotSpeedArm *= -1;
        }

        //Rotate the legs
        bodyScript.rotateLegs(rotSpeedArm * Time.deltaTime, 0, 0);
        legTotalRot += rotSpeedLeg * Time.deltaTime;
        if(legTotalRot > 30) {
            rotSpeedLeg *= -1;
        } else if(legTotalRot < -30) {
            rotSpeedLeg *= -1;
        }
        */
    }

    /// <summary>
    /// Spawns in the creature
    /// </summary>
    /// <param name="maxHeads">The maximum amount of heads the creature can have</param>
    /// <param name="maxArms">The maximum amount of arms the creature can have</param>
    /// <param name="maxLegs">The maximum amount of legs the creature can have</param>
    protected void createCreature(ushort maxHeads = 1, ushort maxArms = 2, ushort maxLegs = 2) {
        if(!spawned) {
            //Spawn in the body
            GameObject body = GameObject.Instantiate<GameObject>(bodyPrefab);
            body.transform.SetParent(transform);

            //Spawn in the Head(s)
            for(int i = 0; i < maxHeads; i++) {
                //Get the body's children
                for(int j = 0; j < body.transform.childCount; j++) {
                    //Check if the spot is a 'Head' spot and that it is free
                    GameObject headSpot = body.transform.GetChild(j).gameObject;
                    if(headSpot.name == "Head" + i && headSpot.transform.childCount == 0) {
                        //Spawn in the head
                        GameObject head = GameObject.Instantiate<GameObject>(headPrefab);
                        //Set the parent
                        head.transform.SetParent(headSpot.transform);
                        //Make sure it is at 0,0,0 in relation to the parent
                        head.transform.localPosition = new Vector3();
                        head.transform.localRotation = Quaternion.identity;
                    }
                }
            }

            //Spawn in the Arms
            for(int i = 0; i < maxArms; i++) {
                //Get the body's children
                for(int j = 0; j < body.transform.childCount; j++) {
                    //Check if the spot is an 'Arm' spot and that it is free
                    GameObject armSpot = body.transform.GetChild(j).gameObject;
                    if(armSpot.name == "Arm" + i && armSpot.transform.childCount == 0) {
                        //Spawn in the arm
                        GameObject arm = GameObject.Instantiate<GameObject>(armPrefab);
                        //Set the parent
                        arm.transform.SetParent(armSpot.transform);
                        //Make sure it is at 0,0,0 in relation to the parent
                        arm.transform.localPosition = new Vector3();
                        arm.transform.localRotation = Quaternion.identity;
                    }
                }
            }

            //Spawn in the Legs
            for(int i = 0; i < maxArms; i++) {
                //Get the body's children
                for(int j = 0; j < body.transform.childCount; j++) {
                    //Check if the spot is an 'Leg' spot and that it is free
                    GameObject legSpot = body.transform.GetChild(j).gameObject;
                    if(legSpot.name == "Leg" + i && legSpot.transform.childCount == 0) {
                        //Spawn in the leg
                        GameObject leg = GameObject.Instantiate<GameObject>(legPrefab);
                        //Set the parent
                        leg.transform.SetParent(legSpot.transform);
                        //Make sure it is at 0,0,0 in relation to the parent
                        leg.transform.localPosition = new Vector3();
                        leg.transform.localRotation = Quaternion.identity;
                    }
                }
            }

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

    /// <summary>
    /// Makes the first letter of the string upper case then returns that string
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    private string upperCaseFirst(string s) {
        // Check for empty string.
        if(string.IsNullOrEmpty(s)) {
            return string.Empty;
        }
        // Return char and concat substring.
        return char.ToUpper(s[0]) + s.Substring(1);
    }
}

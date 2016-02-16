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

    //The script the body contains
    private AlienBody bodyScript;

    //An array of potential name parts
    string[] nameParts = {"si", "la", "ti", "aa", "ul", "er", "ta", "ei",
                          "ae", "ui", "lo", "ka", "pi", "cc", "sc", "br",
                          "fj", "or", "nj", "st", "th", "yu", "pt", "kl",
                          "cl", "ph", "pho", "ri", "we", "gh", "io", "ao",
                          "nm", "mm", "nn", "jy", "fv", "vv", "tb", "lk"};

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

        //Get the object's rotation
        Quaternion rot = transform.rotation;
        //Reset it to 0
        transform.rotation = new Quaternion();

        //Get a reference to the script and init it
        bodyScript = bodyPrefab.GetComponent<AlienBody>();
        bodyScript.initialize();

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
        foreach(GameObject arm in arms) {
            arm.transform.Rotate(new Vector3(0, rotSpeedArm * Time.deltaTime, 0));
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
                bodyScript.addHead(headPrefab);
            }

            //Spawn in the Arms
            for(int i = 0; i < maxArms; i++) {
                bodyScript.addArm(armPrefab);
            }

            //Spawn in the Legs
            for(int i = 0; i < maxArms; i++) {
                bodyScript.addLeg(legPrefab);
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

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
        int nameLength = Random.Range(2, 5);
        for(int i = 0; i < nameLength; i++) {
            creatureName += nameParts[Random.Range(0, nameParts.Length)];
        }
        creatureName = upperCaseFirst(creatureName);

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

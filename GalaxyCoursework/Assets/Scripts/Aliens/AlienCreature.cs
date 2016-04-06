using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* The super class that will handle all the characteristics
 * of the alien creatures that will spawn, all the random spawning variables
 * like arms, legs, heads will happen here too
 *
 * Although the AI is handled in a different class the interaction between other creatures will happen here
 * i.e. finding a target, following a group ect.
 *
 * Each alien will have a unique set of arms/legs/heads (set in inspector) but will always spawn
 * between 1 & 10 for example, then these creatures can be 'copied' to populate a planet
 */

public class AlienCreature : AlienAI {

    //Set in inspector
    public float baseIntelligence;  //Base value, will increase per head
    public float baseStrenth;       //Base value, will increase per arm
    public float baseSpeed;         //Base value, will increase per leg
    public float baseDodge;         //Base value, will increase per wing

    //Set in inspector
    public float reproductionChance;    //The chance of reproduction
    public float reproductionTimer;     //How long between each reproduction attempt (seconds)

    //Set in inspector
    public GameObject bodyPrefab;
    public GameObject headPrefab;
    public GameObject armPrefab;
    public GameObject legPrefab;
    public GameObject wingPrefab;

    //Set in inspector
    public string creatureType = "NO_TYPE";

    //How long has passed
    private float reproductionTimePassed;

    //List of other creatures
    private List<GameObject> otherCreatures = new List<GameObject>();

    //The creature's individual name
    private string creatureName = "NAME_SET_ON_START";

    //Make sure the creature doesn't spawn in again
    private bool spawned = false;

    //Used to check if the creature is scaled
    private bool isScaled = false;

    //The script the body contains
    private AlienBody bodyScript;

    //Keep track of the limb count
    private ushort headCount;
    private ushort armCount;
    private ushort legCount;
    private ushort wingCount;

    //An array of potential name parts
    string[] nameParts = {"si", "la", "ti", "aa", "ul",
                          "er", "ta", "ei", "ae", "ui",
                          "lo", "ka", "pi", "cc", "sc",
                          "br", "fj", "or", "nj", "st",
                          "th", "yu", "pt", "kl", "cl",
                          "ph", "pho", "ri", "we", "gh",
                          "io", "ao", "nm", "mm", "nn",
                          "jy", "fv", "vv", "tb", "lk",
                          "ri", "ru", "lar", "ij"};

    /// <summary>
    /// Will spawn the creature into the game world
    /// </summary>
    public override void Start() {
        base.Start();

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
        transform.rotation = Quaternion.identity;

        //Add the body script
        bodyScript = bodyPrefab.GetComponent<AlienBody>();

        //Get the maximum number of spawn spots
        ushort maxHead = (ushort)bodyScript.getHeadSpotCount();
        ushort maxArm = (ushort)bodyScript.getArmSpotCount();
        ushort maxLeg = (ushort)bodyScript.getLegSpotCount();
        ushort maxWing = (ushort)bodyScript.getWingSpotCount();

        //Create the creature
        createCreature((ushort)Random.Range(1, maxHead + 1), (ushort)Random.Range(2, maxArm + 1), (ushort)Random.Range(2, maxLeg + 1), (ushort)Random.Range(2, maxWing + 1));

        //Now the creature has been created, re apply the rotation
        transform.rotation = rot;

        //Init
        reproductionTimePassed = 0;
    }

    /// <summary>
    /// Update the creature to peform movement ect.
    /// </summary>
    public override void Update() {
        //Wander around
        setSteering(wander());

        //Increment the time passed
        reproductionTimePassed += Time.deltaTime;
        //Check the cool down
        if(reproductionTimePassed >= reproductionTimer) {
            //Reset interval
            reproductionTimePassed = 0;
            //Try and reproduce
            int chance = Random.Range(0, 100);
            if(reproductionChance >= chance) {
                reproduce();
            }
        }

        //Call last to apply velocity updates
        base.Update();
    }

    /// <summary>
    /// Delete this creature's limb count and replace them with the copied creature
    /// </summary>
    /// <param name="creature"></param>
    public void copyCreature(AlienCreature creature) {
        //Make sure the creature type is the same
        if(creatureType != creature.creatureType) {
            return;
        }

        //Store, then reset the rot
        Quaternion rot = transform.rotation;
        transform.rotation = Quaternion.identity;

        //Reset the spawned in limbs
        spawned = false;
        int count = transform.childCount;
        for(int i = 0; i < count; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }

        //Create a new creature with the script provided
        createCreature(creature.getHeadCount(), creature.getArmCount(), creature.getLegCount(), creature.getWingCount());

        //Return the rotation
        transform.rotation = rot;

        //Populate the list
        otherCreatures = new List<GameObject>(creature.getCreatureList());
        //Add the 'parent'
        addCreature(creature.gameObject);
        //Make sure it doesn't contain itself
        otherCreatures.Remove(gameObject);
    }

    /// <summary>
    /// Add a creature to this creature's list of known creatures
    /// </summary>
    /// <param name="newCreature">Creature to add</param>
    public void addCreature(GameObject newCreature) {
        if(!otherCreatures.Contains(newCreature) && newCreature != null && newCreature != gameObject) {
            otherCreatures.Add(newCreature);
        }
    }

    /// <summary>
    /// Returns the list of creatures this creature has
    /// </summary>
    /// <returns></returns>
    public List<GameObject> getCreatureList() {
        return otherCreatures;
    }

    /// <summary>
    /// Returns the current amount of heads this creature has
    /// </summary>
    /// <returns></returns>
    public ushort getHeadCount() {
        return headCount;
    }

    /// <summary>
    /// Returns the current amount of arms this creature has
    /// </summary>
    /// <returns></returns>
    public ushort getArmCount() {
        return armCount;
    }

    /// <summary>
    /// Returns the current amount of legs this creature has
    /// </summary>
    /// <returns></returns>
    public ushort getLegCount() {
        return legCount;
    }

    /// <summary>
    /// Returns the current amount of wings this creature has
    /// </summary>
    /// <returns></returns>
    public ushort getWingCount() {
        return wingCount;
    }

    /// <summary>
    /// Spawns in the creature
    /// </summary>
    /// <param name="maxHeads">The maximum amount of heads the creature can have</param>
    /// <param name="maxArms">The maximum amount of arms the creature can have</param>
    /// <param name="maxLegs">The maximum amount of legs the creature can have</param>
    protected void createCreature(ushort maxHeads = 1, ushort maxArms = 2, ushort maxLegs = 2, ushort maxWings = 0) {
        if(!spawned) {
            //Set the limb counts
            headCount = maxHeads;
            armCount = maxArms;
            legCount = maxLegs;

            //Spawn in the body
            GameObject body = GameObject.Instantiate<GameObject>(bodyPrefab);
            body.transform.SetParent(transform);
            body.transform.localPosition = Vector3.zero;

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
                        head.transform.localPosition = Vector3.zero;
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
                        arm.transform.localPosition = Vector3.zero;
                        arm.transform.localRotation = Quaternion.identity;
                    }
                }
            }

            //Spawn in the Legs
            for(int i = 0; i < maxLegs; i++) {
                //Get the body's children
                for(int j = 0; j < body.transform.childCount; j++) {
                    //Check if the spot is a 'Leg' spot and that it is free
                    GameObject legSpot = body.transform.GetChild(j).gameObject;
                    if(legSpot.name == "Leg" + i && legSpot.transform.childCount == 0) {
                        //Spawn in the leg
                        GameObject leg = GameObject.Instantiate<GameObject>(legPrefab);
                        //Set the parent
                        leg.transform.SetParent(legSpot.transform);
                        //Make sure it is at 0,0,0 in relation to the parent
                        leg.transform.localPosition = Vector3.zero;
                        leg.transform.localRotation = Quaternion.identity;
                    }
                }
            }

            //Spawn in the wings
            for(int i = 0; i < maxWings; i++) {
                //Get the body's children
                for(int j = 0; j < body.transform.childCount; j++) {
                    //Check if the spot is a 'Wing' spot and that it is free
                    GameObject wingSpot = body.transform.GetChild(j).gameObject;
                    if(wingSpot.name == "Wing" + i && wingSpot.transform.childCount == 0) {
                        //Spawn in the wing
                        GameObject wing = GameObject.Instantiate<GameObject>(wingPrefab);
                        //Set the parent
                        wing.transform.SetParent(wingSpot.transform);
                        //Make sure it is at 0,0,0 in relation to the parent
                        wing.transform.localPosition = Vector3.zero;
                        wing.transform.localRotation = Quaternion.identity;
                    }
                }
            }

            //Creature has now been spawned
            spawned = true;
        }
    }

    /// <summary>
    /// Attempts to make another copy of this object
    /// </summary>
    protected void reproduce() {
        //Create a copy of this gameObject
        GameObject spawn = GameObject.Instantiate(gameObject);
        //Copy this creature's values
        spawn.GetComponent<AlienCreature>().copyCreature(this);
        //Add it onto the list
        addCreature(spawn);
        //Make the other creatures aware of this new creature
        foreach(GameObject creature in otherCreatures) {
            creature.GetComponent<AlienCreature>().addCreature(spawn);
        }
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

    //REPLACE WITH BETTER FUNCTIONALITY (CreatureSpawner.cs)
    public bool isSpawned() {
        return spawned;
    }

    public void hasBeenScaled(bool scaled) {
        isScaled = scaled;
    }
    public bool checkScaled() {
        return isScaled;
    }
}

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

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class AlienCreature : AlienAI {

    //Set in inspector
    public GameObject[] bodyPrefabs;
    public GameObject[] headPrefabs;
    public GameObject[] armPrefabs;
    public GameObject[] legPrefabs;
    public GameObject[] wingPrefabs;

    //List of potential targets
    protected List<AlienCreature> nearTargets = new List<AlienCreature>();
    //The agents current target
    protected AlienCreature target;
    //The collider reference
    private SphereCollider targetDetectCollider;

    //List of other creatures
    private List<GameObject> otherCreatures = new List<GameObject>();

    //How much damage the agent can take before dying
    private float health;
    //The accuracy of wether to flee or engage
    private float intelligenceModifier;  //Will increase per head
    //How much damage the agent deals
    private float strengthModifier;      //Will increase per arm
    //How fast the agent moves
    private float speedModifier;         //Will increase per leg
    //Likley hood of avoid damage
    private float dodgeModifier;         //Will increase per wing

    //If the agent can attack
    private bool canAttack;

    //The species of creature
    private string creatureSpecies = "NO_SPECIES";
    //The creature's individual name
    private string creatureName = "NO_NAME";

    //Make sure the creature doesn't spawn in again
    private bool spawned = false;

    //Used to check if the creature is scaled
    private bool isScaled = false;

    //The chance of reproduction
    private float reproductionChance;
    //How long between each reproduction attempt (seconds)
    private float reproductionTimer;
    //How long has passed
    private float reproductionTimePassed;

    //The script the body contains
    private AlienBody bodyScript;

    //Keep track of the limb count
    private ushort headCount;
    private ushort armCount;
    private ushort legCount;
    private ushort wingCount;

    //An array of potential name parts
    string[] nameParts = {"si", "la",  "ti",  "aa", "ul",
                          "er", "ta",  "ei",  "ae", "ui",
                          "lo", "ka",  "pi",  "cc", "sc",
                          "br", "fj",  "or",  "nj", "st",
                          "th", "yu",  "pt",  "kl", "cl",
                          "ph", "pho", "ri",  "we", "gh",
                          "io", "ao",  "nm",  "mm", "nn",
                          "jy", "fv",  "vv",  "tb", "lk",
                          "ri", "ru",  "lar", "ij"};

    //An array of potential species parts
    string[] speciesParts = {"hu", "man", "xe", "mo", "zi",
                             "yu", "lo",  "pa", "th", "ad"};

    /// <summary>
    /// Will spawn the creature into the game world
    /// </summary>
    public override void Start() {
        base.Start();

        //Set the stat values
        health = 100;
        intelligenceModifier = 40;
        strengthModifier = 20;
        speedModifier = 5;
        dodgeModifier = 20;

        canAttack = true;

        //Get some random reproduction values
        reproductionChance = Random.Range(25, 71);
        reproductionTimer = Random.Range(30, 150);

        //Set the species
        creatureSpecies = "";
        int speciesLength = Random.Range(2, 6);
        string firstHalf = "";
        string secondHalf = "";
        for(int i = 0; i < speciesLength; i++) {
            firstHalf += speciesParts[Random.Range(0, speciesParts.Length)];
        }
        for(int i = 0; i < speciesLength; i++) {
            secondHalf += speciesParts[Random.Range(0, speciesParts.Length)];
        }
        creatureSpecies = upperCaseFirst(firstHalf) + " " + upperCaseFirst(secondHalf);

        //Set the name
        creatureName = "";
        int nameLength = Random.Range(2, 4);
        for(int i = 0; i < nameLength; i++) {
            creatureName += nameParts[Random.Range(0, nameParts.Length)];
        }
        creatureName = upperCaseFirst(creatureName);
        //Set the object name
        name = creatureSpecies + ": " + creatureName;

        //Get the object's rotation, easier to spawn everything the same direction
        Quaternion rot = transform.rotation;
        //Reset it to 0
        transform.rotation = Quaternion.identity;

        //Select the body to be used
        GameObject bodyToUse = bodyPrefabs[Random.Range(0, bodyPrefabs.Length)];

        //Add the body script
        bodyScript = bodyToUse.GetComponent<AlienBody>();

        //Get the maximum number of spawn spots
        ushort maxHead = (ushort)bodyScript.getHeadSpotCount();
        ushort maxArm = (ushort)bodyScript.getArmSpotCount();
        ushort maxLeg = (ushort)bodyScript.getLegSpotCount();
        ushort maxWing = (ushort)bodyScript.getWingSpotCount();

        //Create the creature
        createCreature(bodyToUse, (ushort)Random.Range(1, maxHead + 1), (ushort)Random.Range(2, maxArm + 1), (ushort)Random.Range(2, maxLeg + 1), (ushort)Random.Range(2, maxWing + 1));

        //Now the creature has been created, re apply the rotation
        transform.rotation = rot;

        //Init
        reproductionTimePassed = 0;

        //Set the collider size
        targetDetectCollider = GetComponent<SphereCollider>();
        targetDetectCollider.radius = 30;
        targetDetectCollider.isTrigger = true;
    }

    /// <summary>
    /// Update the creature to peform movement ect.
    /// </summary>
    public override void Update() {
        base.Update();

        //DEBUG-------------------------------------
        if(Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(reproduce());
        }
        //------------------------------------------

        //Check the health
        if(health <= 0) {
            Destroy(gameObject);
        }

        //Increment the time passed
        reproductionTimePassed += Time.deltaTime;
        //Check the cool down
        if(reproductionTimePassed >= reproductionTimer) {
            //Reset interval
            reproductionTimePassed = 0;
            //Try and reproduce
            int chance = Random.Range(0, 100);
            if(reproductionChance >= chance) {
                StartCoroutine(reproduce());

                //Add on a random number so reproduction isnt synced
                reproductionTimePassed += Random.Range(1, 11);
            }
        }
    }

    /// <summary>
    /// Delete this creature and replace it with a copy
    /// </summary>
    /// <param name="creature">Creature to copy</param>
    public void copyCreature(AlienCreature creature) {
        //Store, then reset the rot
        Quaternion rot = transform.rotation;
        transform.rotation = Quaternion.identity;

        //Delete the body
        int count = transform.childCount;
        for(int i = 0; i < count; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }

        //Add the body of the parent creature
        GameObject newBody = GameObject.Instantiate(creature.getDuplicateBody());
        newBody.transform.SetParent(transform);
        newBody.transform.localPosition = Vector3.zero;

        //Return the rotation to its default
        transform.rotation = rot;

        //Change the species to parent
        creatureSpecies = creature.getSpecies();
        name = creatureSpecies + ": " + creatureName;

        //Populate the list
        otherCreatures = new List<GameObject>(creature.getCreatureList());
        //Add the 'parent'
        addCreature(creature.gameObject);
        //Make sure it doesn't contain itself
        otherCreatures.Remove(gameObject);

        //Set the modifiers
        intelligenceModifier = creature.getIntelligence();
        strengthModifier = creature.getStrength();
        speedModifier = creature.getSpeedModifier();
        dodgeModifier = creature.getDdogeChance();
        //Increase the max speed
        changeMaxSpeed(getMaxSpeed() + speedModifier);
    }

    /// <summary>
    /// Returns the creatue's species
    /// </summary>
    /// <returns></returns>
    public string getSpecies() {
        return creatureSpecies;
    }

    /// <summary>
    /// Returns the body of this creature and its limbs
    /// </summary>
    /// <returns></returns>
    public GameObject getDuplicateBody() {
        return transform.GetChild(0).gameObject;
    }

    /// <summary>
    /// Returns the list containing other known creatures
    /// </summary>
    /// <returns></returns>
    public List<GameObject> getCreatureList() {
        return otherCreatures;
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
    /// Called when an object enters the collider
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other) {
        if(other.gameObject != gameObject) {
            StartCoroutine(checkIfTarget(other.gameObject));
        }
    }

    /// <summary>
    /// Called when an object leaves the collider
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerExit(Collider other) {
        nearTargets.Remove(other.GetComponent<AlienCreature>());
    }

    /// <summary>
    /// Returns this creatures intelligence modifier
    /// </summary>
    /// <returns></returns>
    public float getIntelligence() {
        return intelligenceModifier;
    }

    /// <summary>
    /// Returns this creatures strength modifier
    /// </summary>
    /// <returns></returns>
    public float getStrength() {
        return strengthModifier;
    }

    /// <summary>
    /// Returns this creatures speed modifier
    /// </summary>
    /// <returns></returns>
    public float getSpeedModifier() {
        return speedModifier;
    }

    /// <summary>
    /// Returns this creatures dodge chance modifier
    /// </summary>
    /// <returns></returns>
    public float getDdogeChance() {
        return dodgeModifier;
    }

    /// <summary>
    /// Tries to damage this creature, has a chance to miss bassed off of dodge
    /// </summary>
    /// <param name="amount">The incomming damage amount</param>
    public void receiveDamage(float amount) {
        float val = Random.value * 100;
        if(val > dodgeModifier) {
            if(health - amount > 0) {
                health -= amount;
            } else {
                health = 0;
            }
        }
    }

    /// <summary>
    /// Returns a value on 0 - 100 if the agent should attack the creature, 100 being that it should definitley attack
    /// </summary>
    /// <param name="creature">The target</param>
    /// <returns></returns>
    protected float shouldAttack(AlienCreature creature) {
        //The stats of this creature comprised into one variable
        float stats;
        //The stats of the other creature

        //Off set by intelligence

        //Return the likleyhood
        return 0; //nothing for now
    }

    /// <summary>
    /// Helper function to start the corutine to damage creatures
    /// </summary>
    /// <param name="creature">The creature to damage</param>
    protected void damageCreature(AlienCreature creature) {
        if(canAttack){
            StartCoroutine(applyDamage(creature));
        }
    }

    /// <summary>
    /// Applies the damage to the target
    /// </summary>
    /// <param name="creature"></param>
    /// <returns></returns>
    private IEnumerator applyDamage(AlienCreature creature) {
        canAttack = false;
        float damage = (5 + strengthModifier) / 3;
        creature.receiveDamage(damage);
        yield return new WaitForSeconds(1.5f); //random value for now
        canAttack = true;
    }

    /// <summary>
    /// Called to check if the target can be added to the list.
    /// Needs to be an enumerator because of how reproduction works
    /// </summary>
    /// <param name="potentialTarget">Game object to check</param>
    /// <returns></returns>
    private IEnumerator checkIfTarget(GameObject potentialTarget) {
        //Wait one frame to avoid the reproduction bug
        yield return null;

        //Check the target
        if(potentialTarget != null && potentialTarget.GetComponent<AlienCreature>() != null) {
            AlienCreature targetScript = potentialTarget.GetComponent<AlienCreature>();
            if(!string.Equals(targetScript.getSpecies(), getSpecies())) {
                if(!nearTargets.Contains(targetScript)) {
                    nearTargets.Add(targetScript);
                }
            }
        }
    }

    /// <summary>
    /// Spawns in the creature
    /// </summary>
    /// <param name="body">The body to attach the limbs to</param>
    /// <param name="maxHeads">The maximum amount of heads the creature can have</param>
    /// <param name="maxArms">The maximum amount of arms the creature can have</param>
    /// <param name="maxLegs">The maximum amount of legs the creature can have</param>
    /// <param name="maxWings">The maximum amount of wings the creature can have</param>
    private void createCreature(GameObject bodyPrefab, ushort maxHeads = 1, ushort maxArms = 2, ushort maxLegs = 2, ushort maxWings = 0) {
        if(!spawned) {
            //Set the limb counts
            headCount = maxHeads;
            armCount = maxArms;
            legCount = maxLegs;

            //Spawn in the body
            GameObject body = GameObject.Instantiate<GameObject>(bodyPrefab);
            body.transform.SetParent(transform);
            body.transform.localPosition = Vector3.zero;

            //Make the body a random colour
            body.transform.FindChild("Body").GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value);

            //Get the head to use
            GameObject headToUse = headPrefabs[Random.Range(0, headPrefabs.Length)];
            //Spawn in the Head(s)
            for(int i = 0; i < maxHeads; i++) {
                //Get the body's children
                for(int j = 0; j < body.transform.childCount; j++) {
                    //Check if the spot is a 'Head' spot and that it is free
                    GameObject headSpot = body.transform.GetChild(j).gameObject;
                    if(headSpot.name == "Head" + i && headSpot.transform.childCount == 0) {
                        //Spawn in the head
                        spawnLimb(headSpot.transform, headToUse);
                    }
                }
            }
            //Increase intelligence based off of head count
            intelligenceModifier += maxHeads * 10;

            //Get the arm to use
            GameObject armToUse = armPrefabs[Random.Range(0, armPrefabs.Length)];
            //Spawn in the Arms
            for(int i = 0; i < maxArms; i++) {
                //Get the body's children
                for(int j = 0; j < body.transform.childCount; j++) {
                    //Check if the spot is an 'Arm' spot and that it is free
                    GameObject armSpot = body.transform.GetChild(j).gameObject;
                    if(armSpot.name == "Arm" + i && armSpot.transform.childCount == 0) {
                        //Spawn in the arm
                        spawnLimb(armSpot.transform, armToUse);
                    }
                }
            }
            //Increase strength based off of the arm count
            strengthModifier += maxArms * 5;

            //Get the leg to use
            GameObject legToUse = legPrefabs[Random.Range(0, legPrefabs.Length)];
            //Spawn in the Legs
            for(int i = 0; i < maxLegs; i++) {
                //Get the body's children
                for(int j = 0; j < body.transform.childCount; j++) {
                    //Check if the spot is a 'Leg' spot and that it is free
                    GameObject legSpot = body.transform.GetChild(j).gameObject;
                    if(legSpot.name == "Leg" + i && legSpot.transform.childCount == 0) {
                        //Spawn in the leg
                        spawnLimb(legSpot.transform, legToUse);
                    }
                }
            }
            //Increase speed based off of the leg count (for the reproducted aliens)
            speedModifier += maxLegs;
            //Increase the max speed
            changeMaxSpeed(getMaxSpeed() + speedModifier);

            //Get the wing to use
            GameObject wingToUse = wingPrefabs[Random.Range(0, wingPrefabs.Length)];
            //Spawn in the wings
            for(int i = 0; i < maxWings; i++) {
                //Get the body's children
                for(int j = 0; j < body.transform.childCount; j++) {
                    //Check if the spot is a 'Wing' spot and that it is free
                    GameObject wingSpot = body.transform.GetChild(j).gameObject;
                    if(wingSpot.name == "Wing" + i && wingSpot.transform.childCount == 0) {
                        //Spawn the wing
                        spawnLimb(wingSpot.transform, wingToUse);
                    }
                }
            }
            //Increase dodge based off of the wing count
            dodgeModifier += wingCount * 2;

            //Creature has now been spawned
            spawned = true;
        }
    }

    /// <summary>
    /// Spawns a limb on the creature
    /// </summary>
    /// <param name="parent">The transform to attack the limb to</param>
    /// <param name="limbPrefab">The prefab to instantiate</param>
    private void spawnLimb(Transform parent, GameObject limbPrefab) {
        //Spawn in the wing
        GameObject limb = GameObject.Instantiate<GameObject>(limbPrefab);
        //Set the parent
        limb.transform.SetParent(parent);
        //Make sure it is at 0,0,0 in relation to the parent
        limb.transform.localPosition = Vector3.zero;
        limb.transform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// Attempts to make another copy of this object
    /// </summary>
    private IEnumerator reproduce() {
        //Create a copy of this gameObject
        GameObject spawn = GameObject.Instantiate(gameObject);
        //Make the spawn apear in a random position near the creature
        spawn.transform.position += new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
        //Wait a frame for the spawn to happen before copying
        yield return null;
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

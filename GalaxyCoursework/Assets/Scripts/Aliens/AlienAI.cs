﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * This class is used to control the behavior tree for each agent
 * This class also has all of the characteristics for the alien creatures
 */

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CharacterController))]
public class AlienAI : MonoBehaviour {

    //Set in inspector
    public float acceleration = 0.1f;  //How quickly the speed changes
    public float currentSpeed = 5;     //How fast the agent is current moving
    public float maxSpeed = 10;        //The fastest this agent can currently go
    public float trueMaxSpeed = 10;    //the fastest possible speed this agent can go at any time
    public float mass = 20;            //How heavy the agent is (this makes the steering more smooth)

    //Set in inspector
    public GameObject[] bodyPrefabs;
    public GameObject[] headPrefabs;
    public GameObject[] armPrefabs;
    public GameObject[] legPrefabs;
    public GameObject[] wingPrefabs;

    //List of potential targets
    protected List<AlienAI> nearTargets = new List<AlienAI>();
    //The agents current target
    protected AlienAI target;
    //The collider reference
    protected SphereCollider targetDetectCollider;

    //List of other creatures
    protected List<GameObject> otherCreatures = new List<GameObject>();

    //How much damage the agent can take before dying
    protected float health;
    protected float maxHealth;
    //The accuracy of wether to flee or engage
    protected float intelligenceModifier;  //Will increase per head
    //How much damage the agent deals
    protected float strengthModifier;      //Will increase per arm
    //How fast the agent moves
    protected float speedModifier;         //Will increase per leg
    //Likley hood of avoid damage
    protected float dodgeModifier;         //Will increase per wing

    //If the agent can attack
    protected bool canAttack;
    //The speed the agent wants to go
    private float targetSpeed;

    //Character controller ref
    private CharacterController controller;

    //Just to stop the creature 'spawning' twice
    bool spawned;

    //How much to scale the speed down by (when on planets)
    private float speedScale;

    //The species of creature
    private string creatureSpecies = "NO_SPECIES";
    //The creature's individual name
    private string creatureName = "NO_NAME";

    //How long between each reproduction attempt (seconds)
    private float reproductionTimer;
    //How long has passed
    private float reproductionTimePassed;
    //The target to try and reproduce with
    private AlienAI reproductionTarget;

    //Running fast will drain the energy
    private float energy;
    private float maxEnergy;
    //At what speed to drain energy
    private float energyThreshold;         

    //How much each steering behaviour will affect the total flocking steering
    private float allignmentWeight;
    private float cohesionWeight;
    private float seperationWeight;
    //Distances on how far to compute the specific steering behaviours
    private float allignmentDistance;
    private float cohesionDistance;
    private float seperationDistance;
    //Variables to control how long to wait for weight changing
    private float weightChangeWait;
    private float weightChangePass;

    //The velocity vector
    private Vector3 velocity;
    //Used to apply steering
    private Vector3 steering;
    private Vector3 desiredVelocity;

    //The script the body contains
    private AlienBody bodyScript;

    //Keep track of the limb count
    private ushort headCount;
    private ushort armCount;
    private ushort legCount;
    private ushort wingCount;

    //The main selector of the behavior tree
    private Selector mainSelector;
    //Tasks for the main selector
    private EscapeSequence escape;
    private RecoupSequence recoup;
    private FightSelector fight;
    private IdleSelector idle;

    //An array of potential name parts
    private string[] nameParts = {"si", "la",  "ti",  "aa", "ul",
                                  "er", "ta",  "ei",  "ae", "ui",
                                  "lo", "ka",  "pi",  "cc", "sc",
                                  "br", "fj",  "or",  "nj", "st",
                                  "th", "yu",  "pt",  "kl", "cl",
                                  "ph", "pho", "ri",  "we", "gh",
                                  "io", "ao",  "nm",  "mm", "nn",
                                  "jy", "fv",  "vv",  "tb", "lk",
                                  "ri", "ru",  "lar", "ij"};

    //An array of potential species parts
    private string[] speciesParts = {"hu", "man", "xe", "mo", "zi",
                                     "yu", "lo",  "pa", "th", "ad"};


    // Use this for initialization
    public virtual void Start () {
        //Has not yet been spawned
        spawned = false;

        //Construct the main selector
        mainSelector = new Selector(this);

        //Construct the tasks
        escape = new EscapeSequence(this);
        recoup = new RecoupSequence(this);
        fight = new FightSelector(this);
        idle = new IdleSelector(this);

        //Add nodes in order of importance
        mainSelector.addChild(escape);
        mainSelector.addChild(recoup);
        mainSelector.addChild(fight);
        mainSelector.addChild(idle);

        //Set the stat values
        health = 100;
        maxHealth = health;
        intelligenceModifier = 40;
        strengthModifier = 20;
        speedModifier = 5;
        dodgeModifier = 20;
        energy = 100;
        maxEnergy = energy;
        energyThreshold = 5;

        //Let the agent attack
        canAttack = true;

        //Init the velocity vectors
        velocity = transform.forward * currentSpeed * Time.deltaTime;
        steering = Vector3.zero;
        desiredVelocity = Vector3.zero;

        //Init weights
        allignmentWeight = 1;
        cohesionWeight  = 1;
        seperationWeight = 1;

        //Init distances
        allignmentDistance = 8;
        cohesionDistance = 12;
        seperationDistance = 10;

        //Init timer
        weightChangeWait = Random.Range(3, 10);
        weightChangePass = 0;

        targetSpeed = currentSpeed;

        //Get the controllre
        controller = GetComponent<CharacterController>();

        //Set the default value
        speedScale = 1;

        //Get some random reproduction values
        reproductionTimer = Random.Range(30, 150);
        reproductionTarget = null;

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
	
	// Update is called once per frame
	public virtual void Update () {
        //Check the health
        if(health <= 0) {
            Destroy(gameObject);
        }

        //Increment the time passed
        if(reproductionTimePassed < reproductionTimer) {
            reproductionTimePassed += Time.deltaTime;
        }

        //Make sure to remove 'dead' creatures from the list
        otherCreatures.Remove(null);
        nearTargets.Remove(null);

        //Adjust the weights on the flocking
        if(weightChangePass >= weightChangeWait) {
            weightChangePass = 0;
            weightChangeWait = Random.Range(3, 10);

            allignmentWeight = Random.value;
            cohesionWeight = Random.value;
            seperationWeight = Random.value;
        }
        weightChangePass += Time.deltaTime;

        //Activate the selector
        mainSelector.activate();

        //Only if the agent wants to move
        if(currentSpeed > 0) {
            //Make the steering smooth
            steering /= mass;

            //Add to the velocity
            if(!float.IsNaN(steering.x) && !float.IsNaN(steering.y) && !float.IsNaN(steering.z)) {
                velocity += steering;
            } else {
                steering = Vector3.zero;
            }

            //Avoid radnom flying ones
            velocity.y = 0;

            //Normalise the desired velocity and add the speed
            velocity = calculateSpeed(velocity);

            //Update the position and look 'forward'
            transform.rotation = Quaternion.LookRotation(velocity);
            controller.Move(velocity);
        }

        //Adjust the speed values
        if(currentSpeed < targetSpeed) {
            currentSpeed += acceleration;
        } else if(currentSpeed > targetSpeed) {
            currentSpeed -= acceleration * 2;
        }
        //0 off the speed
        if(targetSpeed <= 0 && currentSpeed <= 0.1f) {
            currentSpeed = 0;
        }

        //Check to drain energy
        if(currentSpeed > energyThreshold && energy > 0) {
            energy -= 0.1f;
        }
        //When the energy runs out make sure to limit the speed
        if(energy < 0) {
            energy = 0;
            changeMaxSpeed(3);
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
        nearTargets.Remove(other.GetComponent<AlienAI>());
    }

    /// <summary>
    /// Delete this creature and replace it with a copy
    /// </summary>
    /// <param name="creature">Creature to copy</param>
    public void copyCreature(AlienAI creature) {
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
        changeMaxSpeed(getMaximumSpeed() + speedModifier, true);
    }

    /// <summary>
    /// Sets the reproduction target of this agent
    /// </summary>
    /// <param name="target"></param>
    public void setReproductionTarget(AlienAI target) {
        reproductionTarget = target;
    }

    /// <summary>
    /// Returns the reproduction target of this agent
    /// </summary>
    /// <returns></returns>
    public AlienAI getReproductionTarget() {
        return reproductionTarget;
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
    /// Call to set the scale for speed (gets dvidided by amount)
    /// </summary>
    /// <param name="scale">Scale for speed, can't be beloew 1</param>
    public void setSpeedScale(float scale) {
        if(scale < 1) {
            return;
        } else {
            speedScale = scale;
        }
    }

    /// <summary>
    /// Returns the velocity vector of the current agent
    /// </summary>
    /// <returns></returns>
    public Vector3 getVelocity() {
        return velocity;
    }

    /// <summary>
    /// Sets the desired volcity of this agent
    /// </summary>
    /// <param name="velocity"></param>
    public void setDesiredVelocity(Vector3 velocity) {
        desiredVelocity = velocity;
    }

    /// <summary>
    /// Returns the desired velocity of this agent
    /// </summary>
    /// <returns></returns>
    public Vector3 getDesiredVelocity() {
        return desiredVelocity;
    }

    /// <summary>
    /// Returns the health of this agent
    /// </summary>
    /// <returns></returns>
    public float getHealth() {
        return health;
    }

    /// <summary>
    /// Increases the health of this agent
    /// </summary>
    /// <param name="amount"></param>
    public void increaseHealth(float amount) {
        if(health < maxHealth) {
            if(health + amount > maxHealth) {
                health = maxHealth;
            } else {
                health += amount;
            }
        }
    }

    /// <summary>
    /// Returns the current energy of this agent
    /// </summary>
    /// <returns></returns>
    public float getEnergy() {
        return energy;
    }

    /// <summary>
    /// Returns the maximum energy
    /// </summary>
    /// <returns></returns>
    public float getMaxEnergy() {
        return maxEnergy;
    }

    /// <summary>
    /// Increases the agents energy by the amount
    /// </summary>
    /// <param name="amount"></param>
    public void increaseEnergy(float amount) {
        if(energy < maxEnergy) {
            if(maxSpeed != trueMaxSpeed) {
                maxSpeed = trueMaxSpeed;
            }
            if(energy + amount > maxEnergy) {
                energy = maxEnergy;
            } else {
                energy += amount;
            }
        }
    }

    /// <summary>
    /// Returns the maximum health of this agent
    /// </summary>
    /// <returns></returns>
    public float getMaxHealth() {
        return maxHealth;
    }
    
    /// <summary>
    /// Returns the current speed of this agent
    /// </summary>
    /// <returns></returns>
    public float getCurrentSpeed() {
        return currentSpeed;
    }

    /// <summary>
    /// Returns the maximum speed this agent can go
    /// </summary>
    /// <returns></returns>
    public float getMaximumSpeed() {
        return maxSpeed;
    }

    /// <summary>
    /// Returns the near target list
    /// </summary>
    /// <returns></returns>
    public List<AlienAI> getNearTargets() {
        return nearTargets;
    }

    /// <summary>
    /// Returns the target of this agent
    /// </summary>
    /// <returns></returns>
    public AlienAI getTarget() {
        return target;
    }

    /// <summary>
    /// Sets the target of this agent
    /// </summary>
    /// <param name="target">The target to set</param>
    public void setTarget(AlienAI target) {
        this.target = target;
    }

    /// <summary>
    /// Returns the list of other creatures
    /// </summary>
    /// <returns></returns>
    public List<GameObject> getOtherCreatures() {
        return otherCreatures;
    }

    /// <summary>
    /// Returns the allignment weight for this agent
    /// </summary>
    /// <returns></returns>
    public float getAllignmentWeight() {
        return allignmentWeight;
    }

    /// <summary>
    /// Returns the distance to allign from
    /// </summary>
    /// <returns></returns>
    public float getAllignmentDistance() {
        return allignmentDistance;
    }

    /// <summary>
    /// Returns the cohesion weight for this agent
    /// </summary>
    /// <returns></returns>
    public float getCohesionWeight() {
        return cohesionWeight;
    }

    /// <summary>
    /// Returns the distance to coherse from
    /// </summary>
    /// <returns></returns>
    public float getCohesionDistance() {
        return cohesionDistance;
    }

    /// <summary>
    /// Returns the seperation weight for this agent
    /// </summary>
    /// <returns></returns>
    public float getSeperationWeight() {
        return seperationWeight;
    }

    /// <summary>
    /// Returns the distance to seperate from
    /// </summary>
    /// <returns></returns>
    public float getSeperationDistance() {
        return seperationDistance;
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
    /// Helper function to start the corutine to damage creatures
    /// </summary>
    /// <param name="creature">The creature to damage</param>
    public void damageCreature(AlienAI creature) {
        if(canAttack) {
            StartCoroutine(applyDamage(creature));
        }
    }

    /// <summary>
    /// Sets the target speed
    /// </summary>
    /// <param name="value">Target speed</param>
    public void setTargetSpeed(float value) {
        //Make sure the target speed is never an impossible value
        if(value > maxSpeed) {
            targetSpeed = maxSpeed;
        } else if(value < 0) {
            targetSpeed = 0;
        } else {
            targetSpeed = value;
        }
    }

    /// <summary>
    /// Increments the steering vector for the agent to use
    /// </summary>
    /// <param name="steeringVector">New steering force</param>
    public void addSteeringForce(Vector3 steeringVector) {
        steering += steeringVector;
    }

    /// <summary>
    /// Calculates the length of the the vector based off of speed
    /// </summary>
    /// <param name="vec">Vector to calculate from</param>
    /// <returns></returns>
    public Vector3 calculateSpeed(Vector3 vec) {
        return ((vec.normalized * currentSpeed) / speedScale) * Time.deltaTime;
    }

    /// <summary>
    /// Returns true if this agent is ready to reproduce
    /// </summary>
    /// <returns></returns>
    public bool canReproduce() {
        return reproductionTimePassed >= reproductionTimer;
    }

    /// <summary>
    /// Helper function for reoduction
    /// </summary>
    public void AttemptReproduction() {
        StartCoroutine(reproduce());
    }

    /// <summary>
    /// Changes the maximum speed to the amount
    /// </summary>
    /// <param name="amount">How much to change by</param>
    /// <param name="setTrueMax">Whether or not to change the true max speed</param>
    protected void changeMaxSpeed(float amount, bool setTrueMax = false) {
        maxSpeed = amount;
        if(setTrueMax) {
            trueMaxSpeed = amount;
        }
    }

    /// <summary>
    /// Returns a value on if the agent should attack the creature, < 0 and its not recommended > 0 and it is recommended
    /// </summary>
    /// <param name="creature">The target</param>
    /// <returns></returns>
    protected float shouldAttack(AlienAI creature) {
        //The stats of this creature comprised into one variable (weights applied)
        float stats = getStrength() + getSpeedModifier();
        //The stats of the other creature (weights applied)
        float theriStats = creature.getStrength() + creature.getSpeedModifier();
        //Off set by intelligence
        float chance = (stats - theriStats) + (getIntelligence() * 0.1f);

        //Return the likleyhood
        return chance;
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
            wingCount = maxWings;

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
            changeMaxSpeed(getMaximumSpeed() + speedModifier, true);

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
        spawn.GetComponent<AlienAI>().copyCreature(this);
        //Add it onto the list
        addCreature(spawn);
        //Make the other creatures aware of this new creature
        foreach(GameObject creature in otherCreatures) {
            if(creature != null) {
                creature.GetComponent<AlienAI>().addCreature(spawn);
            }
        }
        //Reset the timer
        reproductionTimePassed = 0;
    }

    /// <summary>
    /// Applies the damage to the target
    /// </summary>
    /// <param name="creature"></param>
    /// <returns></returns>
    private IEnumerator applyDamage(AlienAI creature) {
        canAttack = false;
        float damage = (5 + strengthModifier) / 3;
        creature.receiveDamage(damage);
        yield return new WaitForSeconds(1.5f); //random value for now
        canAttack = true;
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
        if(potentialTarget != null && potentialTarget.GetComponent<AlienAI>() != null) {
            AlienAI targetScript = potentialTarget.GetComponent<AlienAI>();
            if(!string.Equals(targetScript.getSpecies(), getSpecies())) {
                if(!nearTargets.Contains(targetScript)) {
                    nearTargets.Add(targetScript);
                }
            }
        }
    }

    /// <summary>
    /// Displays the debug lines for the velocity(Green), steering(Red) and desired velocity(Blue)
    /// </summary>
    private void displayDebugLines() {
        //Desired velocity
        Debug.DrawLine(transform.position, transform.position + (desiredVelocity.normalized * 5), Color.blue);
        //Steering
        Vector3 steer = steering + velocity;
        Debug.DrawLine(transform.position, transform.position + (steer.normalized * 5), Color.red);
        //Velocity
        Debug.DrawLine(transform.position, transform.position + (velocity.normalized * 5), Color.green);
    }
}

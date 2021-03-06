﻿//Script made by: 626224
using UnityEngine;
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
    public string creatureSpecies;      //The species of this creature 
    public float acceleration = 0.1f;   //How quickly the speed changes
    public float mass = 20;             //How heavy the agent is (this makes the steering more smooth)
    public float attackSpeed;           //Time between each attack (seconds)
    public float strengthModifier;      //How much damage the creature does
    public float maxSpeed;              //How fast the agent moves at any given time, see true max speed for an unchanging variable
    public float dodgeModifier;         //Likley hood of avoid damage
    public float reproductionTimer;     //How long between each reproduction attempt (seconds)
    public biomes favouriteBiome;       //Creature gets a +%35 in stats inside this biome
    public biomes leastFavouriteBiome;  //Creature gets a -%60 in stats inside this biome 
    public GameObject[] creatureBody;   //Reference so it can set the colour of its body
    //Distances on how far to compute the specific steering behaviours
    public float allignmentDistance = 8;
    public float cohesionDistance = 12;
    public float seperationDistance = 10;
    public float breakingDistance = 5;  //Distance to start breaking when pursuing targets

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

    //These are here to store the base values for all the modifies (which get changed per biome)
    private float baseAttackSpeed;
    private float baseStrength;
    private float baseDodge;
    private float baseReproduction;

    //Colour of this creature's body
    private Color bodyColour;
    private bool colourSet = false;

    //The speed the agent wants to go
    private float targetSpeed;
    //How fast the agent is current moving
    private float currentSpeed;
    //the fastest possible speed this agent can go      
    private float trueMaxSpeed;

    //If the agent can attack
    protected bool canAttack;

    //Character controller ref
    private CharacterController controller;

    //How much to scale everything down (for the planet size)
    private float planetScale;
    private bool scaleSet = false;  //for reproducing

    //The creature's individual name
    private string creatureName = "NO_NAME";

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
    //Variables to control how long to wait for weight changing
    private float weightChangeWait;
    private float weightChangePass;

    //The velocity vector
    private Vector3 velocity;
    //Used to apply steering
    private Vector3 steering;
    private Vector3 desiredVelocity;

    //The main selector of the behavior tree
    private Selector mainSelector;
    //Tasks for the main selector
    private EscapeSequence escape;
    private RecoupSequence recoup;
    private FightSelector fight;
    private ReproduceSequence reproduceSq;
    private IdleSelector idle;

    //Make sure the agents stay on the ground
    private float startY;

    //Stops agents being double affected by biomes on spawn
    private float biomeWait;
    private float biomeWaitPast;
    private bool set = false;

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

    // Use this for initialization
    public virtual void Start () {
        if(!colourSet) {
            //Get a random colour for this creature
            bodyColour = new Color(Random.value, Random.value, Random.value);
        }
        for(int i = 0; i < creatureBody.Length; i++) {
            creatureBody[i].GetComponent<Renderer>().material.color = bodyColour;
        }

        //Set the biome wait times
        if(!set) {
            biomeWait = 0;  //gets given a value on reproduce
            biomeWaitPast = 0;
        }

        //Construct the main selector
        mainSelector = new Selector(this);

        //Construct the tasks
        escape = new EscapeSequence(this);
        recoup = new RecoupSequence(this);
        fight = new FightSelector(this);
        reproduceSq = new ReproduceSequence(this);
        idle = new IdleSelector(this);

        //Add nodes in order of importance
        mainSelector.addChild(escape);
        mainSelector.addChild(recoup);
        mainSelector.addChild(fight);
        mainSelector.addChild(reproduceSq);
        mainSelector.addChild(idle);

        //Set the stat values
        health = 100;
        maxHealth = health;
        energy = 100;
        maxEnergy = energy;
        energyThreshold = 5;
        trueMaxSpeed = maxSpeed;
        currentSpeed = 0;
        targetSpeed = currentSpeed;
        //Keep track of the base values
        if(baseAttackSpeed == 0) {
            baseAttackSpeed = attackSpeed;
        }
        if(baseDodge == 0) {
            baseDodge = dodgeModifier;
        }
        if(baseStrength == 0) {
            baseStrength = strengthModifier;
        }
        if(baseReproduction == 0) {
            baseReproduction = reproductionTimer;
        }

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

        //Init timer
        weightChangeWait = Random.Range(3, 10);
        weightChangePass = 0;

        //Get the controllre
        controller = GetComponent<CharacterController>();

        //Set the default value
        if(!scaleSet) {
            planetScale = 1;
        }

        //Init
        reproductionTarget = null;
        reproductionTimePassed = 0;

        //Set the name
        creatureName = "";
        int nameLength = Random.Range(2, 4);
        for(int i = 0; i < nameLength; i++) {
            creatureName += nameParts[Random.Range(0, nameParts.Length)];
        }
        creatureName = upperCaseFirst(creatureName);
        //Set the object name
        name = creatureSpecies + ": " + creatureName;

        //Get the collider reference and make sure it is a trigger
        targetDetectCollider = GetComponent<SphereCollider>();
        targetDetectCollider.isTrigger = true;

        startY = transform.position.y;
    }
	
	// Update is called once per frame
	public virtual void Update () {
        if(biomeWaitPast < biomeWait) {
            biomeWaitPast += Time.deltaTime;
        }

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
            //Avoid obstacles independantly of the behavior tree
            addSteeringForce(collisionAvoidance());

            //Make the steering smooth
            steering /= mass;

            //Add to the velocity
            if(!float.IsNaN(steering.x) && !float.IsNaN(steering.y) && !float.IsNaN(steering.z)) {
                velocity += steering;
            } else {
                steering = Vector3.zero;
            }

            //Normalise the desired velocity and add the speed
            velocity = calculateSpeed(velocity);
            //Make sure they do not go in the air
            velocity.y = 0;

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
    /// Called when the creature enters a biome, adjusts stats accordingly
    /// </summary>
    public void enteredBiome(biomes biome) {
        if(biomeWaitPast < biomeWait) {
            return;
        }
        if(biome == favouriteBiome) {
            //Increase stats by 35%
            strengthModifier += baseStrength * 0.35f;
            dodgeModifier += baseDodge * 0.35f;
            attackSpeed -= baseAttackSpeed * 0.35f;
            reproductionTimer -= baseReproduction * 0.35f;
        } else if(biome == leastFavouriteBiome) {
            //Decrease stats by 60%
            strengthModifier -= baseStrength * 0.6f;
            dodgeModifier -= baseDodge * 0.6f;
            attackSpeed += baseAttackSpeed * 0.6f;
            reproductionTimer += baseReproduction * 0.6f;
        } else {
            strengthModifier = baseStrength;
            dodgeModifier = baseDodge;
            attackSpeed = baseAttackSpeed;
            reproductionTimer = baseReproduction;
        }
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
    /// Makes the creature wait before detecting biomes
    /// </summary>
    public void initBiomeWait() {
        biomeWait = 1;
        biomeWaitPast = 0;
        set = true;
    }

    /// <summary>
    /// Returns the creatue's species
    /// </summary>
    /// <returns></returns>
    public string getSpecies() {
        return creatureSpecies;
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
    /// Sets the list of known other creatures
    /// </summary>
    /// <param name="list">The list to replace with</param>
    public void giveCreatureList(List<GameObject> list) {
        //Replace the list
        otherCreatures = new List<GameObject>(list);
        //Make sure the list does not contain itself
        otherCreatures.Remove(gameObject);
    }

    /// <summary>
    /// Call to set the scale for speed (gets dvidided by amount)
    /// </summary>
    /// <param name="scale">Scale for speed, can't be beloew 1</param>
    public void setPlanetScale(float scale) {
        if(scale < 1) {
            return;
        } else {
            planetScale = scale;
            scaleSet = true;
        }
    }

    /// <summary>
    /// Returns the planet scale value
    /// </summary>
    /// <returns></returns>
    public float getPlanetScale() {
        return planetScale;
    }

    /// <summary>
    /// Returns the breaking distance of the agent
    /// </summary>
    /// <returns></returns>
    public float getBreakingDistance() {
        return breakingDistance / planetScale;
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
        return currentSpeed / planetScale;
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
        return allignmentDistance / planetScale;
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
        return cohesionDistance / planetScale;
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
        return seperationDistance / planetScale;
    }

    /// <summary>
    /// Changes the colour of this agent
    /// </summary>
    /// <param name="colour"></param>
    /// <returns></returns>
    public void changeBodyColour(Color colour) {
        bodyColour = colour;
        colourSet = true;
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
        return ((vec.normalized * currentSpeed) / planetScale) * Time.deltaTime;
    }

    /// <summary>
    /// Returns true if this agent is ready to reproduce
    /// </summary>
    /// <returns></returns>
    public bool canReproduce() {
        return reproductionTimePassed >= reproductionTimer && otherCreatures.Count < 15;
    }

    /// <summary>
    /// Attempts to make another copy of this object
    /// </summary>
    public void reproduce() {
        //Create a copy of this gameObject
        GameObject spawn = GameObject.Instantiate(gameObject);
        //Get the script reference
        AlienAI spawnScript = spawn.GetComponent<AlienAI>();

        //Init the biome wait
        spawnScript.initBiomeWait();

        //Immediatley set the scale
        spawnScript.setPlanetScale(planetScale);
        //Set the parent if there is one
        if(transform.parent != null) {
            spawn.transform.SetParent(transform.parent);
        }
        //Make the spawn apear in a random position near the creature
        spawn.transform.position = transform.position + new Vector3(Random.Range(-10, 10) / planetScale, 0, Random.Range(-10, 10) / planetScale);
        //Make sure the scale is normal
        spawn.transform.localScale = transform.localScale;

        //Set the body colour
        spawnScript.changeBodyColour(bodyColour);

        //Give the creature the list of current other creatures
        spawnScript.giveCreatureList(otherCreatures);
        //Let the new creature be aware of this creature
        spawnScript.addCreature(gameObject);
        //Make the other creatures aware of this new creature
        foreach(GameObject creature in otherCreatures) {
            if(creature != null) {
                creature.GetComponent<AlienAI>().addCreature(spawn);
            }
        }
        //Add the new creature onto this own creature's list
        addCreature(spawn);

        //Reset the reproduction timer
        reproductionTimePassed = 0;
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
    /// Returns a force to avoid obstacles with
    /// </summary>
    /// <param name="distance">Distance ahead to avoid obstacles</param>
    /// <param name="strength">How strong the force will be</param>
    /// <returns></returns>
    private Vector3 collisionAvoidance(float distance = 10, float strength = 5) {
        //Scale back the distance
        distance /= planetScale;
        //Combined force of the 3 rays
        Vector3 force = Vector3.zero;
        //Fire a ray straight infront of the target
        force += fireAvoidanceRay(transform.position, velocity, distance);

        //Calculate the force
        force = force.normalized * strength * Time.deltaTime;
        //Set the desired velocity
        setDesiredVelocity(force);

        //Return the new force to push the agent away from obsticles
        return desiredVelocity - velocity;
    }

    /// <summary>
    /// For the collision avoidance function, fires a ray in the direction and returns an un noramlized vector
    /// </summary>
    /// <param name="from">Where to fire the ray from</param>
    /// <param name="direction">Direction to fire the ray</param>
    /// <param name="distance">How far to fire the ray</param>
    /// <returns></returns>
    private Vector3 fireAvoidanceRay(Vector3 from, Vector3 direction, float distance) {
        //Fire a ray at the distance to look ahead
        RaycastHit rayOut;
        if(Physics.Raycast(transform.position, velocity, out rayOut, distance)) {
            //Store the game object
            GameObject hit = rayOut.collider.gameObject;
            //First make sure we arent avoiding targets or allies
            if((target == null || hit != target) && (reproductionTarget == null || hit != reproductionTarget) && !otherCreatures.Contains(hit) && hit.tag != "Biome") {
                //Calculate the avoidance force
                Vector3 ahead = transform.position + velocity * distance * Time.deltaTime;
                Vector3 avoidanceForce = ahead - rayOut.point;
                //Return the new force
                return avoidanceForce;
            }
        }
        return Vector3.zero;
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
        yield return new WaitForSeconds(attackSpeed);
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
        //Avoidance distance (single ray)
        Debug.DrawLine(transform.position, transform.position + (velocity.normalized * 10), Color.magenta);
        //Velocity
        Debug.DrawLine(transform.position, transform.position + (velocity.normalized * 5), Color.green);
    }
}

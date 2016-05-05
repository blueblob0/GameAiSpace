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
    public bool canFly = false;         //DOESN'T DO ANYTHING -------------------------------------------------------------------------------------------------
    public GameObject[] creatureBody;   //Reference so it can set the colour of its body

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

    //The main selector of the behavior tree
    private Selector mainSelector;
    //Tasks for the main selector
    private EscapeSequence escape;
    private RecoupSequence recoup;
    private FightSelector fight;
    private ReproduceSequence reproduceSq;
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

    // Use this for initialization
    public virtual void Start () {
        //Get a random colour for this creature
        Color bodyColour = new Color(Random.value, Random.value, Random.value);
        for(int i = 0; i < creatureBody.Length; i++) {
            creatureBody[i].GetComponent<Renderer>().material.color = bodyColour;
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
        baseAttackSpeed = attackSpeed;
        baseDodge = dodgeModifier;
        baseStrength = strengthModifier;
        baseReproduction = reproductionTimer;

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

        //Get the controllre
        controller = GetComponent<CharacterController>();

        //Set the default value
        planetScale = 1;

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
    /// Called when the creature enters a biome, adjusts stats accordingly
    /// </summary>
    public void enteredBiome(biomes biome) {
        if (biome == favouriteBiome){
            //Increase stats by 35%
            strengthModifier += baseStrength * 0.35f;
            dodgeModifier += baseDodge * 0.35f;
            attackSpeed += baseAttackSpeed * 0.35f;
            reproductionTimer += baseReproduction * 0.35f;
        }

        if (biome == leastFavouriteBiome) {
            //Decrease stats by 60%
            strengthModifier -= baseStrength * 0.6f;
            dodgeModifier -= baseDodge * 0.6f;
            attackSpeed -= baseAttackSpeed * 0.6f;
            reproductionTimer -= baseReproduction * 0.6f;
        }
    }

    /// <summary>
    /// Called when the creature leaves the biome, sets its stat values back to normal
    /// </summary>
    public void leftBiome(){
        strengthModifier = baseStrength;
        dodgeModifier = baseDodge;
        attackSpeed = baseAttackSpeed;
        reproductionTimer = baseReproduction;
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
        return ((vec.normalized * currentSpeed) / planetScale) * Time.deltaTime;
    }

    /// <summary>
    /// Returns true if this agent is ready to reproduce
    /// </summary>
    /// <returns></returns>
    public bool canReproduce() {
        return reproductionTimePassed >= reproductionTimer;
    }

    /// <summary>
    /// Attempts to make another copy of this object
    /// </summary>
    public void reproduce() {
        //Create a copy of this gameObject
        GameObject spawn = GameObject.Instantiate(gameObject);
        //Make the spawn apear in a random position near the creature
        spawn.transform.position += new Vector3(Random.Range(-10, 10) / planetScale, 0, Random.Range(-10, 10) / planetScale);
        //Set the parent if there is one
        if(transform.parent != null) {
            spawn.transform.SetParent(transform.parent);
        }

        //Give the creature the list of current other creatures
        spawn.GetComponent<AlienAI>().giveCreatureList(otherCreatures);
        //Let the new creature be aware of this creature
        spawn.GetComponent<AlienAI>().addCreature(gameObject);
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
    private Vector3 collisionAvoidance(float distance = 10, float strength = 3) {
        //Fire a ray at the distance to look ahead
        RaycastHit rayOut;
        if(Physics.Raycast(transform.position, velocity, out rayOut, distance)) {
            //Store the game object
            GameObject hit = rayOut.collider.gameObject;
            //First make sure we arent avoiding targets or allies
            if((target == null || hit != target) && (reproductionTarget == null || hit != reproductionTarget) && !otherCreatures.Contains(hit)) {
                Debug.Log("Applying force away from " + hit.name);

                //Calculate the avoidance force
                Vector3 ahead = transform.position + velocity * distance * Time.deltaTime;
                Vector3 avoidanceForce = ahead - (rayOut.point + hit.transform.position);
                //Set the desired velocity
                setDesiredVelocity(avoidanceForce.normalized * strength * Time.deltaTime);
                //Return the new force to push the agent away from obsticles
                return desiredVelocity - velocity;
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
        //Avoidance distance
        Debug.DrawLine(transform.position, transform.position + (velocity.normalized * 10), Color.magenta);
        //Velocity
        Debug.DrawLine(transform.position, transform.position + (velocity.normalized * 5), Color.green);
    }
}

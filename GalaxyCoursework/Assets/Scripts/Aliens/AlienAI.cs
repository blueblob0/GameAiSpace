using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * This class basically holds all the steering algs for the creatures
 *
 * THIS WILL BE CHANGED TO INCOPERATE THE BEHAVIOR TREE
 */

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class AlienAI : MonoBehaviour {

    //Set in inspector
    public float acceleration = 0.1f;  //How quickly the speed changes
    public float currentSpeed = 5;     //How fast the agent is current moving
    public float maxSpeed = 10;        //The fastest this agent can go
    public float mass = 20;            //How heavy the agent is (this makes the steering more smooth)

    //List of potential targets
    protected List<AlienCreature> nearTargets = new List<AlienCreature>();
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
    private FightSelector fight;
    private IdleSelector idle;

    // Use this for initialization
    public virtual void Start () {
        //Construct the main selector
        mainSelector = new Selector(this);

        //Construct the tasks
        escape = new EscapeSequence(this);
        fight = new FightSelector(this);
        idle = new IdleSelector(this);

        //Add nodes in order of importance
        mainSelector.addChild(escape);
        mainSelector.addChild(fight);
        mainSelector.addChild(idle);

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
    }
	
	// Update is called once per frame
	public virtual void Update () {
        //DEBUG---------------------
        displayDebugLines();
        //--------------------------

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

        //Make the steering smooth
        steering /= mass;

        //Add to the velocity
        velocity += steering;

        //Normalise the desired velocity and add the speed
        velocity = calculateSpeed(velocity);

        //Update the position and look 'forward'
        if(currentSpeed > 0) {
            transform.rotation = Quaternion.LookRotation(velocity);
            transform.position += velocity;
        }

        //Adjust the speed values
        if(currentSpeed < targetSpeed) {
            currentSpeed += acceleration;
        } else if(currentSpeed > targetSpeed) {
            currentSpeed -= acceleration;
        }
        //0 off the speed
        if(targetSpeed <= 0 && currentSpeed <= 0.1f) {
            currentSpeed = 0;
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
    public List<AlienCreature> getNearTargets() {
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
    /// Changes the maximum speed to the amount
    /// </summary>
    /// <param name="amount"></param>
    protected void changeMaxSpeed(float amount) {
        maxSpeed = amount;
    }

    /// <summary>
    /// Returns the maximum speed
    /// </summary>
    /// <returns></returns>
    protected float getMaxSpeed() {
        return maxSpeed;
    }

    /// <summary>
    /// Increments the steering vector for the agent to use
    /// </summary>
    /// <param name="steeringVector">New steering force</param>
    public void addSteeringForce(Vector3 steeringVector) {
        steering += steeringVector;
    }

    /// <summary>
    /// Makes the agent stop
    /// </summary>
    protected void stopMovement() {
        desiredVelocity = Vector3.zero;
        steering = Vector3.zero;
        targetSpeed = 0;
    }

    /// <summary>
    /// Calculates the length of the the vector based off of speed
    /// </summary>
    /// <param name="vec">Vector to calculate from</param>
    /// <returns></returns>
    public Vector3 calculateSpeed(Vector3 vec) {
        return vec.normalized * currentSpeed * Time.deltaTime;
    }

    /// <summary>
    /// Displays the debug lines for the velocity(Green), steering(Red) and desired velocity(Blue)
    /// </summary>
    private void displayDebugLines() {
        //Desired velocity
        Debug.DrawLine(transform.position, transform.position + (desiredVelocity.normalized * 5), Color.blue);
        //Steering
        Debug.DrawLine(transform.position, transform.position + (steering.normalized * 5), Color.red);
        //Velocity
        Debug.DrawLine(transform.position, transform.position + (velocity.normalized * 5), Color.green);
    }
}

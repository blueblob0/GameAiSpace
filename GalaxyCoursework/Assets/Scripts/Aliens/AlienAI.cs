using UnityEngine;
using System.Collections;

/*
 * This class basically holds all the steering algs for the creatures
 */

public class AlienAI : MonoBehaviour {

    //Set in inspector
    public float acceleration = 0.1f;   //How quickly the speed changes
    public float currentSpeed = 5;     //How far the agent is current moving
    public float maxSpeed = 10;        //The fastest this agent can go
    public float mass = 20;            //How heavy the agent is (this makes the steering more smooth)

    private float targetSpeed;         //The speed the agent wants to go

    //How much each steering behaviour will affect the total flocking steering
    private float allignmentWeight;
    private float cohesiontWeight;
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

    private float wanderAngle;

    // Use this for initialization
    public virtual void Start () {
        //Init the velocity vectors
        velocity = transform.forward * currentSpeed * Time.deltaTime;
        steering = Vector3.zero;
        desiredVelocity = Vector3.zero;

        //Get a random wander angle
        wanderAngle = Random.Range(0, 360);

        //Init weights
        allignmentWeight = 1;
        cohesiontWeight  = 1;
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
            cohesiontWeight = Random.value;
            seperationWeight = Random.value;
        }
        weightChangePass += Time.deltaTime;

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
    public Vector3 getvelocity() {
        return velocity;
    }

    /// <summary>
    /// Sets the target speed
    /// </summary>
    /// <param name="value">Target speed</param>
    private void setTargetSpeed(float value) {
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
    /// Makes the agent seek the target
    /// </summary>
    /// <param name="targetWorldPos">The world position of the target to seek</param>
    /// <returns>Returns the steering force of the desired direction</returns>
    protected Vector3 seek(Vector3 targetWorldPos) {
        //Set the diesried velocity to the direction of the target
        desiredVelocity = calculateSpeed(targetWorldPos - transform.position);
        //Return the steering force of the desired velocity
        return desiredVelocity - velocity;
    }

    /// <summary>
    /// Returns the steering force for persuing the target
    /// </summary>
    /// <param name="target">The agent to seek</param>
    /// <returns></returns>
    protected Vector3 persue(AlienAI target) {
        //How far ahead (as time) to persue the target
        float t = Vector3.Distance(target.transform.position, transform.position) / (currentSpeed * Time.deltaTime);
        //Get the future position of the agent
        Vector3 futurePosition = target.transform.position + (target.getvelocity() * t);
        //Return the seek steering of the future position
        return seek(futurePosition);
    }

    /// <summary>
    /// Makes the agent flee from the target
    /// </summary>
    /// <param name="targetWorldPos">World position of the target to flee from</param>
    /// <returns>Returns the steering force of the desired direction</returns>
    protected Vector3 flee(Vector3 targetWorldPos) {
        //Set the diesried velocity to the away from the target
        desiredVelocity = calculateSpeed(transform.position - targetWorldPos);
        //Return the steering force of the desired velocity
        return desiredVelocity - velocity;
    }

    /// <summary>
    /// Returns the steering force to eveade the target
    /// </summary>
    /// <param name="target">Target to evade</param>
    /// <returns></returns>
    protected Vector3 evade(AlienAI target) {
        //How far ahead (as time) to to evade the target
        float t = Vector3.Distance(target.transform.position, transform.position) / (currentSpeed * Time.deltaTime);
        //Get the future position of the agent
        Vector3 futurePosition = target.transform.position + (target.getvelocity() * t);
        //Return the flee steering of the future position
        return flee(futurePosition);
    }

    /// <summary>
    /// Makes the agent wander around
    /// </summary>
    /// <param name="circleDistance">How far forward to set the wander pos</param>
    /// <param name="circleRadius">How big the displacement vector can be</param>
    /// <returns>The wander (steering) force</returns>
    protected Vector3 wander(float circleDistance = 6.0f, float circleRadius = 5.0f) {
        //Create the 'circle' for a wander position to be in
        Vector3 circleCenter = velocity.normalized;
        circleCenter *= circleDistance;

        //Init the displacement force (direction to wander to)
        Vector3 displacement = new Vector3(0, 0, 1);
        //Displace the vector by the wanderAngle
        displacement.x = circleRadius * Mathf.Cos(wanderAngle);
        displacement.z = circleRadius * Mathf.Sin(wanderAngle);
        //Move the angle slightly in a random direction
        wanderAngle += Random.Range(-1, 1);

        //Normalize the new steering force to the speed
        desiredVelocity = calculateSpeed(circleCenter + displacement);

        //Return the new force
        return desiredVelocity - velocity;
    }

    /// <summary>
    /// Returns a velocity vector for the creature's allignment to other creatures
    /// </summary>
    /// <param name="otherCreatures">Array of other creatures</param>
    /// <returns></returns>
    protected Vector3 computeAllignment(GameObject[] otherCreatures) {
        //Init the return value
        Vector3 velocity = Vector3.zero;
        //Keep track of the neighbours
        int neighbourCount = 0;

        //Loop through the other creatures
        for(int i = 0; i < otherCreatures.Length; i++) {
            //Make sure it isn't computing with itself
            if(otherCreatures[i] != gameObject) {
                //Make sure it is within the allignment range
                if(Vector3.Distance(transform.position, otherCreatures[i].transform.position) <= allignmentDistance) {
                    //Add on the agent's velocity 
                    velocity += otherCreatures[i].GetComponent<AlienAI>().getvelocity();
                    //Increase neighbour count
                    neighbourCount++;
                }
            }
        }

        //If there were not any neighbours then return zero
        if(neighbourCount == 0) {
            return Vector3.zero;
        }

        //Get the velocity based off of the center of mass
        velocity /= neighbourCount;

        //Normalize and return
        velocity.Normalize();
        return velocity;
    }

    /// <summary>
    /// Returns a velocity vector for the creature's cohesion to other creatures
    /// </summary>
    /// <param name="otherCreatures">Array of other creatures</param>
    /// <returns></returns>
    protected Vector3 computeCohesion(GameObject[] otherCreatures) {
        //Init the return value
        Vector3 velocity = Vector3.zero;
        //Keep track of the neighbours
        int neighbourCount = 0;

        //Loop through the other creatures
        for(int i = 0; i < otherCreatures.Length; i++) {
            //Make sure it isn't computing with itself
            if(otherCreatures[i] != gameObject) {
                //Make sure it is within the cohesion range
                if(Vector3.Distance(transform.position, otherCreatures[i].transform.position) > cohesionDistance) {
                    //Add on the agent's position
                    velocity += otherCreatures[i].transform.position;
                    //Increase neighbour count
                    neighbourCount++;
                }
            }
        }

        //If there were not any neighbours then return zero
        if(neighbourCount == 0) {
            return Vector3.zero;
        }

        //Get the position of the center of mass
        velocity /= neighbourCount;
        velocity = velocity - transform.position;

        //Normalize and return
        velocity.Normalize();
        return velocity;
    }

    /// <summary>
    /// Returns a velocity vector for the creature's seperation to other creatures
    /// </summary>
    /// <param name="otherCreatures">Array of other creatures</param>
    /// <returns></returns>
    protected Vector3 computeSeperation(GameObject[] otherCreatures) {
        //Init the return value
        Vector3 velocity = Vector3.zero;
        //Keep track of the neighbours
        int neighbourCount = 0;

        //Loop through the other creatures
        for(int i = 0; i < otherCreatures.Length; i++) {
            //Make sure it isn't computing with itself
            if(otherCreatures[i] != gameObject) {
                //Make sure it is within the seperation range
                if(Vector3.Distance(transform.position, otherCreatures[i].transform.position) <= seperationDistance) {
                    //Add on the distacne from the agent
                    velocity += otherCreatures[i].transform.position - transform.position;
                    //Increase neighbour count
                    neighbourCount++;
                }
            }
        }

        //If there were not any neighbours then return zero
        if(neighbourCount == 0) {
            return Vector3.zero;
        }

        //Get the velocity based off of the center of mass
        velocity /= neighbourCount;

        //Negate the vector to make sure the agent steers away
        velocity *= -1;

        //Normalize and return
        velocity.Normalize();
        return velocity;
    }

    /// <summary>
    /// Computes the flocking behaviours bassed off of the three steering algs and returns the steering force
    /// </summary>
    /// <param name="otherCreatures">Array of other creatures</param>
    /// <returns></returns>
    protected Vector3 computeFlocking(GameObject[] otherCreatures) {
        //Get the three steering behaviours
        Vector3 allignment  = computeAllignment(otherCreatures);
        Vector3 cohesion    = computeCohesion(otherCreatures);
        Vector3 seperation  = computeSeperation(otherCreatures);

        //Set the desired velocity
        desiredVelocity = calculateSpeed(allignment * allignmentWeight + cohesion * cohesiontWeight + seperation * seperationWeight);

        //Return the steering force
        return desiredVelocity - velocity;
    }

    /// <summary>
    /// Increments the steering vector for the agent to use
    /// </summary>
    /// <param name="steeringVector">New steering force</param>
    protected void addSteeringForce(Vector3 steeringVector) {
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
    private Vector3 calculateSpeed(Vector3 vec) {
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

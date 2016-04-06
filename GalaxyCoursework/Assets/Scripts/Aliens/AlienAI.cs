using UnityEngine;
using System.Collections;

/*
 * This class basically holds all the steering algs for the creatures
 */

public class AlienAI : MonoBehaviour {

    //Set in inspector
    public ushort speed = 5;
    public ushort mass = 15;
    public ushort targetDetectRadius = 20;

    //The velocity vector
    private Vector3 velocity;
    //Used to apply steering
    private Vector3 steering;
    private Vector3 desiredVelocity;

    private float wanderAngle;

    // Use this for initialization
    public virtual void Start () {
        //Init the velocity vectors
        velocity = transform.forward * speed * Time.deltaTime;
        steering = Vector3.zero;
        desiredVelocity = Vector3.zero;

        //Get a random wander angle
        wanderAngle = Random.Range(0, 360);
    }
	
	// Update is called once per frame
	public virtual void Update () {
        //DEBUG---------------------
        displayDebugLines();
        //--------------------------

        //Make the steering smooth
        steering /= mass;

        //Add to the velocity
        velocity += steering;

        //Normalise the desired velocity and add the speed
        velocity = calculateSpeed(velocity);

        //Update the position and look 'forward'
        transform.rotation = Quaternion.LookRotation(velocity);
        transform.position += velocity;
    }

    /// <summary>
    /// Returns the velocity vector of the current agent
    /// </summary>
    /// <returns></returns>
    public Vector3 getvelocity() {
        return velocity;
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
        float t = Vector3.Distance(target.transform.position, transform.position) / (speed * Time.deltaTime);
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
        float t = Vector3.Distance(target.transform.position, transform.position) / (speed * Time.deltaTime);
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
    /// Sets the steering vector for the agent to use
    /// </summary>
    /// <param name="steeringVector">Steering Vector</param>
    protected void setSteering(Vector3 steeringVector) {
        steering = steeringVector;
    }

    /// <summary>
    /// Makes the agent stop
    /// </summary>
    protected void stopMovement() {
        velocity = Vector3.zero;
        desiredVelocity = Vector3.zero;
        steering = Vector3.zero;
    }

    /// <summary>
    /// Calculates the length of the the vector based off of speed
    /// </summary>
    /// <param name="vec">Vector to calculate from</param>
    /// <returns></returns>
    private Vector3 calculateSpeed(Vector3 vec) {
        return vec.normalized * speed * Time.deltaTime;
    }

    /// <summary>
    /// Displays the debug lines for the velocity(Green), steering(Red) and desired velocity(Blue)
    /// </summary>
    private void displayDebugLines() {
        //Desired velocity
        Debug.DrawLine(transform.position, transform.position + (desiredVelocity.normalized * 5), Color.blue);
        //Steering
        Debug.DrawLine(transform.position + (velocity.normalized * 5), transform.position + (velocity.normalized * 5) + (steering.normalized * 2), Color.red);
        //Velocity
        Debug.DrawLine(transform.position, transform.position + (velocity.normalized * 5), Color.green);
    }
}

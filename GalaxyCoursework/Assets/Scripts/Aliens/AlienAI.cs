﻿using UnityEngine;
using System.Collections;

/*
 * The class that will control the AI of the creatures
 */

[RequireComponent(typeof(SphereCollider))]
public class AlienAI : MonoBehaviour {

    //Set in inspector
    public ushort speed = 5;
    public ushort mass = 15;
    public ushort targetDetectRadius = 10;

    //The target of the agent
    private GameObject target;

    //The velocity vector
    private Vector3 velocity;
    //Used to apply steering
    private Vector3 steering;
    private Vector3 desiredVelocity;

    //Used to detect targets
    SphereCollider targetCollider;

    // Use this for initialization
    public virtual void Start () {
        //Init the velocity vectors
        velocity = transform.forward * speed * Time.deltaTime;
        steering = Vector3.zero;
        desiredVelocity = Vector3.zero;

        //Set up the collider
        targetCollider = GetComponent<SphereCollider>();
        targetCollider.isTrigger = true;
        targetCollider.radius = targetDetectRadius;
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
    /// Makes the agent wander around
    /// </summary>
    /// <param name="wanderOffSet">How far forward to set the wander pos</param>
    /// <param name="circleRadius">How big the displacement vector can be</param>
    /// <returns>The wander (steering) force</returns>
    protected Vector3 wander(float wanderOffSet = 6.0f, float circleRadius = 5.0f) {
        //Create the 'circle' for a wander position to be in
        Vector3 circleCenter = calculateSpeed(velocity);

        //Init the displacement force (direction to wander to)
        Vector3 displacement = new Vector3(0, 0, 1);
        //Get an angle anywhere from 0 - 360
        float angle = Random.Range(0, 360);
        //Displace the vector by the angle
        displacement.x = circleRadius * Mathf.Cos(angle);
        displacement.z = circleRadius * Mathf.Sin(angle);

        //Normalize the new steering force
        Vector3 wanderForce = calculateSpeed(circleCenter + displacement);

        //Return the new force
        return wanderForce;
    }

    /// <summary>
    /// Sets the steering vector for the agent to use
    /// </summary>
    /// <param name="steeringVector">Steering Vector</param>
    protected void setSteering(Vector3 steeringVector) {
        steering = steeringVector;
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
        Debug.DrawLine(transform.position, transform.position + (steering.normalized * 5), Color.red);
        //Velocity
        Debug.DrawLine(transform.position, transform.position + (velocity.normalized * 5), Color.green);
    }
}

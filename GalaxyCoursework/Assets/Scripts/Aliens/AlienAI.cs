﻿using UnityEngine;
using System.Collections;

/*
 * The class that will control the AI of the creatures
 */

public class AlienAI : MonoBehaviour {

    //Set in inspector
    public ushort speed = 1;

    //The velocity vector
    private Vector3 velocity;

    // Use this for initialization
    public virtual void Start () {
        velocity = Vector3.zero;
	}
	
	// Update is called once per frame
	public virtual void Update () {
        //Get the desired velocity and calculate the steering
        velocity = wander(transform.position, transform.forward, 2);

        //Update the agent's position and rotation based on the velocity
        velocity.Normalize();
        velocity *= speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(velocity);
        transform.position += velocity;
    }

    /// <summary>
    /// Makes the agent seek the target
    /// </summary>
    /// <param name="currentPos">Current position of the agent</param>
    /// <param name="targetWorldPos">The world position of the target to seek</param>
    /// <returns></returns>
    protected Vector3 seek(Vector3 currentPos, Vector3 targetWorldPos) {
        return targetWorldPos - currentPos;
    }

    /// <summary>
    /// Makes the agent flee from the target
    /// </summary>
    /// <param name="currentPos">Current position of the agent</param>
    /// <param name="targetWorldPos">World position of the target to flee from</param>
    /// <returns></returns>
    protected Vector3 flee(Vector3 currentPos, Vector3 targetWorldPos) {
        return currentPos - targetWorldPos;
    }

    /// <summary>
    /// Makes the agent wander around
    /// </summary>
    /// <param name="curentPos">Current position of the agent</param>
    /// <param name="agentForward">Agent's forward vector</param>
    /// <param name="wanderOffSet">How far forward to set the wander pos</param>
    /// <returns></returns>
    protected Vector3 wander(Vector3 curentPos, Vector3 agentForward, float wanderOffSet = 5.0f) {
        //Create the 'circle' for the velcoity to be in
        Vector3 circleCenter = curentPos + (agentForward * wanderOffSet);
        //The radius of the circle will be how far the displacement can go
        float circleRadius = wanderOffSet / 2;

        //Init the displacement force (direction to wander to)
        Vector3 displacement = new Vector3();
        //Get an angle anywhere from 0 - 360
        float angle = Random.Range(0, 360);
        //Displace the vector by the angle
        displacement.x = circleRadius * Mathf.Cos(angle);
        displacement.z = circleRadius * Mathf.Sin(angle);

        //Return the new wander force
        return circleCenter + displacement;
    }
}

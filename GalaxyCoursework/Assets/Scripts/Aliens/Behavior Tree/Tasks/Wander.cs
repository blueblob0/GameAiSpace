﻿using UnityEngine;
using System.Collections;

/*
 * The task to make agents wander
 */
public class Wander : Task {
    //Reference to the agent using this behavior
    protected AlienAI agentRef;
    //constructor
    public Wander(AlienAI agent) {
        agentRef = agent;

        //Get a random wander angle
        wanderAngle = Random.Range(0, 360);
    }

    //The wander angle used
    private float wanderAngle;

    public bool activate() {
        agentRef.addSteeringForce(wander());
        return true;
    }

    /// <summary>
    /// Makes the agent wander around
    /// </summary>
    /// <param name="circleDistance">How far forward to set the wander pos</param>
    /// <param name="circleRadius">How big the displacement vector can be</param>
    /// <returns>The wander (steering) force</returns>
    private Vector3 wander(float circleDistance = 6.0f, float circleRadius = 5.0f) {
        //Create the 'circle' for a wander position to be in
        Vector3 circleCenter = agentRef.getVelocity().normalized;
        circleCenter *= circleDistance;

        //Init the displacement force (direction to wander to)
        Vector3 displacement = new Vector3(0, 0, 1);
        //Displace the vector by the wanderAngle
        displacement.x = circleRadius * Mathf.Cos(wanderAngle);
        displacement.z = circleRadius * Mathf.Sin(wanderAngle);
        //Move the angle slightly in a random direction
        wanderAngle += Random.Range(-1, 1);

        //Normalize the new steering force to the speed
        agentRef.setDesiredVelocity(agentRef.calculateSpeed(circleCenter + displacement));

        //Return the new force
        return agentRef.getDesiredVelocity() - agentRef.getVelocity();
    }
}

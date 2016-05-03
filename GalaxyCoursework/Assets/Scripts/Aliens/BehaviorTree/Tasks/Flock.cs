using UnityEngine;
using System.Collections;
using System;

/*
 * The task that computes the flocking steering behavior
 */
public class Flock : Task {
    //Reference to the agent using this behavior
    protected AlienAI agentRef;
    //constructor
    public Flock(AlienAI agent) {
        agentRef = agent;
    }

    public bool activate() {
        //Set the speed
        agentRef.setTargetSpeed(5);
        //Flock
        agentRef.addSteeringForce(computeFlocking(agentRef.getOtherCreatures().ToArray()));
        return true;
    }

    /// <summary>
    /// Returns a velocity vector for the creature's allignment to other creatures
    /// </summary>
    /// <param name="otherCreatures">Array of other creatures</param>
    /// <returns></returns>
    private Vector3 computeAllignment(GameObject[] otherCreatures) {
        //Init the return value
        Vector3 velocity = Vector3.zero;
        //Keep track of the neighbours
        int neighbourCount = 0;

        //Loop through the other creatures
        for(int i = 0; i < otherCreatures.Length; i++) {
            //Make sure it isn't computing with itself
            if(otherCreatures[i] != null && otherCreatures[i] != agentRef.gameObject) {
                //Make sure it is within the allignment range
                if(Vector3.Distance(agentRef.transform.position, otherCreatures[i].transform.position) <= agentRef.getAllignmentDistance()) {
                    //Add on the agent's velocity 
                    velocity += otherCreatures[i].GetComponent<AlienAI>().getVelocity();
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
    private Vector3 computeCohesion(GameObject[] otherCreatures) {
        //Init the return value
        Vector3 velocity = Vector3.zero;
        //Keep track of the neighbours
        int neighbourCount = 0;

        //Loop through the other creatures
        for(int i = 0; i < otherCreatures.Length; i++) {
            //Make sure it isn't computing with itself
            if(otherCreatures[i] != null && otherCreatures[i] != agentRef.gameObject) {
                //Make sure it is within the cohesion range
                if(Vector3.Distance(agentRef.transform.position, otherCreatures[i].transform.position) > agentRef.getCohesionDistance()) {
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
        velocity = velocity - agentRef.transform.position;

        //Normalize and return
        velocity.Normalize();
        return velocity;
    }

    /// <summary>
    /// Returns a velocity vector for the creature's seperation to other creatures
    /// </summary>
    /// <param name="otherCreatures">Array of other creatures</param>
    /// <returns></returns>
    private Vector3 computeSeperation(GameObject[] otherCreatures) {
        //Init the return value
        Vector3 velocity = Vector3.zero;
        //Keep track of the neighbours
        int neighbourCount = 0;

        //Loop through the other creatures
        for(int i = 0; i < otherCreatures.Length; i++) {
            //Make sure it isn't computing with itself
            if(otherCreatures[i] != null && otherCreatures[i] != agentRef.gameObject) {
                //Make sure it is within the seperation range
                if(Vector3.Distance(agentRef.transform.position, otherCreatures[i].transform.position) <= agentRef.getSeperationDistance()) {
                    //Add on the distacne from the agent
                    velocity += otherCreatures[i].transform.position - agentRef.transform.position;
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
    private Vector3 computeFlocking(GameObject[] otherCreatures) {
        //Get the three steering behaviours
        Vector3 allignment = computeAllignment(otherCreatures);
        Vector3 cohesion = computeCohesion(otherCreatures);
        Vector3 seperation = computeSeperation(otherCreatures);

        //Set the desired velocity
        agentRef.setDesiredVelocity(agentRef.calculateSpeed(allignment * agentRef.getAllignmentWeight() + cohesion * agentRef.getCohesionWeight() + seperation * agentRef.getSeperationWeight()));

        //Return the steering force
        return agentRef.getDesiredVelocity() - agentRef.getVelocity();
    }
}

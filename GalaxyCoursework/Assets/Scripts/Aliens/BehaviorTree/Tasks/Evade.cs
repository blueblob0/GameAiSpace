using UnityEngine;
using System.Collections;
using System;

/*
 * The task to evade enemies
 */
public class Evade : Task {
    //Reference to the agent using this behavior
    protected AlienAI agentRef;
    //constructor
    public Evade(AlienAI agent) {
        agentRef = agent;
    }

    public bool activate() {
        //Move as fast as possible
        agentRef.setTargetSpeed(agentRef.getMaximumSpeed());
        //Evade target
        agentRef.addSteeringForce(evade(agentRef.getTarget()));
        return true;
    }

    /// <summary>
    /// Makes the agent flee from the target
    /// </summary>
    /// <param name="targetWorldPos">World position of the target to flee from</param>
    /// <returns>Returns the steering force of the desired direction</returns>
    private Vector3 flee(Vector3 targetWorldPos) {
        //Set the diesried velocity to the away from the target
        agentRef.setDesiredVelocity(agentRef.calculateSpeed(agentRef.transform.position - targetWorldPos));
        //Return the steering force of the desired velocity
        return agentRef.getDesiredVelocity() - agentRef.getVelocity();
    }

    /// <summary>
    /// Returns the steering force to eveade the target
    /// </summary>
    /// <param name="target">Target to evade</param>
    /// <returns></returns>
    private Vector3 evade(AlienAI target) {
        //How far ahead (as time) to to evade the target
        float t = Vector3.Distance(target.transform.position, agentRef.transform.position) / (agentRef.getCurrentSpeed() * Time.deltaTime);
        //Get the future position of the agent
        Vector3 futurePosition = target.transform.position + (target.getVelocity() * t);
        //Return the flee steering of the future position
        return flee(futurePosition);
    }
}

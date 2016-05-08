//Script made by: 626224
using UnityEngine;
using System.Collections;

/*
 * The task to pursue a target
 */
public class Pursue : Task {
    //Reference to the agent using this behavior
    protected AlienAI agentRef;
    //constructor
    public Pursue(AlienAI agent) {
        agentRef = agent;
    }

    public bool activate() {
        //Quick check to calculate arrival speed
        float distance = Vector3.Distance(agentRef.transform.position, agentRef.getTarget().transform.position);
        if(distance > agentRef.getBreakingDistance()) {
            //Move as fast as possible
            agentRef.setTargetSpeed(agentRef.getMaximumSpeed());
        } else {
            //Set the speed based off how far away the agent is
            agentRef.setTargetSpeed(agentRef.getMaximumSpeed() * (distance / agentRef.getBreakingDistance()));
        }

        //Pursue the target
        agentRef.addSteeringForce(pursueSteering(agentRef.getTarget()));
        return true;
    }

    /// <summary>
    /// Makes the agent seek the target
    /// </summary>
    /// <param name="targetWorldPos">The world position of the target to seek</param>
    /// <returns>Returns the steering force of the desired direction</returns>
    protected Vector3 seek(Vector3 targetWorldPos) {
        //Set the diesried velocity to the direction of the target
        agentRef.setDesiredVelocity(agentRef.calculateSpeed(targetWorldPos - agentRef.transform.position));
        //Return the steering force of the desired velocity
        return agentRef.getDesiredVelocity() - agentRef.getVelocity();
    }

    /// <summary>
    /// Returns the steering force for persuing the target
    /// </summary>
    /// <param name="target">The agent to seek</param>
    /// <returns></returns>
    protected Vector3 pursueSteering(AlienAI target) {
        //How far ahead (as time) to persue the target
        float t = Vector3.Distance(target.transform.position, agentRef.transform.position) / (agentRef.getCurrentSpeed() * Time.deltaTime);
        //Get the future position of the agent
        Vector3 futurePosition = target.transform.position + (target.getVelocity() * t);
        //Return the seek steering of the future position
        return seek(futurePosition);
    }
}

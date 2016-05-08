//Script made by: 626224
using UnityEngine;
using System.Collections;

/*
 * Seeks the reproduction target
 */
public class MoveToReproductionTarget : Task {
    //Reference to the agent using this behavior
    protected AlienAI agentRef;
    //constructor
    public MoveToReproductionTarget(AlienAI agent) {
        agentRef = agent;
    }

    public bool activate() {
        //Set the speed
        agentRef.setTargetSpeed(5);
        //Move to target
        agentRef.addSteeringForce(seek(agentRef.getReproductionTarget().transform.position));
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
}

//Script made by: 626224
using UnityEngine;
using System.Collections;

/*
 * The task to get a target for the agent
 */
public class GetTarget : Task {
    //Reference to the agent using this behavior
    protected AlienAI agentRef;
    //constructor
    public GetTarget(AlienAI agent) {
        agentRef = agent;
    }

    public bool activate() {
        //Loop through and get the first target
        //TODO get a target bassed off of the 'shouldAttack' function
        foreach(AlienAI alien in agentRef.getNearTargets()) {
            agentRef.setTarget(alien);
            break; //Break for now
        }
        return true;
    }
}

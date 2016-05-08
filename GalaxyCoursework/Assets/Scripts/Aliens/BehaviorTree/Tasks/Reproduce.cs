//Script made by: 626224
using UnityEngine;
using System.Collections;

/*
 * The Task that will make the agent reproduce
 */
public class Reproduce : Task {
    //Reference to the agent using this behavior
    protected AlienAI agentRef;
    //constructor
    public Reproduce(AlienAI agent) {
        agentRef = agent;
    }

    public bool activate() {
        agentRef.reproduce();
        return true;
    }
}

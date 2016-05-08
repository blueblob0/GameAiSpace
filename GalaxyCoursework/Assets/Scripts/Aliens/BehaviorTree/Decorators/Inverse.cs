//Script made by: 626224
using UnityEngine;
using System.Collections;
using System;

/*
 * Inverse decorator
 */
public class Inverse : Task {
    //Reference to the agent using this behavior
    protected AlienAI agentRef;

    //The child this decorator inverses
    private Task child;

    //constructor
    public Inverse(AlienAI agent, Task child) {
        //Set the agent
        agentRef = agent;
        //Set the child
        this.child = child;
    }

    public bool activate() {
        return !child.activate();
    }
}

//Script made by: 626224
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*
 * Selector for the behavior tree
 */
public class Selector : Task {
    //Reference to the agent using this behavior
    protected AlienAI agentRef;

    //The children this selector activates
    private List<Task> children;

    //constructor
    public Selector(AlienAI agent) {
        //Set the agent
        agentRef = agent;
        //Initialize the list
        children = new List<Task>();
    }

    public bool activate() {
        //Loop through each child node and activate them
        foreach(Task child in children) {
            if(child.activate()) {
                return true;
            }
        }
        //Return false if it did not select an action
        return false;
    }

    /// <summary>
    /// Adds a child onto this node
    /// </summary>
    /// <param name="child">Child to add</param>
    public void addChild(Task child) {
        children.Add(child);
    }
}

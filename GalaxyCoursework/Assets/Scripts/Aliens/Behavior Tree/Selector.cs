using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*
 * Selector for the behavior tree
 */
public class Selector : Task {
    //The children this sequencer activates
    private List<Task> children;

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

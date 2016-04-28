using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*
 * Sequencer for the behavior tree
 */
public class Sequence : Task {
    //The children this sequencer activates
    private List<Task> children;

    public bool activate() {
        //Loop through each child node and activate them
        foreach(Task child in children) {
            if(!child.activate()) {
                return false;
            }
        }
        //Return true when all children are sucessful
        return true;
    }

    /// <summary>
    /// Adds a child onto this node
    /// </summary>
    /// <param name="child">Child to add</param>
    public void addChild(Task child) {
        children.Add(child);
    }
}

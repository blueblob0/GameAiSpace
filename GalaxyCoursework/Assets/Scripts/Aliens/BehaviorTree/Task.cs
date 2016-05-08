//Script made by: 626224
using UnityEngine;
using System.Collections;

/*
 * Interface class for each task, sequence and selector of the behavior tree
 */
public interface Task {

    /// <summary>
    /// Called to activate this task
    /// </summary>
    /// <returns></returns>
    bool activate();
}

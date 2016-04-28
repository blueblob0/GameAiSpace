using UnityEngine;
using System.Collections;

/*
 * The selector to control the Idle behavior
 */
public class IdleSelector : Selector {

    //The tasks for this behavior
    private FlockSequence flock;
    private Wander wander;

    public IdleSelector(AlienAI agent) : base(agent) {
        //Construct tasks
        flock = new FlockSequence(agent);
        wander = new Wander(agent);

        //Add in the tasks, order is important
        addChild(flock);
        addChild(wander);
    }
}

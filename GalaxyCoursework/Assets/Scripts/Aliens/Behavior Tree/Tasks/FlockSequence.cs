using UnityEngine;
using System.Collections;

/*
 * The sequence to control the flocking of the agent
 */
public class FlockSequence : Sequence {

    //The tasks that make this sequence
    AlliesNear near;
    Flock flock;

    //constructor
    public FlockSequence(AlienAI agent) : base(agent) {
        //Construct tasks
        near = new AlliesNear(agent);
        flock = new Flock(agent);

        //Add the tasks to the list of children, order is important
        addChild(near);
        addChild(flock);
    }
}

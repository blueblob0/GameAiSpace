using UnityEngine;
using System.Collections;

/*
 * Sequence to geta target
 */
public class GetTargetSequence : Sequence {
    //Tasks for this sequence
    private EnemyInSight inSight;
    private GetTarget getTarget;

    //constructor
    public GetTargetSequence(AlienAI agent) : base (agent) {
        //Construct the tasks
        inSight = new EnemyInSight(agent);
        getTarget = new GetTarget(agent);

        //Add the children, order is important
        addChild(inSight);
        addChild(getTarget);
    }
}

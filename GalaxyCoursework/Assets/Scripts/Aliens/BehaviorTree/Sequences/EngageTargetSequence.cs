//Script made by: 626224
using UnityEngine;
using System.Collections;

/*
 * Sequence to check if a target is present then pursue or attack the target
 */
public class EngageTargetSequence : Sequence {
    //Tasks for this sequence
    private HaveTarget haveTarg;
    private PursueAttackSelector purseAttack;

    //construct
    public EngageTargetSequence(AlienAI agent) : base(agent) {
        //Construct the tasks
        haveTarg = new HaveTarget(agent);
        purseAttack = new PursueAttackSelector(agent);

        //Add the children, order matters
        addChild(haveTarg);
        addChild(purseAttack);
    }
}

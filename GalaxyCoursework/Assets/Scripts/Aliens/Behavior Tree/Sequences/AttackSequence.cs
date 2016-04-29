using UnityEngine;
using System.Collections;

/*
 * The sequence to attack a target
 */
public class AttackSequence : Sequence {
    //Tasks for this sequence
    private HaveTarget haveTarget;
    private EnemyInRange inRange;
    private Attack attack;

    //constructor
    public AttackSequence(AlienAI agent) : base (agent) {
        //Construct the tasks
        haveTarget = new HaveTarget(agent);
        inRange = new EnemyInRange(agent);
        attack = new Attack(agent);

        //Add the children, order is important
        addChild(haveTarget);
        addChild(inRange);
        addChild(attack);
    }
}

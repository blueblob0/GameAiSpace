//Script made by: 626224
using UnityEngine;
using System.Collections;

/*
 * Sequence to check if an enemy is in range and to attack that enemy
 */
public class AttackTargetSequence : Sequence {
    //The tasks for this sequence
    private EnemyInRange inRange;
    private Attack attack;

    public AttackTargetSequence(AlienAI agent) : base(agent) {
        //Construct the sequences
        inRange = new EnemyInRange(agent);
        attack = new Attack(agent);

        //Add the tasks, order matters
        addChild(inRange);
        addChild(attack);
    }
}

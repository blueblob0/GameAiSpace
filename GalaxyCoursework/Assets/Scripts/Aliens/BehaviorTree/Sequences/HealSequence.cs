using UnityEngine;
using System.Collections;

/*
 * Sequence for healing the agent
 */
public class HealSequence : Sequence {
    //Tasks for this sequence
    private HealthNotFull hNotFull;
    private Heal heal;

    //constructor
    public HealSequence(AlienAI agent) : base(agent) {
        //Construct the tasks
        hNotFull = new HealthNotFull(agent);
        heal = new Heal(agent);

        //Add the children, order is important
        addChild(hNotFull);
        addChild(heal);
    }
}

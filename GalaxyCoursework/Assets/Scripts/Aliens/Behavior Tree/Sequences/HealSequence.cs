using UnityEngine;
using System.Collections;

/*
 * Sequence for healing the agent
 */
public class HealSequence : Sequence {
    //Tasks for this sequence
    private HealthLow hLow;
    private Heal heal;

    //constructor
    public HealSequence(AlienAI agent) : base(agent) {
        //Construct the tasks
        hLow = new HealthLow(agent);
        heal = new Heal(agent);

        //Add the children, order is important
        addChild(hLow);
        addChild(heal);
    }
}

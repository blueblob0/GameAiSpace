//Script made by: 626224
using UnityEngine;
using System.Collections;

public class EscapeSequence : Sequence {

    private HealthLow HPlow;
    private EnemyInSight sight;
    private EnemyTargetingSelf targettingSelf;
    private Evade evade;

    //constrcutor
    public EscapeSequence(AlienAI agent) : base(agent) {
        //Construct the tasks
        HPlow = new HealthLow(agent);
        sight = new EnemyInSight(agent);
        targettingSelf = new EnemyTargetingSelf(agent);
        evade =  new Evade(agent);

        //Add the tasks to the list of children, order is important
        addChild(HPlow);
        addChild(sight);
        addChild(targettingSelf);
        addChild(evade);
    }
}

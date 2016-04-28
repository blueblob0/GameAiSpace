using UnityEngine;
using System.Collections;

public class EscapeSequence : Sequence {

    private EnemyInSight sight;
    private EnemyTargetingSelf targettingSelf;
    private HealthLow HPlow;
    private Evade evade;

    //constrcutor
    public EscapeSequence(AlienAI agent) : base(agent) {
        //Construct the tasks
        sight = new EnemyInSight(agent);
        targettingSelf = new EnemyTargetingSelf(agent);
        HPlow = new HealthLow(agent);
        evade =  new Evade(agent);

        //Add the tasks to the list of children, order is important
        addChild(sight);
        addChild(targettingSelf);
        addChild(HPlow);
        addChild(evade);
    }
}

using UnityEngine;
using System.Collections;

/*
 * The sequence that checks if any enemies are near before doing the recoup selector
 */
public class RecoupSequence : Sequence {
    //The tasks this sequence uses
    EnemyInSight insight;
    RecoupSelector recSelect;
    //The inverse decorator
    Inverse inv;

    public RecoupSequence(AlienAI agent) : base(agent){
        //Construct the tasks
        insight = new EnemyInSight(agent);
        recSelect = new RecoupSelector(agent);
        inv = new Inverse(agent, insight);

        //Add the children, order matters
        addChild(inv);
        addChild(recSelect);
    }
}

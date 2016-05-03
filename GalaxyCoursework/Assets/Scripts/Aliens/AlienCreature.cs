using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* The super class that will handle all the characteristics
 * of the alien creatures that will spawn, all the random spawning variables
 * like arms, legs, heads will happen here too
 *
 * Each alien will have a unique set of arms/legs/heads (set in inspector) but will always spawn
 * between 1 & 10 for example, then these creatures can be 'copied' to populate a planet
 */


public class AlienCreature : AlienAI {


    

    

    
    /// <summary>
    /// Will spawn the creature into the game world
    /// </summary>
    public override void Start() {
        base.Start();

        
    }

    //Had to put these 2 functions here to make it easier to check for species

    

    /// <summary>
    /// Update the creature to peform movement ect.
    /// </summary>
    public override void Update() {
        base.Update();

        //DEBUG-------------------------------------
        if(Input.GetKeyDown(KeyCode.Space)) {
            //StartCoroutine(reproduce());
        }
        //------------------------------------------

        
    }

    

   

    

   

    

    

    
    
   
}

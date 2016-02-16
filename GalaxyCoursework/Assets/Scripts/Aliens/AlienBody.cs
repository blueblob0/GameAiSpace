using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Simple class that just holds some basic info on the body
 */

public class AlienBody : MonoBehaviour{
    private List<GameObject> headSpots;
    private List<GameObject> armSpots;
    private List<GameObject> legSpots;

    //Inits the body, must be called firstd
    public void initialize() {
        headSpots = new List<GameObject>();
        armSpots = new List<GameObject>();
        legSpots = new List<GameObject>();

        //Get all the head/arm/leg spots on start
        for(int i = 0; i < transform.childCount; i++) {
            //Get each child
            Transform child = transform.GetChild(i);
            //Then check their name to see what 'spot' they are
            if(child.name[0] == 'H') {
                headSpots.Add(child.gameObject);
            }
            if(child.name[0] == 'A') {
                armSpots.Add(child.gameObject);
            }
            if(child.name[0] == 'L') {
                legSpots.Add(child.gameObject);
            }
        }
    }

    //These functions rotate the limbs (for walknig ect)
    public void rotateArms(float XAmount, float YAmount, float ZAmount) {
        Vector3 rot = new Vector3(XAmount, YAmount, ZAmount);
        rotateArms(rot);
    }
    public void rotateArms(Vector3 eulerAmount) {
        foreach(GameObject arm in armSpots) {
            arm.transform.Rotate(eulerAmount);
        }
    }

    public void rotateLegs(float XAmount, float YAmount, float ZAmount) {
        Vector3 rot = new Vector3(XAmount, YAmount, ZAmount);
        rotateLegs(rot);
    }
    public void rotateLegs(Vector3 eulerAmount) {
        foreach(GameObject leg in legSpots) {
            leg.transform.Rotate(eulerAmount);
        }
    }

    //Getters
    public int getHeadSpotCount() {
        return headSpots.Count;
    }
    public int getArmSpotCount() {
        return armSpots.Count;
    }
    public int getLegSpotCount() {
        return legSpots.Count;
    }
}

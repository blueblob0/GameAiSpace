using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Simple class that just holds some basic info on the body
 */

public class AlienBody : MonoBehaviour{
    public GameObject   body;
    public GameObject[] headSpots;
    public GameObject[] armSpots;
    public GameObject[] legSpots;
    public GameObject[] wingSpots;

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
        return headSpots.Length;
    }
    public int getArmSpotCount() {
        return armSpots.Length;
    }
    public int getLegSpotCount() {
        return legSpots.Length;
    }
    public int getWingSpotCount() {
        return wingSpots.Length;
    }
}

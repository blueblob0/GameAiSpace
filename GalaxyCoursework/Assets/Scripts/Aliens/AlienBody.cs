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

    //These functions add and parent a gameobject onto the available slots
    public void addHead(GameObject headPrefab) {
        //Loop through the available spots
        foreach(GameObject spot in headSpots) {
            //Check if the spot is free
            if(spot.transform.childCount == 0) {
                //Add and set up the new body part
                GameObject head = GameObject.Instantiate<GameObject>(headPrefab);
                head.transform.SetParent(spot.transform);
                head.transform.position = spot.transform.position;
                head.transform.rotation = spot.transform.rotation;
            }
        }
    }
    public void addArm(GameObject armPrefab) {
        //Loop through the available spots
        foreach(GameObject spot in armSpots) {
            //Check if the spot is free
            if(spot.transform.childCount == 0) {
                //Add and set up the new body part
                GameObject arm = GameObject.Instantiate<GameObject>(armPrefab);
                arm.transform.SetParent(spot.transform);
                arm.transform.position = spot.transform.position;
                arm.transform.rotation = spot.transform.rotation;
            }
        }
    }
    public void addLeg(GameObject legPrefab) {
        //Loop through the available spots
        foreach(GameObject spot in legSpots) {
            //Check if the spot is free
            if(spot.transform.childCount == 0) {
                //Add and set up the new body part
                GameObject leg = GameObject.Instantiate<GameObject>(legPrefab);
                leg.transform.SetParent(spot.transform);
                leg.transform.position = spot.transform.position;
                leg.transform.rotation = spot.transform.rotation;
            }
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

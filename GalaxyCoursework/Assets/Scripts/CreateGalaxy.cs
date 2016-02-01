using UnityEngine;
using System.Collections.Generic;

public class CreateGalaxy : MonoBehaviour {

    public int numStars = 100;
    public List<GameObject> stars = new List<GameObject>();
    public string starPrefabName = "StarPrefab";
    private int spaceBetween =30;
	// Use this for initialization
	void Start () {
        Vector3 starPos;
        int xPos = 0;
        int yPos = 0;
        int zPos = 0;
        for (int i = 0; i < numStars; i++)
        {
            bool cantSpawn = true;
            int count =0;
            while (cantSpawn)
            {
                count++;
                xPos = Random.Range(-10000, 10000);
                zPos = Random.Range(-10000, 10000);
                cantSpawn = CheckStarAlreadyThere(xPos, zPos);
                // this is to stop it lookng over and over and creating an endless loop
                if (count>20)
                {
                    cantSpawn = false;
                    i = numStars;
                    Debug.Log("cant find spot");
                }
            }


            CheckStarNear(xPos, zPos);
            starPos = new Vector3(xPos,yPos,zPos);
            GameObject a = Instantiate(Resources.Load(starPrefabName)) as GameObject;
            Debug.Log("need to add lod to planets");
            stars.Add(a);
            a.transform.position = starPos;
           
        }



	}

    bool CheckStarAlreadyThere(int xcord,int zcord)
    {
        
        foreach(GameObject s in stars)
        {
            // if theese are true than the 
           // if(s.transform.position.x -spaceBetween >xcord || s.transform.position.x + spaceBetween < xcord){}

            // if this is true then the planet is within the x of this
            if (s.transform.position.x - spaceBetween < xcord && s.transform.position.x + spaceBetween > xcord)
            {
                if (s.transform.position.z - spaceBetween < zcord && s.transform.position.z + spaceBetween > zcord)
                {
                    Debug.Log(s.transform.position.x + " "+ xcord+ " "+ s.transform.position.z + " " + zcord);
                    // if this is ever reached then there is an overlap
                    return true;
                }
            }

        }


        return  false;
        
    }


    void CheckStarNear(int xcord, int zcord)
    {
        int spaceNear =100;
        foreach (GameObject s in stars)
        {
            // if this is true then the planet is within the x of this
            if (s.transform.position.x - spaceNear < xcord && s.transform.position.x + spaceNear > xcord)
            {
                if (s.transform.position.z - spaceNear < zcord && s.transform.position.z + spaceNear > zcord)
                {
                    s.name = "1";
                    Debug.Log(s.transform.position.x + " " + xcord + " " + s.transform.position.z + " " + zcord);
                }
            }

        }
    }

    // Update is called once per frame
    void Update () {
	
	}
}

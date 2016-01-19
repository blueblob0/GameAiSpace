using UnityEngine;
using System.Collections.Generic;

public class CreateGalaxy : MonoBehaviour {

    public int numStars = 10;
    public List<GameObject> stars = new List<GameObject>();
    public string starPrefabName = "StarPrefab";

	// Use this for initialization
	void Start () {
        Vector3 starPos;
        int xPos = 0;
        int yPos = 0;
        int zPos = 0;
        for (int i = 0; i < numStars; i++)
        {
            xPos = Random.Range(0, 100);
            zPos = Random.Range(0, 100);
            starPos = new Vector3(xPos,yPos,zPos);
            GameObject a = Instantiate(Resources.Load(starPrefabName)) as GameObject;
            stars.Add(a);
            a.transform.position = starPos;
        }



	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

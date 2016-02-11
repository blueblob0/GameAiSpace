using UnityEngine;
using System.Collections;

public class BlackHole : CelestialBody
{

	// Use this for initialization
	protected override void  Start ()
    {
        base.Start();
        mass = 100;
       

        
        transform.localScale = Vector3.one * (mass );
    }
	
	// Update is called once per frame
	void Update () {
        if (mass > 0)
        {
            foreach (GameObject s in controler.stars)
            {

                float force = 100 * mass/ s.GetComponent<Star>().mass; // / Vector3.Distance(transform.position, s.transform.position * s.GetComponent<Star>().mass);
                s.transform.position = Vector3.MoveTowards(s.transform.position, transform.position, force * Time.deltaTime);
                // Debug.Log(force); 
            }

        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("1");
        if(other.tag != "CelestialBody")
        {
            return;
        }

        if (!controler)
        {
            controler = FindObjectOfType<CreateGalaxy>();
        }

       
        controler.stars.Remove(other.gameObject);
        //Debug.Log(other);
        //Debug.Log(other.GetComponent<CelestialBody>());
        //mass += other.GetComponent<CelestialBody>().Mass;

        //transform.localScale = Vector3.one * (mass );

        //Debug.Log(mass);
        mass -= other.GetComponent<CelestialBody>().Mass;

        Destroy(other.gameObject);
    }


}

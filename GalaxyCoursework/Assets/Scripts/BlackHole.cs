using UnityEngine;
using System.Collections;

public class BlackHole : CelestialBody
{
    public int count =25;
    public bool move = true;
    bool check = false;
	// Use this for initialization
	protected override void  Start ()
    {
        base.Start();
        mass = 1000; 
        transform.localScale = Vector3.one * (mass );

        if (!controler)
        {
            controler = FindObjectOfType<CreateGalaxy>();
        }

        //make sure we remove planets starting inside the balckhole 
        Collider[] hitColliders =Physics.OverlapSphere(transform.position, transform.lossyScale.x);
        for(int i = 0;i< hitColliders.Length;i++)
        {
            if (hitColliders[i].gameObject != gameObject)
            {
                //remove from the list so we dont try and acces a bull gameobjecct 
                controler.DestroyStar(hitColliders[i].gameObject);
               
            }



        }

    }
	
	// Update is called once per frame
	void Update () {
        if (move)
        {
            UpdateTowardsBlackHole();

        }

    }
    //move every sun towards the backhole
    public void MoveTowardsBlackHole()
    {

        while(count > 0)
        {
            controler.moveStars(transform.position, mass);
                       
        }
    }
    public void UpdateTowardsBlackHole()
    {

        if (count > 0)
        {
            controler.moveStars(transform.position, mass);
        }
        else if(!check)
        {
            check = true;
            


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
       
        //mass -= other.GetComponent<CelestialBody>().Mass;
        //going to try removing count isntead of mass as big stars isntatly remove the black hole 
        //count--;

        count -= Mathf.CeilToInt(Mathf.Log10(other.GetComponent<CelestialBody>().Mass));

        controler.DestroyStar(other.gameObject);
    }


}

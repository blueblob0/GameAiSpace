using UnityEngine;
using System.Collections;

public class BlackHole : CelestialBody
{
    public int count =  20;
    public bool move = false;
    bool check = false;
	// Use this for initialization
	protected override void  Start ()
    {
        base.Start();
        mass = 500; 
        transform.localScale = Vector3.one * (mass );

        if (!controler)
        {
            controler = FindObjectOfType<CreateGalaxy>();
        }
        int holdcount = count; //store count as it would be lowered by any stars spawning sindie the hole 
        RemoveStarsInsideBH();
        count = holdcount;
    }

    /// <summary>
    /// make sure we remove planets inside the balckhole 
    /// </summary>
    void RemoveStarsInsideBH()
    {      
        
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, transform.lossyScale.x);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject != gameObject && hitColliders[i].name!= CreateGalaxy.removeName)
            {
                //remove from the list so we dont try and acces a bull gameobjecct 
                //going to try removing count isntead of mass as big stars isntatly remove the black hole 
                //count--;

                count -= Mathf.CeilToInt(Mathf.Log10(hitColliders[i].GetComponent<CelestialBody>().Mass));
               
                Debug.Log(hitColliders[i].name);
                hitColliders[i].name = CreateGalaxy.removeName;
                //controler.DestroyStar(hitColliders[i].gameObject);
                
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
       // StartCoroutine(MoveTowardsBlackH());
        if (!controler)
        {
            controler = FindObjectOfType<CreateGalaxy>();
        }      

        int i = 0;
        while (count > 0 &&  i < 10000)
        {
            controler.moveStars(transform.position, mass);
            RemoveStarsInsideBH(); //check for strs insdide and destroy
            i++;
           // Debug.Log(count);

        }

        //Debug.LogError("stop"  + " " + i + " " + count);
    }



    IEnumerator MoveTowardsBlackH()
    {
        if (!controler)
        {
            controler = FindObjectOfType<CreateGalaxy>();
        }
        bool first = true;

        float timehold = Time.time;
        int i = 0;
        while (count > 0 && timehold > Time.time - 60 && i < 1000)
        {
            controler.moveStars(transform.position, mass);
            RemoveStarsInsideBH(); //check for strs insdide and destroy
            i++;
            yield return new WaitForSeconds(0.1f);

        }

        Debug.LogError("stop" + (Time.time - 60) + " " + i + " " + count);


    }


    public void UpdateTowardsBlackHole()
    {

        if (count > 0)
        {
            controler.moveStars(transform.position, mass);
            RemoveStarsInsideBH();
        }
        else if(!check)
        {
            check = true;

            controler.CheckStarBlackHole();
        }

    }



    void OnTriggerEnter(Collider other)
    {
       // Debug.Log("1");
        if(other.tag != "CelestialBody")
        {
            return;
        }
        return;
         //getting rid of for now as it dosent wrok on start 
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

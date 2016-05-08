//script made by: up651590
using UnityEngine;

public class Moon : Satalite {

	// Use this for initialization
	protected  override void Start ()
    {
        base.Start();

    }

    protected override void SetScale()
    {
        transform.localScale = CreateGalaxy.planetMuti / 10 * Vector3.one;
    }
    public override void SortBiomes()
    {
        //no biomes on moon just to forfil the abstract method
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class crossHairAnimate : MonoBehaviour {


	public List<GameObject> crossHairs;
	public List<Vector3> InitPositionList;
	public List<Vector3> TargetPositionList;

	public float heatTime;
	public float coolTime;
	public float counter;

	public bool shoot = false;


	// Use this for initialization
	void Start () {
		counter = coolTime;
	}
	
	// Update is called once per frame
	void Update () {

		if (shoot) 
		{
			shoot = false;
			counter = 0;
		}
	
		UpdateAnimate();
	}

	void UpdateAnimate()
	{
		if (counter < coolTime) 
		{
			counter += Time.deltaTime;
		}
		
		if (counter >= coolTime) 
		{
			for(int i=0;i<crossHairs.Count;i++)
			{
				crossHairs[i].transform.localPosition = InitPositionList[i];
			}
		}
		else if(counter > heatTime)
		{
			float ratio = (counter - heatTime)/(coolTime-heatTime);
			
			for(int i=0;i<crossHairs.Count;i++)
			{
				crossHairs[i].transform.localPosition = Vector3.Lerp( TargetPositionList[i],InitPositionList[i],ratio);
			}
		}
		else
		{
			for(int i=0;i<crossHairs.Count;i++)
			{
				crossHairs[i].transform.localPosition = Vector3.Lerp( InitPositionList[i],TargetPositionList[i],counter/heatTime);
			}
		}
	}
}

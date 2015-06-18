using UnityEngine;
using System.Collections;

public class AccelerometerListener : MonoBehaviour {

	public CommunicationManager communicationManager;
	public float threashhold;
	public float cooldown;

	private bool block = false;

	// Use this for initialization
	void Start () {
	
	}

	void unblock()
	{
		block = false;
	}
	
	// Update is called once per frame
	void Update () {

		if (block) return;

		//Debug.Log (Input.acceleration);

		float value = Input.acceleration.magnitude;

		if (value > threashhold) 
		{
			block = true;	
			Invoke("unblock",cooldown);

			JSONObject commandJsonObject = new JSONObject();
			Debug.Log("acceleration:"+value);
			commandJsonObject.AddField("command","acceleration");
			commandJsonObject.AddField("value",value);
			communicationManager.SendJson (commandJsonObject);

		}


		
	}
}

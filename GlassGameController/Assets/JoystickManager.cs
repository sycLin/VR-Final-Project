using UnityEngine;
using System.Collections;

public class JoystickManager : MonoBehaviour {

	//public GameObject joyStickRight;

	public bool playable = false;

	public CNJoystick joystickLeft;
	public CNJoystick joystickRight;

	public CommunicationManager communicationManager;

	public bool test;

	// Use this for initialization
	void Start () {
		joystickLeft.JoystickMovedEvent += moved;
		joystickLeft.FingerLiftedEvent += stopped;

		joystickRight.JoystickMovedEvent += rotated;
		joystickRight.FingerLiftedEvent += stopRotated;

	}

	void moved(Vector3 vec3)
	{
		JSONObject commandJsonObject = new JSONObject();
		commandJsonObject.AddField("command","move");
		commandJsonObject.AddField("x",vec3.x);
		commandJsonObject.AddField("y",-(vec3.y));

		communicationManager.SendJson (commandJsonObject);
	}

	void rotated(Vector3 vec3){
		JSONObject commandJsonObject = new JSONObject();
		commandJsonObject.AddField("command","rotate");
		commandJsonObject.AddField("x",vec3.x);
		commandJsonObject.AddField("y",-(vec3.y));
		
		communicationManager.SendJson (commandJsonObject);
	}

	void stopRotated(){
		JSONObject commandJsonObject = new JSONObject();
		commandJsonObject.AddField("command","rotate");
		commandJsonObject.AddField("x",0);
		commandJsonObject.AddField("y",0);
		
		communicationManager.SendJson (commandJsonObject);
	}

	void stopped(){
		JSONObject commandJsonObject = new JSONObject();
		commandJsonObject.AddField("command","move");
		commandJsonObject.AddField("x",0);
		commandJsonObject.AddField("y",0);
		
		communicationManager.SendJson (commandJsonObject);
	}
	




	// Update is called once per frame
	void Update () 
	{
		if (test) 
		{
			test = false;	

			JSONObject commandJsonObject = new JSONObject();
			commandJsonObject.AddField("command","move");
			commandJsonObject.AddField("x",0);
			commandJsonObject.AddField("y",-1);
			
			communicationManager.SendJson (commandJsonObject);
		}
	}
}

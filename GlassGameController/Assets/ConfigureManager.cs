using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;


public class ConfigureManager : MonoBehaviour {

	public CommunicationManager communicate;
	public AccelerometerListener acc;
	public MicListener mic;
	public List<GameObject> leftJoyStick;
	public List<GameObject> rightJoyStick;
	public CNJoystick leftJoyStickScript;
	public CNJoystick rightJoyStickScript;

	public CheckBox MoveJoystickCheckBox;
	public CheckBox RotateJoystickCheckBox;
	public CheckBox VoiceControl;
	public CheckBox AccControl;
	public CheckBox EyeWink;
	public CheckBox HeadGesture;
	public CheckBox OnTrack;
	public TapGesture Exit;

	

	// Use this for initialization
	void Start () {

		MoveJoystickCheckBox.EnableChangeEvent += (bool enabled) =>
		{
			foreach (GameObject g in rightJoyStick) 
			{
				g.SetActive (enabled);
			}
			
			JSONObject json = new JSONObject();
			json.AddField("command","clearOffsetX");
			communicate.SendJson(json);
			rightJoyStickScript.enabled = enabled;
		};

		RotateJoystickCheckBox.EnableChangeEvent += (bool enabled) =>
		{

			foreach (GameObject g in leftJoyStick) 
			{
				g.SetActive (enabled);
			}
			leftJoyStickScript.enabled = enabled;
		};
		
		HeadGesture.EnableChangeEvent += (bool enabled) =>
		{
				JSONObject json = new JSONObject ();
				json.AddField ("command", "headGesture");
				json.AddField ("value", enabled);
				communicate.SendJson (json);
		};

		OnTrack.EnableChangeEvent += (bool enabled) =>
		{
			JSONObject json = new JSONObject ();
			json.AddField ("command", "onTrack");
			json.AddField ("value", enabled);
			communicate.SendJson (json);
		};
		
		VoiceControl.EnableChangeEvent += (bool enabled) =>
		{
			mic.enabled = enabled;
		};

		AccControl.EnableChangeEvent += (bool enabled) =>
		{
			acc.enabled = enabled;
		};

		EyeWink.EnableChangeEvent += (bool enabled) =>
		{
			//;
			JSONObject json = new JSONObject ();
			json.AddField ("command", "changeBlink");
			json.AddField ("state", enabled);
			Debug.Log (json);
			communicate.SendJson (json);

		};

		Exit.Tapped += (object sender, System.EventArgs e) => 
		{
			JSONObject json = new JSONObject ();
			json.AddField ("command", "exit");
			communicate.SendJson (json);
		};


		
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

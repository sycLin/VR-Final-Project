using UnityEngine;
using System.Collections;

public class GyroManager : MonoBehaviour {

	public NetworkManager network;

	public float updateRate = 0.1f;

	public float counter =0;

	public bool Transfering;

	public CommunicationManager communicationManager;

	public GameObject GyroTest;

	// Use this for initialization
	void Start () {
		Input.gyro.enabled = true;

		network.networkCommandEvent += NetworkCommand;
	}

	public void NetworkCommand(JSONObject obj)
	{
		if (obj ["command"].str == "IMU1") 
		{
			float yaw = ((float)obj["yaw"].n);
			float pitch = ((float)obj["pitch"].n);
			float roll = ((float)obj["roll"].n);
			Debug.Log("yaw:"+yaw+" pitch:"+pitch+" roll:"+roll);

			GyroTest.transform.rotation = Quaternion.Euler(pitch,yaw,roll);
			//GyroTest.transform.Rotate(new Vector3(0,-yaw,0));
			//GyroTest.transform.Rotate(new Vector3(-pitch,0,0));
			//GyroTest.transform.Rotate(new Vector3(0,0,roll));
		}
	}

	public Quaternion lastQ;


	public Vector3 lastAcc;

	
	// Update is called once per frame
	void Update () {

		lastQ = Input.gyro.attitude;
		lastAcc = Input.acceleration;

		if (Transfering) {

			counter += Time.deltaTime;

						if (counter > updateRate) {
								counter = 0;

								JSONObject json = new JSONObject ();
								json.AddField ("command", "gyro");
								json.AddField ("x", lastQ.x);
								json.AddField ("y", lastQ.y);
								json.AddField ("z", lastQ.z);
								json.AddField ("w", lastQ.w);
								json.AddField ("accX", lastAcc.x);
								json.AddField ("accY", lastAcc.y);
								json.AddField ("accZ", lastAcc.z);
								communicationManager.SendJson (json);


						}

				}

	}
}

using UnityEngine;
using System.Collections;

public class TiltListener : MonoBehaviour {

	public Camera mainCamera;

	public float TitltData;

	public float Threashhold = 8;
	public float maxHold = 12;

	public float initSpeed = 0.2f;
	public float maxSpeed = 0.5f;
	
	public PlayerController playerController;

	// Use this for initialization
	void Start () {
		clearTiltingData ();
	}


	void OnDisable () {
		clearTiltingData();
//		Debug.Log("disabled");
	}

	public void clearTiltingData()
	{
		playerController.tiltingDataV3 = Vector3.zero;
	}

	// Update is called once per frame
	void Update () {

		Vector3 up = mainCamera.transform.up;
		up.y = 0;

		//Debug.Log ("mag:"+up.magnitude);

		if (up.magnitude > Threashhold) 
		{
			Vector3 normalUP = up.normalized;

			float ratio = (up.magnitude - Threashhold) / (maxHold - Threashhold);

			//Debug.Log ("ratio:"+ratio);
			Vector3 vec = normalUP * ( ratio * (maxSpeed-initSpeed) + initSpeed);
			playerController.tiltingDataV3 = vec;
			//Debug.Log ("tiltingData:"+playerController.tiltingDataV3);
		}
		else 
		{
			playerController.tiltingDataV3 = Vector3.zero;
		}


		/*
	 	Vector3 up = mainCamera.transform.up;
		Vector3 right = mainCamera.transform.right;
		Vector3 planeRight = right;

		planeRight.y = 0;
		planeRight.Normalize ();


		TitltData = Vector3.Angle(up, planeRight)-90;

		if (Mathf.Abs (TitltData) > Threashhold) 
		{
			if(TitltData>0)
			{
				float ratio = (TitltData-Threashhold) / (maxHold - Threashhold);
				float speed = ratio*maxSpeed + (1-ratio)*initSpeed;

				playerController.tiltData = -speed;
			}
			else if(TitltData<0)
			{
				float ratio = -(TitltData+Threashhold) / (maxHold - Threashhold);
				float speed = ratio*maxSpeed + (1-ratio)*initSpeed;


				playerController.tiltData = speed;
			}
		}
		else
		{
			playerController.tiltData = 0;
		}*/

	}

}

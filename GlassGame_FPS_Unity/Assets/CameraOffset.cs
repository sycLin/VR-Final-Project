using UnityEngine;
using System.Collections;

public class CameraOffset : MonoBehaviour {

	public GameObject targetCamera;
	public GameObject lookAt;

	//public int CountDown = 90;

	public void LookAt(GameObject target,float time)
	{
		LerpingTime = time;
		lookAt = target;

		Vector3 difVec3 = lookAt.transform.position - targetCamera.transform.position;
		difVec3.Normalize ();
		float targetRotationY =  Mathf.Atan2(difVec3.x,difVec3.z);
		targetRotationY = (float)(targetRotationY/ Mathf.PI * 180);

		Vector3 vec3 = targetCamera.transform.forward;

		double data = Mathf.Atan2(vec3.x,vec3.z);


		double result = data / Mathf.PI * 180;

		Vector3 rotate = transform.localRotation.eulerAngles;
		init = rotate;

		float dif = - (float)result + (float)targetRotationY;

		if (dif > 180)
						dif -= 360;
		else if (dif < - 180)
						dif += 360;

		rotate.y += dif;


		currentCounter = 0;

		end = rotate;
		animating = true;

	}

	Vector3 init;
	Vector3 end;
	public float LerpingTime;
	float currentCounter;
	bool animating = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.A)) {
			this.LookAt(lookAt,LerpingTime);
		}

		if (animating) 
		{
			currentCounter += Time.deltaTime;


			float ratio = currentCounter / LerpingTime;

			if(ratio > 1)
			{
				ratio = 1;
				animating = false;
			}

			Vector3 result = end * ratio + (init * (1 - ratio));

			transform.localRotation = Quaternion.Euler(result);
				
		}
	}
}

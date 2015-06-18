using UnityEngine;
using System.Collections;

public class SteelController : MonoBehaviour {

	private float rotateAngle = 180;
	public float rotateRatio = 0.3f;
	// Use this for initialization
	void Start () {
		rotateAngle = gameObject.transform.rotation.z;
	}
	
	// Update is called once per frame
	void Update () {
		//update rotation
		Vector3 tempAngle = gameObject.transform.rotation.eulerAngles;
		tempAngle.z = rotateAngle;
		Quaternion rotateQuaternion = Quaternion.Euler (tempAngle);
		gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation,rotateQuaternion,rotateRatio);
	}

	public void setRotateAngle(float angle){
		this.rotateAngle = angle;
	}
}

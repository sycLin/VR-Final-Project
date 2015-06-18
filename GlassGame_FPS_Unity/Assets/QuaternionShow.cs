using UnityEngine;
using System.Collections;

public class QuaternionShow : MonoBehaviour {

	public Quaternion quaternion;

	// Use this for initialization
	void Start () {
		gameObject.transform.localRotation = new Quaternion (0.7071f,0,0,0.7071f);
	}
	
	// Update is called once per frame
	void Update () {
		quaternion = gameObject.transform.localRotation;
	}
}

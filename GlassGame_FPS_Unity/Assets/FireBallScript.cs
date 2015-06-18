using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireBallScript : MonoBehaviour {


	float Timer = 0;

	public Vector3 Velicity;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		Timer += Time.deltaTime;

		if (Timer > 5) 
		{
			GameObject.Destroy (gameObject);
		}

		this.transform.position += Velicity * Time.deltaTime;
	
	}

	List<Collider> touched = new List<Collider>();

	void OnTriggerEnter(Collider other) {

		if (touched.Contains (other))
						return;
		touched.Add(other);

		Debug.Log ("test");
		//Destroy(other.gameObject);

		MonsterScript monster = other.gameObject.GetComponent<MonsterScript> ();
		if (monster != null) 
		{
			monster.Hurt(10);
		}
	}

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletController : MonoBehaviour {
	public float lifeTime = 3.0f;
	private float alreadyLiveTime;
	public int attackValue;

	private List<Collider> contactList = new List<Collider>();
	// Use this for initialization
	void Start () {
		//init
		alreadyLiveTime = 0.0f;

	}


	void OnTriggerEnter(Collider other) 
	{
		if (contactList.Contains (other))return;

		ZombieBehavior ms = other.gameObject.GetComponent<ZombieBehavior>();

		if (ms) 
		{
			ms.Hurt(attackValue);
		}

		contactList.Add(other);
	}

	// Update is called once per frame
	void Update () {
		//delete

		alreadyLiveTime += Time.deltaTime;
		if (alreadyLiveTime > lifeTime)
		{
			GameObject.Destroy (gameObject);
		}

	}
}

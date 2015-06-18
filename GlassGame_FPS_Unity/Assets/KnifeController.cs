using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KnifeController : MonoBehaviour {

	public int damage;

	public bool attacking;

	public float attackTiming;

	List<Collider> inRanged = new List<Collider>();
	AnimationClip attackClip;

	// Use this for initialization
	void Start () {
		attackClip = animation.GetClip ("Cut01 Knife01Mobile");

	}

	public void Attack()
	{
		if (!attacking) 
		{
			attacking = true;
			animation.Play("Cut01 Knife01Mobile");
			Invoke ("Done", attackClip.length);
			Invoke ("AttackAllInRanged",attackTiming);
		}
	}

	public void Done()
	{
		attacking = false;
	}

	void OnTriggerEnter(Collider other) 
	{
		inRanged.Add(other);

	}


	void OnTriggerExit(Collider other)
	{
		inRanged.Remove(other);

	}

	public void AttackAllInRanged()
	{
		List<Collider> check = new List<Collider> ();
		check.AddRange(inRanged);

		foreach (Collider col in check) 
		{
			if (col==null) inRanged.Remove(col);
		}

		foreach (Collider col in inRanged) 
		{
			ZombieBehavior m = col.GetComponent<ZombieBehavior>();
			m.Hurt(damage);
		}
	}


}

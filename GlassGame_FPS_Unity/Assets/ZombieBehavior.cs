using UnityEngine;
using System.Collections;

public class ZombieBehavior : MonoBehaviour {

	public float moveSpeed;
	public float attackDistance;
	public float HP = 100;
	public float idle = 0;
	public MonsterSpwanNodes monsterSpwanNodes;

	public GameObject player;


	private Animator animator;
	private bool animating = false;

	// Use this for initialization
	void Start () 
	{
		animator = GetComponent<Animator>();
	}

	public void reset()
	{
		HP = 30;
		idle = 0;
		animator.SetBool("Attack",false);
		animator.SetBool("BeAttack",false);
		animator.SetBool("Dead",false);
		//animator.set
	}

	public void Hurt(float value)
	{
		if(HP>0)
		{
			if(this.animator.GetCurrentAnimatorStateInfo(0).IsName ("BeAttack") ||
			   this.animator.GetCurrentAnimatorStateInfo(0).IsName ("Dead"))
			{
				return;
			}

			idle = 1f;
			hurtEvent = true;
			HP -= value;
		}
	}

	bool hurtEvent = false;

	public bool autoAdd = false;

	public void Destroy()
	{
		//GameObject.Destroy(gameObject);
		//monsterSpwanNodes.in
		int index = monsterSpwanNodes.index;
		if (autoAdd) 
		{
			index = monsterSpwanNodes.AddToNext ();
		}
		else 
		{
			index = (index + 1) % monsterSpwanNodes.MonsterSpwanNodeList.Count;
		}

		GameObject position = monsterSpwanNodes.MonsterSpwanNodeList[index];

		transform.position = position.transform.position;

		reset ();
	}



	public float destroyTime = 2;
	// Update is called once per frame
	void Update () {

		


		idle -= Time.deltaTime;
		if (idle < 0) idle = 0;

		/*if (Input.GetKeyDown (KeyCode.Space))
						Hurt (10);*/


		if (hurtEvent) 
		{
			hurtEvent = false;
			animating = true;

			if(HP>0)
			{
				animator.SetBool("BeAttack",true);
			}
			else
			{

				animator.SetBool("Dead",true);
				Invoke("Destroy",destroyTime);
			}

			return;
		}
		if (HP <= 0) return;

		if (this.animator.GetCurrentAnimatorStateInfo(0).IsName ("Walking")) 
		{
			animating = false;
		}

		if (!animating) 
		{
			if((player.transform.position - transform.position).magnitude < attackDistance)
			{
				animator.SetBool("Attack",true);
				animating = true;
			}
			else
			{
				animator.SetBool("Attack",false);
				animator.SetBool("BeAttack",false);

				if(idle<=0)
				{
					
					gameObject.transform.LookAt(player.transform.position);
					Vector3 dif = transform.forward * moveSpeed * Time.deltaTime;
					dif.y = 0;
					gameObject.transform.position += dif;
				}
			}
		}
		else
		{
			animator.SetBool("Attack",false);
			animator.SetBool("BeAttack",false);
		}


	}
}

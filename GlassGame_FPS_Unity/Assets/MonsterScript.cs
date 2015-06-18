using UnityEngine;
using System.Collections;

// Require these components when using this script
[RequireComponent(typeof (Animator))]
[RequireComponent(typeof (CapsuleCollider))]
[RequireComponent(typeof (Rigidbody))]
public class MonsterScript : MonoBehaviour
{

	public float hp = 50;
	public float animSpeed = 1.5f;				// a public setting for overall animator animation speed
	public float lookSmoother = 3f;				// a smoothing setting for camera motion
	public float moveSpeed = 1;
	public int attackValue = 10;
	
	private Animator anim;							// a reference to the animator on the character
	private AnimatorStateInfo currentBaseState;			// a reference to the current state of the animator, used for base layer
	private AnimatorStateInfo layer2CurrentState;	// a reference to the current state of the animator, used for layer 2
	private CapsuleCollider col;					// a reference to the capsule collider of the character
	
	
	static int idleState = Animator.StringToHash("Base Layer.Idle");	
	static int locoState = Animator.StringToHash("Base Layer.Locomotion");			// these integers are references to our animator's states
	static int jumpState = Animator.StringToHash("Base Layer.Jump");				// and are used to check state for various actions to occur
	static int jumpDownState = Animator.StringToHash("Base Layer.JumpDown");		// within our FixedUpdate() function below
	static int fallState = Animator.StringToHash("Base Layer.Fall");
	static int rollState = Animator.StringToHash("Base Layer.Roll");
	static int waveState = Animator.StringToHash("Layer2.Wave");
	static int attackState = Animator.StringToHash("Base Layer.Attack");
	static int hurtState = Animator.StringToHash("Base Layer.Hurt");
	static int monsterAttackState = Animator.StringToHash("Base Layer.MonsterAttack");

	GameManager manager;
	GameObject player;
	//BotControlScript playerScript;

	GameObject movingTarget;

	private float pushBack = 0;

	public float pushBackValue = 20;
	public float pushSpeed = 5;

	public bool angry = true;

	bool attack = false;


	void OnCollisionEnter(Collision collision) {

		if (hp > 0)
		{
			if (collision.gameObject == manager.Player)
			{

				Debug.Log ("hit player");
				anim.SetBool("MonsterAttack", true);
				attack = true;
			}
		}
		
	}

	public void Hurt(int value)
	{

		if (!angry)
		{
			transform.LookAt(player.transform.position);
			angry = true;
		}

		if (hp < 0) 
		{
			return;
		}

		hp -= value;

		anim.SetBool("Hurt", true);

		if (hp > 0) 
		{
			pushBack = pushBackValue;
		}
	}
	

	void Start ()
	{
		// initialising reference variables
		anim = gameObject.GetComponent<Animator>();					  
		col = gameObject.GetComponent<CapsuleCollider>();				
		if(anim.layerCount ==2)
			anim.SetLayerWeight(1, 1);

		manager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();

		player = manager.Player;

		movingTarget = player;

//		playerScript = player.GetComponent<BotControlScript> ();
	}

	void MoveToTarget()
	{
		if (!angry)return;
		//float h = Input.GetAxis("Horizontal");				// setup h variable as our horizontal input axis
		
		float MyRotate = transform.rotation.eulerAngles.y ;
		
		MyRotate %= 360;
		
		Vector3 difPosition = movingTarget.transform.position-transform.position;
		
		float rotate = -(180f*Mathf.Atan2(difPosition.z,difPosition.x)/Mathf.PI) +90;
		
		if (rotate < 0)
			rotate += 360;
		
		
		//Debug.Log ("MyRotate:"+MyRotate + " : "+ "Rotate:"+rotate);
		
		float rotateValue = (MyRotate - rotate);
		
		if (rotateValue > 180)
			rotateValue = -(360 - rotateValue) ;
		else if (rotateValue < -180)
			rotateValue = 360 + rotateValue;
		
		rotateValue = -rotateValue;
		
		rotateValue = rotateValue / 10f;
		
		//Debug.Log ("rotate Value:"+rotateValue);
		
		if (rotateValue > 1)
			rotateValue = 1;
		else if (rotateValue < -1)
			rotateValue = -1;

		float v = 0.2f;				// setup v variables as our vertical input axis
		
		anim.SetFloat("Speed", v*moveSpeed);							// set our animator's float parameter 'Speed' equal to the vertical input axis				
		anim.SetFloat("Direction", rotateValue); 						// set our animator's float parameter 'Direction' equal to the horizontal input axis		
		anim.speed = animSpeed;								// set the speed of our animator to the public variable 'animSpeed'


	}

	public bool killed = false;

	public void Dying()
	{
		anim.SetFloat("Speed", 0);							// set our animator's float parameter 'Speed' equal to the vertical input axis				
		anim.SetFloat("Direction", 0); 
		if (transform.rotation.eulerAngles.x > 270 || transform.rotation.eulerAngles.x <10) {
						transform.Rotate (-2, 0, 0);
			transform.Translate(0,-0.0005f,0);

			if(!killed)
			{
				collider.enabled=false;
				rigidbody.useGravity=false;
				anim.enabled = false;
			rigidbody.velocity = Vector3.zero;
				killed = true;
				manager.killed();
			}

				}



		vanishTimer -= Time.deltaTime;

		if (vanishTimer < 0) {
			GameObject.Destroy(gameObject);


				}
	}

	public float vanishTimer = 3;

	void FixedUpdate ()
	{

		if (hp > 0) 
		{
			MoveToTarget ();
		} 
		else 
		{
			Dying();
			return;
		}

		if (pushBack > 0) 
		{
			pushBack--;
			transform.position -= transform.forward * pushSpeed * Time.deltaTime;
		}



		currentBaseState = anim.GetCurrentAnimatorStateInfo(0);	// set our currentState variable to the current state of the Base Layer (0) of animation
		
		if(anim.layerCount ==2)		
			layer2CurrentState = anim.GetCurrentAnimatorStateInfo(1);	// set our layer2CurrentState variable to the current state of the second Layer (1) of animation
		
		/*
		bool attackPressed = Input.GetMouseButtonDown (0);
		
		if (attackPressed) 
		{
			anim.SetBool("Attack",true);
		}
		*/
		if (currentBaseState.nameHash == monsterAttackState)
		{
			if(!anim.IsInTransition(0))
			{				
				// reset the Jump bool so we can jump again, and so that the state does not loop 
				anim.SetBool("MonsterAttack", false);

				if(attack)
				{
					attack = false;
					Vector3 forward = transform.forward;
					//playerScript.Hurt(attackValue,forward);
				}
			}
		}


		if (hp > 0) {
						if (currentBaseState.nameHash == hurtState) {
						
								if (!anim.IsInTransition (0)) {				
										anim.SetBool ("Hurt", false);
								}
						}
				}
		
		
		// if we are in the jumping state... 
		if(currentBaseState.nameHash == jumpState)
		{
			//  ..and not still in transition..
			if(!anim.IsInTransition(0))
			{				
				// reset the Jump bool so we can jump again, and so that the state does not loop 
				anim.SetBool("Jump", false);
			}
			
			// Raycast down from the center of the character.. 
			Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
			RaycastHit hitInfo = new RaycastHit();
			
			if (Physics.Raycast(ray, out hitInfo))
			{
				// ..if distance to the ground is more than 1.75, use Match Target
				if (hitInfo.distance > 1.75f)
				{
					
					// MatchTarget allows us to take over animation and smoothly transition our character towards a location - the hit point from the ray.
					// Here we're telling the Root of the character to only be influenced on the Y axis (MatchTargetWeightMask) and only occur between 0.35 and 0.5
					// of the timeline of our animation clip
					anim.MatchTarget(hitInfo.point, Quaternion.identity, AvatarTarget.Root, new MatchTargetWeightMask(new Vector3(0, 1, 0), 0), 0.35f, 0.5f);
				}
			}
		}
		
		
		// JUMP DOWN AND ROLL 
		
		// if we are jumping down, set our Collider's Y position to the float curve from the animation clip - 
		// this is a slight lowering so that the collider hits the floor as the character extends his legs
		else if (currentBaseState.nameHash == jumpDownState)
		{
			col.center = new Vector3(0, anim.GetFloat("ColliderY"), 0);
		}
		
		// if we are falling, set our Grounded boolean to true when our character's root 
		// position is less that 0.6, this allows us to transition from fall into roll and run
		// we then set the Collider's Height equal to the float curve from the animation clip
		else if (currentBaseState.nameHash == fallState)
		{
			col.height = anim.GetFloat("ColliderHeight");
		}
		
		// if we are in the roll state and not in transition, set Collider Height to the float curve from the animation clip 
		// this ensures we are in a short spherical capsule height during the roll, so we can smash through the lower
		// boxes, and then extends the collider as we come out of the roll
		// we also moderate the Y position of the collider using another of these curves on line 128
		else if (currentBaseState.nameHash == rollState)
		{
			if(!anim.IsInTransition(0))
			{
				col.center = new Vector3(0, anim.GetFloat("ColliderY"), 0);
				
			}
		}
		// IDLE
		
		// check if we are at idle, if so, let us Wave!
		else if (currentBaseState.nameHash == idleState)
		{
			if(Input.GetButtonUp("Jump"))
			{
				anim.SetBool("Wave", true);
			}
		}
		// if we enter the waving state, reset the bool to let us wave again in future
		if(layer2CurrentState.nameHash == waveState)
		{
			anim.SetBool("Wave", false);
		}
	}
}

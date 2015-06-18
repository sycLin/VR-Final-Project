using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public float moveSpeed;
	public float rotateSpeed;

	public Vector2 joyStickInput = new Vector2(0,0);
	public Vector2 joyStickInputRight = new Vector2 (0, 0);

	public GameObject offset;
	public Camera camera;

	public WeaponManager weaponManager;
	public float touchPadMoveSpeed;

	public bool triggerTap = false;


	public float forwordRatio;
	public float backRatio;

	public float tiltData;
	public Vector3 tiltingDataV3;

	private float forwordPosition;
	
	private float backPosition;

	public MonsterSpwanNodes spwanNodes;
	public ZombieBehavior zombie;
	public CameraOffset cameraOffset;
	public GameObject MoveTarget;
	public float Distance;


	// Use this for initialization
	void Start () {

		backPosition = AndroidInput.secondaryTouchWidth *backRatio;
		forwordPosition = AndroidInput.secondaryTouchWidth *forwordRatio;
	}

	public bool UserControl
	{
		get
		{
			return userControl;
		}
		set
		{

			if(userControl!=value)
			{
				if(!value)
				{
					//OnTrackMoving
					spwanNodes.index = 1;
					transform.position = spwanNodes.MonsterSpwanNodeList[0].transform.position;
					zombie.autoAdd = false;
					zombie.Destroy();

					MoveTarget = spwanNodes.MonsterSpwanNodeList[1];
					cameraOffset.LookAt(MoveTarget,0.5f);
					lookAtMonster = true;
				}
				else
				{
					zombie.autoAdd = true;
				}
			}

			userControl = value;
		}
	}

	private bool userControl = true;

	// Update is called once per frame
	void Update () 
	{
		/*if (Input.GetKeyDown (KeyCode.O)) 
		{
			UserControl = !UserControl;		
		}*/


		if (userControl) 
		{
			handleMove();
			handleJoystick ();
		} 
		else 
		{
			Vector3 v = MoveTarget.transform.position - gameObject.transform.position;
			float dis = v.magnitude;
			v.Normalize();

			if(dis>Distance)
			{
				gameObject.rigidbody.velocity = v * OnTrackMoveSpeed;
			}
			else
			{
				if(zombie.HP<=0)
				{
					lookAtMonster = true;
					int index = spwanNodes.AddToNext();
					MoveTarget = spwanNodes.MonsterSpwanNodeList[index];
					cameraOffset.LookAt(MoveTarget,0.5f);
				}
				else
				{
					if(lookAtMonster)
					{
						lookAtMonster = false;
						cameraOffset.LookAt(zombie.gameObject,0.5f);
					}
				}
			}
		}
	

		handleJoystick2();

		//fire
		handleTouch ();


	}

	private bool lookAtMonster = false;

	public float OnTrackMoveSpeed;

	bool touched = false;
	bool longpressed = false;

	void handleMove()
	{
		if(touched)
		{
			if (touchCounter > 0) 
			{
				touchCounter-= Time.deltaTime;
			} 
			else 
			{
				if(triggerTap)
				{
					longpressed = true;
					triggerTap = false;
				}
			}

			if(longpressed)
			{
				if(lastTouch.position.x < backPosition)
				{
					joyStickInput.y = -1*touchPadMoveSpeed;
				}
				else if(lastTouch.position.x > backPosition)
				{
					joyStickInput.y = 1*touchPadMoveSpeed;
				}
				else
				{
					joyStickInput.y = 0;
				}
			}


		}
	}

	public float moveDelay;
	private float touchCounter = 0;

	private Touch lastTouch;

	void handleJoystick2()
	{
		float rotateValue = joyStickInputRight.x * rotateSpeed * Time.deltaTime;
		float rotateValue2 = -joyStickInputRight.y * rotateSpeed * Time.deltaTime;



		offset.transform.Rotate (new Vector3 (0, rotateValue, 0));


		currentOffestX += rotateValue2;

		if (currentOffestX < -90)
				currentOffestX = -90;
		if(currentOffestX>90)
				currentOffestX = 90;

		camera.transform.localRotation = Quaternion.Euler (currentOffestX,0,0);


	}

	float currentOffestX = 0;

	public void ClearCurrentOffestX()
	{
		currentOffestX = 0;
	}

	void handleJoystick()
	{
		Vector3 CurrentVec3 = camera.transform.forward;
		CurrentVec3.y = 0;
		CurrentVec3.Normalize();
		
		Vector3 RotateVec3 = camera.transform.right;
		RotateVec3.y = 0;
		RotateVec3.Normalize ();
		
		Vector3 v = (joyStickInput.y * CurrentVec3 + (joyStickInput.x) * RotateVec3);
		v += tiltingDataV3;

//		Debug.Log (v);

		gameObject.rigidbody.velocity = v * moveSpeed ;
	}



	void handleTouch(){

		if (AndroidInput.touchCountSecondary > 1)
						Debug.Log ("Multi-Touched?");

		if (AndroidInput.touchCountSecondary != 0) {



			for(int i = 0;i< AndroidInput.touchCountSecondary;i++)
			{

			Touch touch = AndroidInput.GetSecondaryTouch(i);
			
				//Debug.Log("touched");


		

			switch (touch.phase) { //following are 2 cases
				
			case TouchPhase.Began: //here begins the 1st case
				//shoot
				//shoot();	
				touchCounter = moveDelay;
				lastTouch = touch;
				triggerTap = true;
				touched=true;


				break; //here ends the 1st case
				
			case TouchPhase.Ended: //here begins the 2nd case

				if(triggerTap)shoot();

				triggerTap = false;
				touched = false;
				joyStickInput.y = 0;
				longpressed = false;

				break;

			case TouchPhase.Moved:

				Vector2 delta = touch.deltaPosition;

				if(delta.magnitude > 15)
				{
					triggerTap = false;
				}

				if(delta.y < -15 && delta.x < 5 && delta.x>-5)
				{
					Application.Quit();

				}
				else if(delta.y > 15 && delta.x < 5 && delta.x>-5)
				{
//					weaponManager.SwitchWeapon();
					
				}
				else
				{
					/*
					Vector3 v = delta.x * camera.transform.forward * touchPadMoveSpeed;
					v.y = 0;
					rigidbody.velocity = v;*/

					if(delta.x > 15 || delta.x <-15)
					{
						transform.transform.Rotate(new Vector3(0,delta.x*touchPadRotateSpeed,0));
					}
				}



				break;
			}
			}
		}
	}

	public float touchPadRotateSpeed;
	private Vector3 axis = new Vector3(0,1,0);


	public void shoot(){

		weaponManager.TriggerAttack ();
		//shoot
		//Instantiate(bullet,transform.position,transform.rotation);

	}
}

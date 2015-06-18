using UnityEngine;
using System.Collections;

public class BulletShooter : MonoBehaviour {

	public GameObject spark;

	public bool firing = false;
	public float shootDuration = 0.5f;
	public float flashDuration = 0.02f;
	public bool singleFire = true;

	public GameObject pushBackObject;
	public Vector3 originRotate;
	public Vector3 pushRotate;
	
	public float pushBackTime = 0.02f;
	public float backTime = 0.5f;

	public GameObject bullet;
	public float bulletSpeed;
	public GameObject aimPlane;

	private bool flashing = false;
	private float counter;

	Quaternion targetGyro;
	public float gyroLerpRatio;

	public GameObject gyroOffset;
	public GameObject cameraOffset;
	public DynamicCrossHairController DynamicCrossHairController;

	public enum AimType
	{
		viewportCenter,
		phoneGun,
	}

	public enum WeaponType
	{
		gun,
		//flashLight,
	}

	private WeaponType _weaponType = WeaponType.gun;
	public WeaponType weaponType
	{
		get
		{
			return _weaponType;
		}
		set
		{
			_weaponType = value;

		 if(_weaponType == WeaponType.gun)
			{
				gunObject.SetActive(true);
				//flashLightObject.SetActive(false);
			}
		}
	} 

	public GameObject gunObject;
	//public GameObject flashLightObject;


	public GameObject gunGyro;
	public Camera mainCamera;

	private AimType _aimType = AimType.viewportCenter;
	public AimType aimType 
	{
		get
		{
			return _aimType;
		}
		set
		{
			_aimType = value;

			switch(value)
			{
			case AimType.viewportCenter:
				gunGyro.transform.localRotation = Quaternion.Euler(new Vector3(0,270,90));
				gyroOffset.transform.localRotation = Quaternion.Euler(new Vector3(0,0,0));
				DynamicCrossHairController.Reset();
				aimPlane.SetActive(false);
				
				DynamicCrossHairController.enabled = false;
				break;
			case AimType.phoneGun:
				//crossHairControll.gameObject.SetActive(false);
				targetGyro = gunGyro.transform.localRotation;

				aimPlane.SetActive(true);
				DynamicCrossHairController.enabled = true;

				break;
			}
		}
	}

	public void setGunGyro(Quaternion q)
	{
		if (aimType != AimType.phoneGun)
						return;

	
		targetGyro = q;
	}



	public void updateGyro()
	{
		if (aimType != AimType.phoneGun)
			return;

		gunGyro.transform.localRotation = Quaternion.Slerp(gunGyro.transform.localRotation,targetGyro, gyroLerpRatio);

		Vector3 temp= gyroOffset.transform.localRotation.eulerAngles;
		Vector3 vec3 = mainCamera.transform.forward;
		double data = Mathf.Atan2(vec3.x,vec3.z);
		double result = data / Mathf.PI * 180;
		temp.z = (float)result - cameraOffset.transform.localRotation.eulerAngles.y;

		//Debug.Log ("camera offset:"+cameraOffset.transform.localRotation.eulerAngles.y);

		gyroOffset.transform.localRotation = Quaternion.Euler(temp);
		//gunGyro.transform.RotateAround (new Vector3 (0, 1, 0), -mainCamera.transform.localRotation.eulerAngles.y);
	}

	public crossHairAnimate crossHairControll;

	// Use this for initialization
	void Start () {


		counter = shootDuration;
	}

	public void TriggerShoot()
	{
		spark.transform.Rotate (new Vector3(0,0,90));
		spark.SetActive (true);
		flashing = true;
		singleFire = false;

		counter = 0;

		crossHairControll.shoot = true;
		InitBulletAndShoot();
	}
	
	// Update is called once per frame
	void Update () {

		if (counter < shootDuration) 
		{
			counter+=Time.deltaTime;
		}

		CheckShoot();
		UpdatePowerBack();
		UpdateFlash ();
		updateGyro ();
	}

	void InitBulletAndShoot()
	{
		GameObject nowBullet = Instantiate(bullet,transform.position,transform.rotation) as GameObject;
		nowBullet.rigidbody.velocity = transform.forward * bulletSpeed;
	}

	void UpdatePowerBack()
	{
		if (counter >= shootDuration) 
		{
			pushBackObject.transform.localRotation = Quaternion.Euler(originRotate);
		}
		else if(counter < pushBackTime)
		{
			Vector3 currentEuler = Vector3.Lerp(originRotate, pushRotate, counter/pushBackTime);
			pushBackObject.transform.localRotation = Quaternion.Euler(currentEuler);
		}
		else if(counter >= pushBackTime)
		{
			float ratio = (counter-pushBackTime) /(backTime-pushBackTime);
			Vector3 currentEuler = Vector3.Lerp(pushRotate, originRotate, ratio);
			pushBackObject.transform.localRotation = Quaternion.Euler(currentEuler);
		}
	}

	void CheckShoot()
	{
		if (firing||singleFire) 
		{
			if(weaponType != WeaponType.gun)return;

			if(counter>=shootDuration)
			{
				TriggerShoot();
			}
		} 
	}

	void UpdateFlash()
	{
		if (counter > flashDuration && flashing) 
		{
			flashing = false;
			spark.SetActive (false);
			
		}
	}
}

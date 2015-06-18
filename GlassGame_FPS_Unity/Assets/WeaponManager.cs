using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour {


	public WeaponType weaponType;
	public List<GameObject> weaponRoot;
	public int currentWeaponIndex = 0;

	//Weapon Script
	public BulletShooter bulletShooter;
	public GrenadeScript grenadeThrower;
	public DynamicCrossHairController CrossHairController;
	public KnifeController knifeController;


	public enum WeaponType
	{
		handGun,
		handGrenade,
		knife,
		count,
	}

	// Use this for initialization
	void Start () {
		//weaponRoot [currentWeaponIndex].SetActive (false);
		weaponRoot [currentWeaponIndex].SetActive (true);
		CheckWeaponState();
	}


	public void ChangeWeaponToPrevous()
	{

		//Debug.Log ("previous");
		weaponRoot [currentWeaponIndex].SetActive (false);
		
		currentWeaponIndex = (currentWeaponIndex - 1);

		if (currentWeaponIndex < 0)
						currentWeaponIndex += WeaponType.count;

		weaponType = (WeaponType)currentWeaponIndex;
		
		weaponRoot[currentWeaponIndex].SetActive(true);
		
		CheckWeaponState ();

	}

	public void ChangeWeaponToNext()
	{
		//Debug.Log ("next");
		weaponRoot [currentWeaponIndex].SetActive (false);
		
		currentWeaponIndex = (currentWeaponIndex + 1) % weaponRoot.Count;
		weaponType = (WeaponType)currentWeaponIndex;

		weaponRoot[currentWeaponIndex].SetActive(true);

		CheckWeaponState ();
	}

	public void CheckWeaponState()
	{
		if (weaponType == WeaponType.handGrenade)
		{
			CrossHairController.gameObject.SetActive(false);
		}
		else if(weaponType == WeaponType.handGun)
		{
			CrossHairController.gameObject.SetActive(true);
			CrossHairController.Reset();
			bulletShooter.weaponType = BulletShooter.WeaponType.gun;
		}
		else if(weaponType == WeaponType.handGun)
		{
			CrossHairController.gameObject.SetActive(false);
		}
	}

	public void TriggerAttack()
	{
		switch (weaponType) 
		{
		case WeaponType.handGun:
			bulletShooter.singleFire = true;
			break;
		case WeaponType.handGrenade:
			grenadeThrower.throwGrenade();
			break;

		case WeaponType.knife:
			knifeController.Attack();
			break;
			
		}
	}

}

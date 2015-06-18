using UnityEngine;
using System.Collections;
using TouchScript;
using System.Collections.Generic;
using TouchScript.Gestures;
using Holoville.HOTween;
public class GGestureManager : MonoBehaviour {


	public GameObject TouchPanel;
	//public FlickGesture flickGesture;
	public CommunicationManager communicationManager;
	public TapGesture RightArrow;
	public TapGesture LeftArrow;

	public GameObject weaponBase;
	public List<GameObject> allWeaponIcon;
	public bool WeaponChanging = false;
	public FlickGesture flickerGesture;

	public float ShiftDistance;
	public float AnimationTime;
	public float ScaleRatio;

	public GameObject Wheel;
	public List<GameObject> joySticks;


	public EaseType easeType;

	public AimTypeController aimTypeController;

	public List<Vector3> WeaponOffsets;
	public List<Vector3> WeaponRotates;

	public enum WeaponType
	{
		gun,
		grenade,
		knife,
		Count,
		flashLight,
		car,
	}

	private WeaponType _weapon = WeaponType.gun;
	public WeaponType Weapon 
	{
		get
		{
			return _weapon;
		}
	}

	// Use this for initialization
	void Start () {
		//IGestureManager

		HOTween.Init (true,true,true);

		for (int i =0;i<allWeaponIcon.Count;i++)
		{
			GameObject o = allWeaponIcon[i];

			if(i == (int)_weapon)continue;
			if(i == (int)WeaponType.Count)break;

			o.transform.localScale = o.transform.localScale / ScaleRatio;
		}

		TouchManager.Instance.TouchesBegan += touchBegin;

		TapGesture[] taps = TouchPanel.GetComponents<TapGesture> ();

		foreach (TapGesture g in taps) 
		{
			if(g.NumberOfTapsRequired == 1)
			{
				g.Tapped += (object sender, System.EventArgs e) => 
				{
					singleTap();
				};
			}
			else if(g.NumberOfTapsRequired == 2)
			{
				g.Tapped += (object sender, System.EventArgs e) => 
				{
					doubleTap();
				};
			}
		}




				flickerGesture.Flicked += (object sender, System.EventArgs e) => 
				{
					
					float absX = Mathf.Abs(flickerGesture.ScreenFlickVector.x);
					float absY = Mathf.Abs(flickerGesture.ScreenFlickVector.y);

					//flickGesture.scree

					if(absX>absY)
					{
							if(flickerGesture.ScreenFlickVector.x>0)
							{
								changeToPreviousWeapon();
							}
							else if(flickerGesture.ScreenFlickVector.x<0)
							{
								changeToNextWeapon();
							}
					}
					else
					{

							if(flickerGesture.ScreenFlickVector.y<0)
							{
								if(aimTypeController.aimType == AimTypeController.AimType.phoneGun)
								{
									/*aimTypeController.changeToWheelMode();
									Wheel.SetActive(true);
									allWeaponIcon[(int)_weapon].SetActive(false);

									foreach(GameObject o in joySticks)
									{
										o.SetActive(true);
									}*/
									
								}
								else if(aimTypeController.aimType == AimTypeController.AimType.viewportCenter)
								{
									aimTypeController.changeToGunMode();
								}
							}
							else if(flickerGesture.ScreenFlickVector.y>0)
							{
								if(aimTypeController.aimType == AimTypeController.AimType.wheel)
								{
									aimTypeController.changeToGunMode();
									Wheel.SetActive(false);
									allWeaponIcon[(int)_weapon].SetActive(true);

									foreach(GameObject o in joySticks)
									{
										o.SetActive(false);
									}
								}
								else if(aimTypeController.aimType == AimTypeController.AimType.phoneGun)
								{
									aimTypeController.changeToViewPortMode();
								}
							}
					}
				};

				



		RightArrow.Tapped += (object sender, System.EventArgs e) => 
		{
			changeToNextWeapon();
		};

		LeftArrow.Tapped += (object sender, System.EventArgs e) => 
		{
			changeToPreviousWeapon();
		};

		aimTypeController.AimTypeChangeEvent += AimModeChanging;
	}

	private void changeToNextWeapon()
	{
		if (WeaponChanging)
						return;

		if (aimTypeController.aimType == AimTypeController.AimType.phoneGun)
						return;

		Debug.Log ("changeWeapon : next");

		//no next;
		if (((int)_weapon) == ((int)WeaponType.Count-1))return;

		WeaponChanging = true;

		HOTween.To(weaponBase.transform,AnimationTime,new TweenParms()
		           .Prop("position",new Vector3(-ShiftDistance,0,0),true)
		           .Ease(easeType).OnStepComplete(weaponChangingDone));

		HOTween.To(allWeaponIcon[(int)_weapon].transform,AnimationTime,new TweenParms()
		           .Prop("localScale",allWeaponIcon[(int)_weapon].transform.localScale/ScaleRatio));



		_weapon++;

		HOTween.To(allWeaponIcon[(int)_weapon].transform,AnimationTime,new TweenParms()
		           .Prop("localScale",allWeaponIcon[(int)_weapon].transform.localScale*ScaleRatio));

		CheckWeaponState();

		JSONObject commandJsonObject = new JSONObject();
		commandJsonObject.AddField("command","changeWeapon");
		commandJsonObject.AddField("direction","next");
		
		communicationManager.SendJson (commandJsonObject);
	}

	private void changeToPreviousWeapon()
	{
		if (WeaponChanging)
						return;

		if (aimTypeController.aimType == AimTypeController.AimType.phoneGun)
			return;

		Debug.Log ("changeWeapon : previous");

		//no previous;
		if (((int)_weapon) == 0)return;

		WeaponChanging = true;

		HOTween.To(weaponBase.transform,AnimationTime,new TweenParms()
		           .Prop("position",new Vector3(ShiftDistance,0,0),true)
		           .Ease(easeType).OnStepComplete(weaponChangingDone));



		HOTween.To(allWeaponIcon[(int)_weapon].transform,AnimationTime,new TweenParms()
		           .Prop("localScale",allWeaponIcon[(int)_weapon].transform.localScale/ScaleRatio));


		_weapon--;

		HOTween.To(allWeaponIcon[(int)_weapon].transform,AnimationTime,new TweenParms()
		           .Prop("localScale",allWeaponIcon[(int)_weapon].transform.localScale*ScaleRatio));


		CheckWeaponState();

		JSONObject commandJsonObject = new JSONObject();
		commandJsonObject.AddField("command","changeWeapon");
		commandJsonObject.AddField("direction","previous");
		
		communicationManager.SendJson (commandJsonObject);
	}

	public void CheckWeaponState()
	{
				if (((int)_weapon) == 0) 
				{
						LeftArrow.gameObject.SetActive (false);
						RightArrow.gameObject.SetActive (true);
				} 
				else if (((int)_weapon) == ((int)WeaponType.Count - 1)) 
				{
						RightArrow.gameObject.SetActive (false);
						LeftArrow.gameObject.SetActive (true);
				}
				else 
				{
					RightArrow.gameObject.SetActive (true);
					LeftArrow.gameObject.SetActive (true);	
				}

	}
	
	public void singleTap()
	{
		if (WeaponChanging)
						return;

		JSONObject commandJsonObject = new JSONObject();
		commandJsonObject.AddField("command","singleTap");
		
		communicationManager.SendJson (commandJsonObject);
	}
	
	public void doubleTap()
	{
		if (WeaponChanging)
			return;

		JSONObject commandJsonObject = new JSONObject();
		commandJsonObject.AddField("command","doubleTap");
		
		
		communicationManager.SendJson (commandJsonObject);
	}

	public void AimModeChanging(AimTypeController.AimType previous,AimTypeController.AimType type)
	{
		if (type == AimTypeController.AimType.phoneGun&&previous== AimTypeController.AimType.viewportCenter) 
		{
			WeaponChanging = true;
			
			HOTween.To(TouchPanel.transform,AnimationTime,new TweenParms()
			           .Prop("localPosition",new Vector3(0,-WeaponPanelOffset,0),true).OnStepComplete(weaponChangingDone));

			HOTween.To(allWeaponIcon[(int)_weapon].transform,AnimationTime,new TweenParms()
			           .Prop("localScale",allWeaponIcon[(int)_weapon].transform.localScale*ScaleUp));

			HOTween.To(allWeaponIcon[(int)_weapon].transform,AnimationTime,new TweenParms()
			           .Prop("localPosition",WeaponOffsets[(int)_weapon],true));

			HOTween.To(allWeaponIcon[(int)_weapon].transform,AnimationTime,new TweenParms()
			           .Prop("rotation",WeaponRotates[(int)_weapon],true));


			for (int i =0;i<allWeaponIcon.Count;i++)
			{
				GameObject o = allWeaponIcon[i];
				
				if(i == (int)_weapon)continue;
				if(i == (int)WeaponType.Count)break;
				
				o.SetActive(false);
			}

			RightArrow.gameObject.SetActive(false);
			LeftArrow.gameObject.SetActive(false);
			//CheckWeaponState();
		}
		else if (type == AimTypeController.AimType.viewportCenter&& previous == AimTypeController.AimType.phoneGun)
		{
			WeaponChanging = true;
			
			HOTween.To(TouchPanel.transform,AnimationTime,new TweenParms()
			           .Prop("localPosition",new Vector3(0,WeaponPanelOffset,0),true).OnStepComplete(weaponChangingDone));

			
			HOTween.To(allWeaponIcon[(int)_weapon].transform,AnimationTime,new TweenParms()
			           .Prop("localScale",allWeaponIcon[(int)_weapon].transform.localScale/ScaleUp));

			HOTween.To(allWeaponIcon[(int)_weapon].transform,AnimationTime,new TweenParms()
			           .Prop("localPosition",-WeaponOffsets[(int)_weapon],true));


			HOTween.To(allWeaponIcon[(int)_weapon].transform,AnimationTime,new TweenParms()
			           .Prop("rotation",-WeaponRotates[(int)_weapon],true));

			for (int i =0;i<allWeaponIcon.Count;i++)
			{
				GameObject o = allWeaponIcon[i];
				
				//if(i == (int)_weapon)continue;
				if(i == (int)WeaponType.Count)break;
				
				o.SetActive(true);
			}

			
			CheckWeaponState();
		}
	}

	public int WeaponPanelOffset = 7;
	public float ScaleUp = 3;

	public void weaponChangingDone()
	{
		WeaponChanging = false;
	}

	private void touchBegin(object sender, TouchEventArgs e)
	{
		//Debug.Log(e.Touches[0].Position);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

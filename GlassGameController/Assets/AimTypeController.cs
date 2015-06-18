using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AimTypeController : MonoBehaviour {

	public CommunicationManager communicationManager;
	public GyroManager gyroManager;

	public bool GyroListening;
	public bool GUIShowing;
	public float GunYValue = -0.8f;
	public float ViewPortZValue = -0.8f;

	public List<GameObject> ViewPortCenterEnables = new List<GameObject>();
	public List<GameObject> GunAimEnables = new List<GameObject>();

	public delegate void AimTypeChangeEventHandler(AimType previous,AimType type);
	public event AimTypeChangeEventHandler AimTypeChangeEvent;




	public enum AimType
	{
		viewportCenter,
		phoneGun,
		wheel,
	}

	public AimType _aimType = AimType.viewportCenter;
	public AimType aimType
	{
		get
		{
			return _aimType;
		}
	}

	//public voi

	public GGestureManager gestureManager;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(communicationManager.Playable)
		{
			if(!gestureManager.WeaponChanging)
			{
				if (GyroListening) 
				{
					if(_aimType == AimType.viewportCenter)
					{

						if(gyroManager.lastAcc.y < GunYValue)
						{
							changeToGunMode();
						}
					}
					else if(_aimType == AimType.phoneGun)
					{
						if(gyroManager.lastAcc.z < ViewPortZValue)
						{
							changeToViewPortMode();
						}
					}
				}
			}

			if(forceChangeMode)
			{
				ForceChangeMode();
				forceChangeMode = false;
			}
		}

	}

	public void changeToGunMode()
	{
		if (_aimType == AimType.phoneGun)
						return;

		AimType previous = _aimType;

		gyroManager.Transfering = true;
		
		_aimType = AimType.phoneGun;

		if (AimTypeChangeEvent != null)
		{
			AimTypeChangeEvent(previous,_aimType);
		}

		JSONObject json = new JSONObject();
		json.AddField("command","changeAimMode");
		json.AddField("mode","phoneGun");

		SetEnableList (GunAimEnables,true);
		SetEnableList (ViewPortCenterEnables, false);

		communicationManager.SendJson(json);
	}

	public void changeToViewPortMode()
	{
		if (_aimType == AimType.viewportCenter)
			return;

		gyroManager.Transfering = false;

		AimType previous = _aimType;
		
		_aimType = AimType.viewportCenter;

		if (AimTypeChangeEvent != null)
		{
			AimTypeChangeEvent(previous,_aimType);
		}
		
		JSONObject json = new JSONObject();
		json.AddField("command","changeAimMode");
		json.AddField("mode","viewportCenter");

		SetEnableList (GunAimEnables,false);
		SetEnableList (ViewPortCenterEnables, true);
		
		communicationManager.SendJson(json);
	}

	public void changeToWheelMode()
	{
		if (_aimType == AimType.wheel || _aimType == AimType.viewportCenter)
			return;
		
		gyroManager.Transfering = true;

		AimType previous = _aimType;

		_aimType = AimType.wheel;
		
		if (AimTypeChangeEvent != null)
		{
			AimTypeChangeEvent(previous,_aimType);
		}
		
		JSONObject json = new JSONObject();
		json.AddField("command","changeAimMode");
		json.AddField("mode","phoneGun");
		
		SetEnableList (GunAimEnables,false);
		SetEnableList (ViewPortCenterEnables, true);
		
		communicationManager.SendJson(json);
	}

	public void SetEnableList(List<GameObject> list,bool active){

		foreach (GameObject o in list) 
		{
			o.SetActive(active);
		}
	}

	void OnGUI()
	{
		if(communicationManager.Playable)
		{
			if(GUIShowing)
			{
				if (_aimType == AimType.viewportCenter) 
				{
					if (GUI.Button(new Rect(Screen.width-100, 0, 100, 100), "change to gun"))
					{
						changeToGunMode();
					}
						
				} 
				else if (_aimType == AimType.phoneGun) 
				{
					if (GUI.Button(new Rect(Screen.width-100, 0, 100, 100), "change to viewport"))
					{

						changeToViewPortMode();
					}	
				}
			}

		}
	}

	public bool forceChangeMode = false;

	public void ForceChangeMode()
	{

			if (_aimType == AimType.viewportCenter) 
			{
				//if (GUI.Button(new Rect(Screen.width-100, 0, 100, 100), "change to gun"))
				{
					changeToGunMode();
				}
				
			} 
			else if (_aimType == AimType.phoneGun) 
			{
				//if (GUI.Button(new Rect(Screen.width-100, 0, 100, 100), "change to viewport"))
				{
					
					changeToViewPortMode();
				}	
			}

	}
}

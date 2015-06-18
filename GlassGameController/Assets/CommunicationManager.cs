using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AOT;

public class CommunicationManager : MonoBehaviour {

	public NetworkManager networkManager;
	public GameObject bluetoothManager;
	private bool playable = false;
	public bool bluetoothSupport;

	public bool Playable
	{
		get
		{
			return playable;
		}
	}

    void OnGUI()
	{
		if (!playable) {
						if (GUI.Button (new Rect (100, 100, Screen.width / 3, 100), "Bluetooth")) {
								bluetoothSupport = true;
								bluetoothManager.SetActive (true);
						} else if (GUI.Button (new Rect (100, 200, Screen.width / 3, 100), "Wifi")) {
								networkManager.gameObject.SetActive (true);
						}
				}
	}


	public enum CommunicateTheme
	{
		none,
		network,
		bluetooth
	}

	private CommunicateTheme communicateTheme = CommunicateTheme.none;

	public List<GameObject> playableEnableObjects;
	public List<GameObject> playableDisableObjects;

	public void SendJson(JSONObject json)
	{
		if (!playable) {
			//Debug.Log("Send JSON failed");
			return;
		}

		switch (communicateTheme)
		{
		case CommunicateTheme.bluetooth:
			Bluetooth.Instance().Send(json.ToString().Replace("\n",""));
			break;
		case CommunicateTheme.network:
			networkManager.Send(json.ToString().Replace("\n",""));

			break;
		}
	}


	
	// Update is called once per frame
	void Update () {
		if (!playable) 
		{
			if(isBlueToothConnected)
			{
				EnablePlayble();
				communicateTheme = CommunicateTheme.bluetooth;
			}
			if(networkManager.isConnected)
			{
				EnablePlayble();
				communicateTheme = CommunicateTheme.network;
			}
		}
	}

	private void EnablePlayble()
	{
		playable = true;
		foreach(GameObject obj in playableEnableObjects)
		{
			obj.SetActive(true);
		}
	}

	public bool isBlueToothConnected
	{
		get
		{
			if(!bluetoothSupport)return false;

			return Bluetooth.Instance().IsConnected();
		}
	}
}

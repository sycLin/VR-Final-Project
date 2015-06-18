using UnityEngine;
using System.Collections;

public class BluetoothManager : MonoBehaviour {

	private bool isEnabled = false;
	private bool isConnected = false;
//	private string MACaddress = "BC:F5:AC:75:18:AD";
	private string result = "Fuck you\n";
	private string Result = "";
	private int count = 0;
	// Use this for initialization
	void Start () {
		Debug.Log ("Discover");

		isEnabled = Bluetooth.Instance().IsEnabled();
		if(!isEnabled){
			Bluetooth.Instance().EnableBluetooth();
		}

		Bluetooth.Instance().Discoverable();

		Invoke("Discover",60);
		Debug.Log("" + Bluetooth.Instance().DeviceName());
	}

	public void Discover()
	{
		
		Bluetooth.Instance().Discoverable();

		Debug.Log("bluetooth discover!");

		if (!isConnected) 
		{
			Invoke("Discover",60);
		}
	}
	
	// Update is called once per frame
	void Update () {
		isEnabled = Bluetooth.Instance().IsEnabled();
		isConnected = Bluetooth.Instance().IsConnected();
	}

	/*void OnGUI(){
		if(!isConnected){
			GUI.enabled = true;
			if (GUI.Button(new Rect(0, 0, Screen.width / 3, Screen.height / 10), "Send"))
			{
				Result = Bluetooth.Instance().Send("Fuck you, Nexus");
			}
			//Search Button
			if (GUI.Button(new Rect(0, (Screen.height / 10), Screen.width / 3, Screen.height / 10), "Search Device"))
			{
				Result = Bluetooth.Instance().SearchDevice();
			}
			//Discoverable Button
			if (GUI.Button(new Rect(0, (Screen.height / 10) * 2, Screen.width / 3, Screen.height / 10), "Discoverable"))
			{
				Result = Bluetooth.Instance().Discoverable();
			}
			//Enable Bluetooth Button
			if (GUI.Button(new Rect(0, (Screen.height / 10) * 3, Screen.width / 3, Screen.height / 10), "Enable"))
			{
				Result = Bluetooth.Instance().EnableBluetooth();
			}
			//Disable Bluetooth Button
			if (GUI.Button(new Rect(0, (Screen.height / 10) * 4, Screen.width / 3, Screen.height / 10), "Disable"))
			{
				Result = Bluetooth.Instance().DisableBluetooth();
			}
			//Get Device Connected Name Button
			if (GUI.Button(new Rect(0, (Screen.height / 10) * 5, Screen.width / 3, Screen.height / 10), "DeviceConnectedName"))
			{
				Result = Bluetooth.Instance().GetDeviceConnectedName();
			}
			//Get Current Device Name Button
			if (GUI.Button(new Rect(0, (Screen.height / 10) * 6, Screen.width / 3, Screen.height / 10), "Get Device Name"))
			{
				Result = Bluetooth.Instance().DeviceName();
			}
			//Is Bluetooth Connecte Button
			if (GUI.Button(new Rect(0, (Screen.height / 10) * 7, Screen.width / 3, Screen.height / 10), "Is Connected"))
			{
				Result = Bluetooth.Instance().IsConnected().ToString();
			}
			//Is Bluetooth Enabled Button
			if (GUI.Button(new Rect(0, (Screen.height / 10) * 8, Screen.width / 3, Screen.height / 10), "Is Enabled"))
			{
				Result = Bluetooth.Instance().IsEnabled().ToString();
			}
			//Devices the Bluetooth found, so you can connect if you want
			for (int i = 0; i < Bluetooth.Instance().MacAddresses.Count; i++)
			{
				if (GUI.Button(new Rect(Screen.width / 2, 0 + (i * (Screen.height / 8)), Screen.width / 3, Screen.height / 8), Bluetooth.Instance().MacAddresses[i]))
				{
					Bluetooth.Instance().Connect(Bluetooth.Instance().MacAddresses[i]);
				}
			}
		}else{
			GUI.enabled = false;
		}


	}*/
}

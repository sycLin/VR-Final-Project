using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BluetoothGUI : MonoBehaviour
{
    public static string Result = "";//To show the plugin result
    void OnGUI()
    {
        GUI.enabled = false;
        GUI.TextField(new Rect(0, (Screen.height / 10) * 9, Screen.width, Screen.height / 10), Result);
        GUI.enabled = true;
        //Send Button
        if (GUI.Button(new Rect(0, 0, Screen.width / 3, Screen.height / 10), "Send"))
        {
            Result = Bluetooth.Instance().Send("Your Message");
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
    }
}
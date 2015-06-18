using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bluetooth
{
    public List<string> MacAddresses = new List<string>();//List for the Bluetooth Devices 
    private static Bluetooth Instance_obj;//Bluetooth Singleton Object to make this class accessible
    private AndroidJavaClass _plugin;//AndroidJavaClass Object
    private AndroidJavaObject _activityObject;//AndroidJavaObject Object
    public static Bluetooth Instance()//Constractor
    {
        if (Instance_obj == null)
        {
            Instance_obj = new Bluetooth();
            Instance_obj.PluginStart();
        }
        return Instance_obj;
    }
    private void PluginStart()//Start to initialize the plugin
    {
        _plugin = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        _activityObject = _plugin.GetStatic<AndroidJavaObject>("currentActivity");
        _activityObject.Call("StartFun");
    }
    public string Send(string message)//Send specific message to the connected device
    {
        return _activityObject.Call<string>("sendMessage", message);
    }
    public string SearchDevice()//Search Device Function to search for other devices
    {
       MacAddresses.Clear();
       return _activityObject.Call<string>("ScanDevice");
       
    }
    public string GetDeviceConnectedName()//Get Device Connected Name Function to retrieve the name of the connected device
    {
       return _activityObject.Call<string>("GetDeviceConnectedName");

    }
    public string Discoverable()//To make sure the current Bluetooth is discoverable
    {
        return _activityObject.Call<string>("ensureDiscoverable");
    }
    public void Connect(string Address)//To connect to another device
    {
        _activityObject.Call("Connect", Address);
    }
    public string EnableBluetooth()//To enable the Bluetooth if it's available
    {
        return _activityObject.Call<string>("BluetoothEnable");
    }
    public string DisableBluetooth()//To disable the Bluetooth if it's enabled
    {
        return _activityObject.Call<string>("DisableBluetooth");
    }
    public string DeviceName()//Get current Bluetooth device name
    {
        return _activityObject.Call<string>("DeviceName");
    }
    public bool IsEnabled()//Is the Bluetooth enabled
    {
        return _activityObject.Call<bool>("IsEnabled");
    }
    public bool IsConnected()//Is the current Bluetooth device connected
    {
        return _activityObject.Call<bool>("IsConnected");
    }
}
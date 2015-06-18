using UnityEngine;
using System.Collections;

public class BluetoothEventListener : MonoBehaviour {

    void ConnectionStateEvent(string state)
    {
        //Connection State event this is the result of the connection fire after you try to Connect
        switch (state)
        {
            case "STATE_CONNECTED":

                break;
            case "STATE_CONNECTING":

                break;
            case "UNABLE TO CONNECT":

                break;
        }
        Debug.Log(state);
        BluetoothGUI.Result = state;
    }
    void DoneSendingEvent(string writeMessage)
    {
        //Done sending the message event
        Debug.Log("writeMessage " + writeMessage);
        BluetoothGUI.Result = writeMessage;
    }
    void DoneReadingEvent(string readMessage)
    {
        //Done reading the message event
        Debug.Log("readMessage " + readMessage);
        BluetoothGUI.Result = readMessage;
    }
    void FoundZeroDeviceEvent()
    {
        //Event for the end of the search devices and there is zero device
        Debug.Log("FoundZeroDeviceEvent");
        BluetoothGUI.Result = "FoundZeroDeviceEvent";
    }
    void ScanFinishEvent()
    {
        //Event for the end of the current search
        Debug.Log("ScanFinishEvent");
        BluetoothGUI.Result = "ScanFinishEvent";
    }
    void FoundDeviceEvent(string Device)
    {
        //Event when the search find new device
         Debug.Log("FoundDeviceEvent");
         BluetoothGUI.Result = "FoundDeviceEvent";
         Bluetooth.Instance().MacAddresses.Add(Device);
    }
}
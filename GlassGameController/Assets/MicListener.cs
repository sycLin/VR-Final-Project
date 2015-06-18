using UnityEngine;
using System.Collections;

public class MicListener : MonoBehaviour {

	private string device;
	public float threashHold;
	public float cooldown;

	private bool _block = false;

	public CommunicationManager communicationManager;

	public void unblock()
	{
		_block = false;
	}

	void Start(){
		if (device == null) device = Microphone.devices[0];
		audio.clip = Microphone.Start(device, true, 999, 44100);
		while (!(Microphone.GetPosition(device) > 0))
		{} 

		audio.Play();
	}
	void Update(){

		if (_block)return;

		int dec = 128;
		float[] waveData = new float[dec];
		int micPosition = Microphone.GetPosition(null)-(dec+1); // null means the first microphone
		audio.clip.GetData(waveData, micPosition);
		
		// Getting a peak on the last 128 samples
		float levelMax = 0;
		for (int i = 0; i < dec; i++) {
			float wavePeak = waveData[i] * waveData[i];
			if (levelMax < wavePeak) {
				levelMax = wavePeak;
			}
		}
		// levelMax equals to the highest normalized value power 2, a small number because < 1
		// use it like:
		float volume = Mathf.Sqrt(levelMax);
		if (volume > threashHold) 
		{
			_block = true;
			Invoke("unblock",cooldown);
			Debug.Log ("volume:" + volume);
			JSONObject commandJsonObject = new JSONObject();
			commandJsonObject.AddField("command","microphone");
			commandJsonObject.AddField("value",volume);
			communicationManager.SendJson (commandJsonObject);
		}
	}

}

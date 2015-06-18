using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;

public class CheckBox : MonoBehaviour {
	
	public Texture EnableTexture;
	public Texture DisableTexture;
	public Texture LockTexture;

	public TapGesture tap;
	public bool Locked;
	public bool Enabled;

	public delegate void EnableChangeHandler(bool enable);
	public EnableChangeHandler EnableChangeEvent;

	// Use this for initialization
	void Start () 
	{
		if (Locked) 
		{
			renderer.material.SetTexture (0, LockTexture);

		} 
		else 
		{
			checkTexture();
			tap = gameObject.GetComponent<TapGesture>();
			tap.Tapped += (object sender, System.EventArgs e) => 
			{
				Enabled = !Enabled;
				checkTexture();
				if(EnableChangeEvent!=null)EnableChangeEvent(Enabled);

				//Debug.Log("Option "+name+" : "+Enabled );
			};
		}
	}

	private void checkTexture()
	{
		if(Enabled)
		{
			renderer.material.SetTexture (0, EnableTexture);
		}
		else
		{
			renderer.material.SetTexture (0, DisableTexture);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

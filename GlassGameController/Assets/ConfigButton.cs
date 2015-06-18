using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;

public class ConfigButton : MonoBehaviour {
	

	public List<GameObject> DisableList;
	public List<GameObject> EnableList;

	private List<bool> originState = new List<bool>();

	private bool enabled = false;

	private TapGesture tap;


	// Use this for initialization
	void Start () {
		tap = gameObject.GetComponent<TapGesture>();

		tap.Tapped += (object sender, System.EventArgs e) => 
		{
			enabled = !enabled;

			if(enabled)
			{
				originState.Clear();

				foreach(GameObject g in DisableList)
				{
					originState.Add(g.activeSelf);
					g.SetActive(false);
				}

				foreach(GameObject g in EnableList)
				{
					g.SetActive(true);
				}
			}
			else
			{
				for(int i = 0 ; i<originState.Count ;i++)
				{
					DisableList[i].SetActive(originState[i]);
				}

				foreach(GameObject g in EnableList)
				{
					g.SetActive(false);
				}
			}

		};
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

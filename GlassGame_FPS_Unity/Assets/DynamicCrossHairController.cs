using UnityEngine;
using System.Collections;

public class DynamicCrossHairController : MonoBehaviour {


	public GameObject smallGun;
	public Camera camera;

	int plnaeMask;
	// Use this for initialization
	void Start () {

		plnaeMask = LayerMask.NameToLayer("Enemy");

	}

	public void Reset()
	{
		Vector3 vec3 = gameObject.transform.localPosition;

		vec3.x = 0;
		vec3.y = 0;

		gameObject.transform.localPosition = vec3;
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hit;

		if(Physics.Raycast(smallGun.transform.position, smallGun.transform.forward, out hit)){


			Vector3 CrossHairScreenPosition = camera.WorldToScreenPoint(hit.point);

			CrossHairScreenPosition.x -= Screen.width/2;
			CrossHairScreenPosition.y -= Screen.height/2;

			CrossHairScreenPosition.x *= transform.localScale.x;
			CrossHairScreenPosition.y *= transform.localScale.y;
			CrossHairScreenPosition.z = transform.localPosition.z;

			transform.localPosition = CrossHairScreenPosition;
		}

	}
}

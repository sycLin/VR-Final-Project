using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterSpwanNodes : MonoBehaviour {

	public int index = 0;
	public List<GameObject> MonsterSpwanNodeList;

	public int AddToNext()
	{
		index ++;
		index = index % MonsterSpwanNodeList.Count;

		return index;
	}





}

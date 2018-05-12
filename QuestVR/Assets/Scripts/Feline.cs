using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feline : MonoBehaviour {

	public Dragon dragon;
	
	void OnDestroy()
	{
		if (transform.parent.childCount <= 1)
			dragon.SetState(DragonState.Dead);
		else
			dragon.SetState(DragonState.Weak);
	}

//	void OnDisable()
//	{
//		if (transform.parent.childCount == 1)
//			dragon.Die ();
//		else
//			dragon.Idle02 ();
//	}
}

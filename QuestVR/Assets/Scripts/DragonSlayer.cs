using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonSlayer : MonoBehaviour {


	void OnTriggerEnter(Collider other)
	{
		Dragon dragon = other.GetComponent<Dragon> ();
		if (dragon != null && dragon.state == DragonState.Idle)
			dragon.Scream ();
	}
}

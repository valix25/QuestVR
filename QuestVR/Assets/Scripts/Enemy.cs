using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public void Die() {
		// possibly play some effect, animation here
		Destroy (gameObject);
	}

	void OnCollisionEnter(Collision col){
		if (col.gameObject.tag == "Fireball") {
			Die ();
			Destroy (col.gameObject);
		}
	}
}

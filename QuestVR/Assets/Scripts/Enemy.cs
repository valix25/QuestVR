using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public GameObject explosion;

	public void Die() {
		// possibly play some effect, animation here
		Destroy (gameObject);
		playExplosionEffect();
	}

	void OnCollisionEnter(Collision col){
		if (col.gameObject.tag == "Laser") {
			Die ();
			Destroy (col.gameObject);
		}
		if (col.gameObject.tag == "Fireball") {
			Die ();
		}
		if (col.gameObject.tag == "Player") {
			Die ();
		}
	}

	void playExplosionEffect() {
		GameObject explosionObj = Instantiate(explosion, transform.position, transform.rotation);
		Destroy (explosionObj, 0.5f);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleDragon : MonoBehaviour {

	public GameObject dragon;
	public float resetDragonHit = 1.5f;

	private bool dragonCanHit = true;
	private float dragonHitTimer = 0.0f;
	
	// Update is called once per frame
	void Update () {
		if (dragonCanHit == false) {
			dragonHitTimer += Time.deltaTime;
			if (dragonHitTimer % 60 > resetDragonHit) {
				dragonCanHit = true;
				dragonHitTimer = 0.0f;
			}
		}
	}

	void OnTriggerEnter(Collider col){
		if (col.gameObject.tag == "Dragon") {
			dragon.GetComponent<Dragon> ().startScream ();
		}
		if (col.gameObject.tag == "DragonHead") {
			if (dragonCanHit == true) {
				gameObject.GetComponent<Damage> ().lives -= 1;
				dragonCanHit = false;
			}
			Debug.Log ("Dragon head hit: lives - 1");
		}
	}
}

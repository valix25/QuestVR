using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonSaver : MonoBehaviour {

//	private LineRenderer line;

	void Start () {
//		line = gameObject.AddComponent<LineRenderer>(); 
//		line.SetColors(Color.red, Color.yellow);
//		line.SetWidth(0.2f, 0.2f); 
	}
	
	void FixedUpdate () {
		if (Input.GetKey(KeyCode.E)) {
			RaycastHit hit;
			if (Physics.Raycast (transform.position, transform.forward, out hit)) {

				print ("Hit: " + hit.collider.name);
				Debug.DrawLine(transform.position, hit.point, Color.red);

//				line.SetPosition(0, transform.position);
//				line.SetPosition(1, hit.point);

				SwordOnDragon sword = hit.collider.GetComponent<SwordOnDragon> ();
				if (sword) {
					sword.TakeSword ();
				}
			}
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandFireball : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		if (transform.localScale.x < 5 && transform.localScale.y < 5 && transform.localScale.z < 5) {
			transform.localScale += new Vector3 (0.2f, 0.2f, 0.2f);
		}
	}
}

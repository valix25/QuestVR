using UnityEngine;
using System.Collections;

public class ShotBehavior : MonoBehaviour {

	public float shotSpeed = 10.0f;
	// Update is called once per frame
	void Update () {
		transform.position += transform.forward * Time.deltaTime * shotSpeed;
	}
}

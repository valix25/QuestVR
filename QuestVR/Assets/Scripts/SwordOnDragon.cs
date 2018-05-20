using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordOnDragon : MonoBehaviour {

	public float threshold;
	public bool isTaken;
	public Material red;
	//private Material originalMaterial;
	private Renderer renderer;
	private Vector3 originalLocalPos;

	void Start () {
		isTaken = false;
		originalLocalPos = transform.localPosition;
		renderer = GetComponent<Renderer> ();
		//originalMaterial = renderer.material;
	}

	public void TakeSword () {
		if (isTaken)
			return;
		
		renderer.material = red;
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 1, transform.localPosition.z);
		if (transform.localPosition.y - originalLocalPos.y < threshold) {
			isTaken = true;
			//renderer.material = originalMaterial;
			Destroy (gameObject);
		}
	}

}

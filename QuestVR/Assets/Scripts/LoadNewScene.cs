using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNewScene : MonoBehaviour {

	void OnCollisionEnter(Collision col) {
		if (col.gameObject.tag == "Portal") {
			
			Debug.Log ("VR: Load new scene");
			float fadeTime = this.GetComponent<Fading> ().BeginFade (1);
			Invoke ("StartNewScene", fadeTime/1.5f);

		}
	}

	void StartNewScene() {
		SceneManager.LoadScene ("SceneTwo");
		Debug.Log ("VR: StartnewScene called");
		// Scene sceneLoaded = SceneManager.GetActiveScene ();
		// SceneManager.MoveGameObjectToScene (this.gameObject, sceneLoaded);
		// this.transform.position = new Vector3 (-687, 37, 137);
	}
}

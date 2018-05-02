using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlAnimations : MonoBehaviour {

	public int defend_time = 2;

	private Animator anim;
	bool started_defend = false;
	float defend_timer = 0.0f;
	bool attack1_trigger = false;
	bool attack2_trigger = false;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.Q)) {
			anim.SetBool ("defend", true);
			started_defend = true;
		}

		if (started_defend == true) {
			defend_timer += Time.deltaTime;
			print ("defend_timer: " + defend_timer);
			if (defend_timer % 60 > defend_time) {
				anim.SetBool ("defend", false);
				started_defend = false;
				defend_timer = 0.0f;
			}
		}

		if (Input.GetButton("Fire1")) {
			print ("Pressed fire1 / left mouse button");
			//anim.SetBool ("attack1", true);
			//attack1_trigger = true;
			anim.Play("attack1");
		}

		if (Input.GetButton ("Fire2")) {
			// anim.SetBool ("attack2", true);
			//attack2_trigger = true;
			anim.Play("attack2");
		}

		if (Input.GetKey (KeyCode.W)) {
			print ("Pressed W");
			anim.SetBool ("run", true);
		} else {
			anim.SetBool ("run", false);
		}
	}
}

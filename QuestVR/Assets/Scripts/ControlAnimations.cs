using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlAnimations : MonoBehaviour {

	private Animator anim;
	private bool started_defend = false;
	private float defend_timer = 0.0f;
	private bool started_fireball = true;
	private float fireball_timer = 0.0f;

	public int defend_time = 2;
	public int fireball_time = 1;
	public GameObject fireball;
	public Camera camera;
	public float beamSpeed = 2f;
	public float range = 100f;
	public float fireballDelay = 0.5f;

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
			anim.Play("attack1");
		}

		if (started_fireball) {
			fireball_timer += Time.deltaTime;
			if (fireball_timer % 60 > fireball_time) {
				started_fireball = false;
				fireball_timer = 0.0f;
			}
		} else if (Input.GetButton ("Fire2")) {
			anim.Play("attack2");
			Invoke("shootRaycast", fireballDelay);
			started_fireball = true;
		}

		if (Input.GetKey (KeyCode.W)) {
			print ("Pressed W");
			anim.SetBool ("run", true);
		} else {
			anim.SetBool ("run", false);
		}
	}

	void shootRaycast() {
		// 1. Instantiate fireball
		GameObject projectile = Instantiate (fireball) as GameObject;
		projectile.transform.position = camera.transform.position + camera.transform.forward * 2 + Vector3.up * 0.3f;
		// Vector3 newrotation = camera.transform.rotation.eulerAngles;
		// newrotation += new Vector3 (0.0f, -90.0f, 0.0f);
		// projectile.transform.Rotate(newrotation);
		// 2. Give the fireball some velocity in the viewing direction
		Rigidbody rb = projectile.GetComponent<Rigidbody> ();
		rb.velocity = camera.transform.forward * beamSpeed;
		Destroy (projectile, 5.0f);

		RaycastHit hit;
		if (Physics.Raycast (camera.transform.position, camera.transform.forward, out hit, range)) {
			print (hit.transform.tag);
			// Enemy enemy = hit.transform.GetComponent<Enemy> ();
			// if (enemy != null) {
			// 	enemy.Die ();
			// }
		}
	}
}

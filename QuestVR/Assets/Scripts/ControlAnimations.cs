using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlAnimations : MonoBehaviour {

	private Animator anim;
	private bool started_defend = false;
	private float defend_timer = 0.0f;
	private bool started_fireball = true;
	private float fireball_timer = 0.0f;
	private bool started_laser = true;
	private float laser_timer = 0.0f;

	public Camera camera;
	public GameObject shield;
	public float defendTime = 2;
	public float shieldDelay = 0.5f;
	public float shieldLifetime = 5f;
	public GameObject fireball;
	public int fireballTime = 1;
	public float fireballSpeed = 2f;
	public float fireballAcceleration = 100f;
	public float fireballDelay = 0.5f;
	public GameObject laser;
	public int laserTime = 2;
	public float laserSpeed = 3f;
	public float laserDelay = 1.0f;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		// 1. Pressing 'q' triggers defend animation
		if (started_defend == true) {
			defend_timer += Time.deltaTime;
			if (defend_timer % 60 > defendTime) {
				anim.SetBool ("defend", false);
				started_defend = false;
				defend_timer = 0.0f;
			}
		} else if (Input.GetKeyDown (KeyCode.Q)) {
			// anim.SetBool ("defend", true);
			anim.Play("defend");
			Invoke ("shieldWall", shieldDelay);
			started_defend = true;
		}

		// 2. Left mouse shoots fireball
		if (started_fireball) {
			fireball_timer += Time.deltaTime;
			if (fireball_timer % 60 > fireballTime) {
				started_fireball = false;
				fireball_timer = 0.0f;
			}
		} else if (Input.GetButton ("Fire1")) {
			anim.Play("attack1");
			Invoke("shootFireball", fireballDelay);
			started_fireball = true;
		}

		// 3. Right mouse shoots laser beam
		if (started_laser) {
			laser_timer += Time.deltaTime;
			if (laser_timer % 60 > laserTime) {
				started_laser = false;
				laser_timer = 0.0f;
			}
		} else if (Input.GetButton("Fire2")) {
			anim.Play("attack2");
			Invoke ("shootLaser", laserDelay);
			started_laser = true;
		}
			
		// 4. Trigger run animation
		if (Input.GetKey (KeyCode.W)) {
			anim.SetBool ("run", true);
		} else {
			anim.SetBool ("run", false);
		}
	}

	void shootFireball() {
		// 1. Instantiate fireball
		GameObject projectile = Instantiate (fireball) as GameObject;
		projectile.transform.position = camera.transform.position + camera.transform.forward * 0.5f + Vector3.up * 0.3f;
		// 2. Give the fireball some velocity in the viewing direction
		Rigidbody rb = projectile.GetComponent<Rigidbody> ();
		rb.velocity = camera.transform.forward * fireballSpeed;
		rb.AddForce(camera.transform.forward * fireballAcceleration, ForceMode.Acceleration);
		Destroy (projectile, 5.0f);
	}

	void shootLaser(){
		// 1. Instantiate laser
		GameObject laserObj = Instantiate (laser) as GameObject;
		laserObj.transform.position = camera.transform.position + camera.transform.forward * 2.25f + Vector3.up * 0.2f;
		Vector3 newrotation = new Vector3(0.0f, camera.transform.rotation.eulerAngles.y, camera.transform.rotation.eulerAngles.z);
		newrotation += new Vector3 (0.0f, 180.0f, 0.0f);
		laserObj.transform.Rotate(newrotation);
		// 2. Give the laser some velocity in the right direction
		Rigidbody rb = laserObj.GetComponent<Rigidbody>();
		Vector3 dir = new Vector3 (camera.transform.forward.x, 0.0f, camera.transform.forward.z);
		rb.velocity = dir * laserSpeed;
		Destroy (laserObj, 5.0f);
	}

	void shieldWall(){
		// 1. Instantiate shield
		GameObject shieldObj = Instantiate(shield) as GameObject;
		shieldObj.transform.position = camera.transform.position + camera.transform.forward * 2f - Vector3.up * 0.1f;
		Vector3 newrotation = camera.transform.rotation.eulerAngles;
		newrotation += new Vector3 (0.0f, 180.0f, 0.0f);
		shieldObj.transform.Rotate (newrotation);
		Destroy (shieldObj, shieldLifetime);
	}
}

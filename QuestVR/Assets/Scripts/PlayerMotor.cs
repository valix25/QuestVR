using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// always need a rigidbody for the object this script is attached to
[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {

	[SerializeField]
	private Camera cam;

	private Vector3 velocity = Vector3.zero;
	private Vector3 rotation = Vector3.zero;
	private float cameraRotationX = 0f;
	private float currentCameraRotationX = 0f;
	private Vector3 thrusterForce = Vector3.zero;

	[SerializeField]
	private float cameraRotationLimit = 85f;

	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
	}
	
	// Run every physics update
	void FixedUpdate () {
		PerformMovement ();
		PerformRotation ();
		PerformJumping ();
	}

	public void Move(Vector3 _velocity) {
		velocity = _velocity;
	}

	public void Rotate(Vector3 _rotation) {
		rotation = _rotation;
	}

	public void RotateCamera(float _cameraRotationX) {
		cameraRotationX = _cameraRotationX;
	}

	public void ApplyThruster(Vector3 _thrusterForce) {
		thrusterForce = _thrusterForce;
	}

	private void PerformMovement(){
		if (velocity != Vector3.zero) {
			rb.MovePosition (transform.position + velocity * Time.fixedDeltaTime);
		}
//		if (thrusterForce != Vector3.zero) {
//			rb.AddForce (thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
//		}
	}

	private void PerformJumping() {
		if (velocity != Vector3.zero) {
			if (thrusterForce != Vector3.zero) {
				rb.AddForce (thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
			}
		}
	}

	private void PerformRotation() {
		if (rotation != Vector3.zero) {
			rb.MoveRotation (rb.rotation * Quaternion.Euler (rotation));
		}
		if (cam != null) {
//			if (cameraRotation != Vector3.zero) {
//				cam.transform.Rotate (-cameraRotation);
//			}
			// New rotational calculation
			currentCameraRotationX -= cameraRotationX;
			currentCameraRotationX = Mathf.Clamp (currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
			cam.transform.localEulerAngles = new Vector3 (currentCameraRotationX, 0f, 0f);
		}
	}
}

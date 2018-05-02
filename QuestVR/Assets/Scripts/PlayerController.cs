using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

	[SerializeField]
	private float speed = 5f;

	[SerializeField]
	private float lookSensitivity = 3f;

	[SerializeField]
	private float thrusterForce = 1000f;

	private PlayerMotor motor;

	// Use this for initialization
	void Start () {
		motor = GetComponent<PlayerMotor> ();
	}
	
	// Update is called once per frame
	void Update () {
		// Calculate movement velocity as 3d vector
		// Whenever we use a keyboard or controller the "Horizontal" and "Vertical" will go between -1 and 1
		float _xMov = Input.GetAxisRaw("Horizontal");
		float _zMov = Input.GetAxisRaw ("Vertical");

		// Unlike Vector3.right, Transform.right moves the GameObject while also considering its rotation.
		// When a GameObject is rotated, the red arrow representing the X axis of the GameObject also changes direction. 
		// Transform.right moves the GameObject in the red arrow’s axis (X).
		Vector3 _movHorizontal = transform.right * _xMov; // transform.right (1,0,0)
		Vector3 _movVertical = transform.forward * _zMov; // transform.forward (0,0,1)

		// get local velocity vector, final movement vector
		Vector3 _velocity = (_movVertical + _movHorizontal).normalized * speed;

		motor.Move (_velocity);

		// Calculate rotation as a 3D vector (turning around)
		float _yRot = Input.GetAxisRaw("Mouse X");
		Vector3 _rotation = new Vector3 (0f, _yRot, 0f) * lookSensitivity;

		// Apply rotation
		motor.Rotate (_rotation);

		// Calculate camera rotation as a 3D vector (up -- down)
		float _xRot = Input.GetAxisRaw("Mouse Y");
		// Vector3 _cameraRotation = new Vector3 (_xRot, 0f, 0f) * lookSensitivity;
		float _cameraRotationX = _xRot * lookSensitivity;

		// Apply rotation
		motor.RotateCamera (_cameraRotationX);

		// Apply the thruster force
		Vector3 _thrusterForce = Vector3.zero;
		// if(Input.GetButton("Jump")) {
			_thrusterForce = Vector3.up * thrusterForce;
		// }

		motor.ApplyThruster (_thrusterForce);
	}
}

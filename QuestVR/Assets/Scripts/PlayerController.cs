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

	private float timer = 0.0f;
	public float bobbingSpeed = 0.18f;
	public float bobbingAmount = 0.2f;
	public float midpoint = 2.0f;

	private PlayerMotor motor;
	public float count = 0;
	float waveslice;
	float horizontal;
	float vertical;
	Vector3 cSharpConversion;

	// Use this for initialization
	void Start () {
		motor = GetComponent<PlayerMotor> ();
	}
	
	// Update is called once per frame
	void Update () {
		// 1. Get user input and move player front -- back and left -- right
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
		if (Mathf.Abs (_xMov) != 0 || Mathf.Abs (_zMov) != 0) {
			motor.Move (_velocity);
		} else {
			motor.Move (Vector3.zero);
		}
		headBobbing();

		// 2. Calculate rotation as a 3D vector (turning around left -- right)
		float _yRot = Input.GetAxisRaw("Mouse X");
		Vector3 _rotation = new Vector3 (0f, _yRot, 0f) * lookSensitivity;
		motor.Rotate (_rotation);

		// 3. Calculate camera rotation as a 3D vector (up -- down)
		float _xRot = Input.GetAxisRaw("Mouse Y");
		// Vector3 _cameraRotation = new Vector3 (_xRot, 0f, 0f) * lookSensitivity;
		float _cameraRotationX = _xRot * lookSensitivity;
		motor.RotateCamera (_cameraRotationX);

		// Optional 4. Apply the thruster force
		Vector3 _thrusterForce = Vector3.zero;
		_thrusterForce = Vector3.up * thrusterForce;
		// motor.ApplyThruster (_thrusterForce);
	}

	void headBobbing() {
		waveslice = 0.0f;
		horizontal = Input.GetAxis("Horizontal");
		vertical = Input.GetAxis("Vertical");

		cSharpConversion = transform.localPosition;
		if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0) {
			timer = 0.0f;
		}
		else {
			waveslice = Mathf.Sin(timer);
			timer = timer + bobbingSpeed;
			if (timer > Mathf.PI * 2) {
				timer = timer - (Mathf.PI * 2);
			}
		}
		if (waveslice != 0) {
			float translateChange = waveslice * bobbingAmount;
			float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
			totalAxes = Mathf.Clamp (totalAxes, 0.0f, 1.0f);
			translateChange = totalAxes * translateChange;
			cSharpConversion.y = midpoint + translateChange;
		}
		else {
			cSharpConversion.y = midpoint;
		}
		transform.localPosition = cSharpConversion;
	}
}

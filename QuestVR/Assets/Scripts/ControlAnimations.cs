using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.DataTypes;

public class ControlAnimations : MonoBehaviour {

	private Animator anim;
	private bool started_defend = false;
	private float defend_timer = 0.0f;
	private bool started_fireball = true;
	private float fireball_timer = 0.0f;
	private bool started_laser = true;
	private float laser_timer = 0.0f;

	#region
	[Header("Watson credentials")]
	[SerializeField]
	private string _username;
	[SerializeField]
	private string _password;
	[SerializeField]
	private string _url;
	[SerializeField]
	bool _active = false;
	#endregion

	private int mode;

	[Header("Animations")]
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

	// Microphone recording parameters
	private int _recordingRoutine = 0;
	private string _microphoneID = null;
	private AudioClip _recording = null;
	private int _recordingBufferSize = 1;
	private int _recordingHZ = 22050;

	private SpeechToText _speechToText;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();

		LogSystem.InstallDefaultReactors();

		//  Create credential and instantiate service
		Credentials credentials = new Credentials(_username, _password, _url);

		// Initialize SpeechToText object
		_speechToText = new SpeechToText(credentials);
		Active = _active;

		StartRecording();
	}

	public bool Active
	{
		get { return _speechToText.IsListening; }
		set
		{
			if (value && !_speechToText.IsListening)
			{
				_speechToText.DetectSilence = true;
				_speechToText.EnableWordConfidence = true;
				_speechToText.EnableTimestamps = true;
				_speechToText.SilenceThreshold = 0.01f;
				_speechToText.MaxAlternatives = 0;
				_speechToText.EnableInterimResults = true;
				_speechToText.OnError = OnError;
				_speechToText.InactivityTimeout = -1;
				_speechToText.ProfanityFilter = false;
				_speechToText.SmartFormatting = true;
				_speechToText.SpeakerLabels = false;
				_speechToText.WordAlternativesThreshold = null;
				_speechToText.StartListening(OnRecognize, OnRecognizeSpeaker);
			}
			else if (!value && _speechToText.IsListening)
			{
				_speechToText.StopListening();
			}
		}
	}

	private void OnError(string error)
	{
		Active = false;
		Log.Debug("SphereSpeechProcessing.OnError()", "Error! {0}", error);
	}

	private void StartRecording()
	{
		if (_recordingRoutine == 0)
		{
			UnityObjectUtil.StartDestroyQueue();
			_recordingRoutine = Runnable.Run(RecordingHandler());
		}
	}

	private void StopRecording()
	{
		if (_recordingRoutine != 0)
		{
			Microphone.End(_microphoneID);
			Runnable.Stop(_recordingRoutine);
			_recordingRoutine = 0;
		}
	}

	private IEnumerator RecordingHandler()
	{
		Log.Debug("SphereSpeechProcessing.RecordingHandler()", "devices: {0}", Microphone.devices);
		_recording = Microphone.Start(_microphoneID, true, _recordingBufferSize, _recordingHZ);
		yield return null;      // let _recordingRoutine get set..

		if (_recording == null)
		{
			StopRecording();
			yield break;
		}

		bool bFirstBlock = true;
		int midPoint = _recording.samples / 2;
		float[] samples = null;

		while (_recordingRoutine != 0 && _recording != null)
		{
			int writePos = Microphone.GetPosition(_microphoneID);
			if (writePos > _recording.samples || !Microphone.IsRecording(_microphoneID))
			{
				Log.Error("SphereSpeechProcessing.RecordingHandler()", "Microphone disconnected.");

				StopRecording();
				yield break;
			}

			if ((bFirstBlock && writePos >= midPoint)
				|| (!bFirstBlock && writePos < midPoint))
			{
				// front block is recorded, make a RecordClip and pass it onto our callback.
				samples = new float[midPoint];
				_recording.GetData(samples, bFirstBlock ? 0 : midPoint);

				AudioData record = new AudioData();
				record.MaxLevel = Mathf.Max(Mathf.Abs(Mathf.Min(samples)), Mathf.Max(samples));
				record.Clip = AudioClip.Create("Recording", midPoint, _recording.channels, _recordingHZ, false);
				record.Clip.SetData(samples, 0);

				_speechToText.OnListen(record);

				bFirstBlock = !bFirstBlock;
			}
			else
			{
				// calculate the number of samples remaining until we ready for a block of audio, 
				// and wait that amount of time it will take to record.
				int remaining = bFirstBlock ? (midPoint - writePos) : (_recording.samples - writePos);
				float timeRemaining = (float)remaining / (float)_recordingHZ;

				//yield return new WaitForSeconds(timeRemaining);
				yield return null;
			}

		}

		yield break;
	}

	private void OnRecognizeSpeaker(SpeakerRecognitionEvent result, Dictionary<string, object> customData)
	{
		if (result != null)
		{
			foreach (SpeakerLabelsResult labelResult in result.speaker_labels)
			{
				Log.Debug("SphereSpeechProcessing.OnRecognize()", string.Format("speaker result: {0} | confidence: {3} | from: {1} | to: {2}", labelResult.speaker, labelResult.from, labelResult.to, labelResult.confidence));
			}
		}
	}

	private void OnRecognize(SpeechRecognitionEvent result, Dictionary<string, object> customData)
	{
		if (result != null && result.results.Length > 0)
		{
			foreach (var res in result.results)
			{
				foreach (var alt in res.alternatives)
				{
					// string text = string.Format("{0} ({1}, {2:0.00})\n", alt.transcript, res.final ? "Final" : "Interim", alt.confidence);
					Log.Debug("SphereSpeechProcessing.OnRecognize()", alt.transcript, res.final);
					if (alt.transcript.ToLower().Contains ("shield") || alt.transcript.ToLower().Contains("she'll") || 
						alt.transcript.ToLower().Contains ("defend")) {
						mode = 1; // shielWall animation
					}
					if (alt.transcript.ToLower ().Contains ("fire ball") || alt.transcript.ToLower ().Contains("fireball") ||
						alt.transcript.ToLower().Contains("firebolt") || alt.transcript.ToLower().Contains("fire bolt")) {
						mode = 2;
					}
					if (alt.transcript.ToLower().Contains ("laser")) {
						mode = 3;
					}
					if (alt.transcript.ToLower ().Contains ("disable")) {
						Active = false;
						_active = false;
						StopRecording ();
					}
					if (alt.transcript.ToLower ().Contains ("stop")) {
						mode = 0;
					}
				}

				//				if (res.keywords_result != null && res.keywords_result.keyword != null)
				//				{
				//					foreach (var keyword in res.keywords_result.keyword)
				//					{
				//						Log.Debug("SphereSpeechProcessing.OnRecognize()", "keyword: {0}, confidence: {1}, start time: {2}, end time: {3}", keyword.normalized_text, keyword.confidence, keyword.start_time, keyword.end_time);
				//					}
				//				}
				//
				//				if (res.word_alternatives != null)
				//				{
				//					foreach (var wordAlternative in res.word_alternatives)
				//					{
				//						Log.Debug("SphereSpeechProcessing.OnRecognize()", "Word alternatives found. Start time: {0} | EndTime: {1}", wordAlternative.start_time, wordAlternative.end_time);
				//						foreach(var alternative in wordAlternative.alternatives)
				//							Log.Debug("SphereSpeechProcessing.OnRecognize()", "\t word: {0} | confidence: {1}", alternative.word, alternative.confidence);
				//					}
				//				}
			}
		}
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
				mode = 0;
			}
		} else {
			if (Input.GetKeyDown (KeyCode.Q) && _active == false) {
				// anim.SetBool ("defend", true);
				anim.Play ("defend");
				Invoke ("shieldWall", shieldDelay);
				started_defend = true;
			}
			if (_active && mode == 1) {
				anim.Play ("defend");
				Invoke ("shieldWall", shieldDelay);
				started_defend = true;
			}
		}

		// 2. Left mouse shoots fireball
		if (started_fireball) {
			fireball_timer += Time.deltaTime;
			if (fireball_timer % 60 > fireballTime) {
				started_fireball = false;
				fireball_timer = 0.0f;
				mode = 0;
			}
		} else {
			if (Input.GetButton ("Fire1") && _active == false) {
				anim.Play ("attack1");
				Invoke ("shootFireball", fireballDelay);
				started_fireball = true;
			}
			if (_active && mode == 2) {
				anim.Play ("attack1");
				Invoke ("shootFireball", fireballDelay);
				started_fireball = true;
			}
		}

		// 3. Right mouse shoots laser beam
		if (started_laser) {
			laser_timer += Time.deltaTime;
			if (laser_timer % 60 > laserTime) {
				started_laser = false;
				laser_timer = 0.0f;
				mode = 0;
			}
		} else {
			if (Input.GetButton ("Fire2") && _active == false) {
				anim.Play ("attack2");
				Invoke ("shootLaser", laserDelay);
				started_laser = true;
			}
			if (_active && mode == 3) {
				anim.Play ("attack2");
				Invoke ("shootLaser", laserDelay);
				started_laser = true;
			}
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

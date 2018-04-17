using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.DataTypes;

public class SphereSpeechProcessing : MonoBehaviour {
	#region PLEASE SET THESE VARIABLES IN THE INSPECTOR
	[SerializeField]
	private string _username;
	[SerializeField]
	private string _password;
	[SerializeField]
	private string _url;
	#endregion

	public bool _active;
	public float speed;
	private int mode = 0;

	// Microphone recording parameters
	private int _recordingRoutine = 0;
	private string _microphoneID = null;
	private AudioClip _recording = null;
	private int _recordingBufferSize = 1;
	private int _recordingHZ = 22050;

	private SpeechToText _speechToText;

	// Use this for initialization
	void Start () {
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

	private void OnRecognizeSpeaker(SpeakerRecognitionEvent result)
	{
		if (result != null)
		{
			foreach (SpeakerLabelsResult labelResult in result.speaker_labels)
			{
				Log.Debug("SphereSpeechProcessing.OnRecognize()", string.Format("speaker result: {0} | confidence: {3} | from: {1} | to: {2}", labelResult.speaker, labelResult.from, labelResult.to, labelResult.confidence));
			}
		}
	}

	private void OnRecognize(SpeechRecognitionEvent result)
	{
		if (result != null && result.results.Length > 0)
		{
			foreach (var res in result.results)
			{
				foreach (var alt in res.alternatives)
				{
					// string text = string.Format("{0} ({1}, {2:0.00})\n", alt.transcript, res.final ? "Final" : "Interim", alt.confidence);
					Log.Debug("SphereSpeechProcessing.OnRecognize()", alt.transcript, res.final);
					if (alt.transcript.ToLower().Contains ("left") || alt.transcript.ToLower().Contains("last") || 
						alt.transcript.ToLower().Contains ("let")) {
						mode = 1;
					}
					if (alt.transcript.ToLower().Contains ("right")) {
						mode = 2;
					}
					if (alt.transcript.ToLower ().Contains ("front") || alt.transcript.ToLower ().Contains("from")) {
						mode = 3;
					}
					if (alt.transcript.ToLower ().Contains ("back")) {
						mode = 4;
					}
					if (alt.transcript.ToLower ().Contains ("disable") || alt.transcript.ToLower().Contains("off") ||
						alt.transcript.ToLower ().Contains ("of")) {
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

	void Update() {
		
	}

	void FixedUpdate() {
		Vector3 direction = Vector3.zero;
		if (mode == 0) {
			direction = Vector3.zero;
		} else if (mode == 1) {
			direction = new Vector3 (-1.0f, 0.0f, 0.0f);
		} else if (mode == 2) {
			direction = new Vector3 (1.0f, 0.0f, 0.0f);
		} else if (mode == 3) {
			direction = new Vector3 (0.0f, 0.0f, 1.0f);
		} else if (mode == 4) {
			direction = new Vector3 (0.0f, 0.0f, -1.0f);
		}
		this.transform.position += direction * Time.deltaTime * speed;
	}
}

﻿using SoundInput;
using System.Collections;
using UnityEngine;

public class VoiceRing : MonoBehaviour
{
	[SerializeField]
	private SoundInputController SIC;

	private VoiceRingData _data;

	private MeshRenderer _meshRenderer;

	private float _pitch = 0.0f;
	private float _currentPitch = 0.0f;
	private float _volume = 0.0f;
	private float _currentVolume = 0.0f;

	private bool _isFading = false;

	private void Awake()
	{
		_meshRenderer = GetComponent<MeshRenderer>();
	}

	private void Start()
	{
		//StartCoroutine(WaitForFade(_data.fadeAfterSecondsVar.value));
	}

	public void Setup(VoiceRingData voiceRingData, SoundInputController soundInputController)
	{
		_data = voiceRingData;
		SIC = soundInputController;
	}

	private void Update()
	{
		_pitch = SIC.inputData.relativeFrequency;
		_volume = SIC.inputData.relativeAmplitude;

		_currentPitch = LerpPitch(_data.pitchOffsetFactorVar.value, 1);
		_currentVolume = (_volume > 0) ? LerpVolume(_data.volumeOffsetFactorVar.value, 1) : 0.0f;

		transform.localScale = new Vector3(
			transform.localScale.x + _currentVolume + _data.moveSpeedVar.value * Time.deltaTime,
			transform.localScale.y + _currentVolume + _data.moveSpeedVar.value * Time.deltaTime,
			transform.localScale.z + _currentVolume + _data.moveSpeedVar.value * Time.deltaTime
		);

		// Fading
		if (transform.localScale.x >= 2000)
		{
			transform.localScale = new Vector3(
				transform.localScale.x,
				transform.localScale.y,
				transform.localScale.z - 18
			);

			Color newColor = _meshRenderer.material.color;
			float startAlpha = newColor.a;

			newColor.a = Mathf.Lerp(startAlpha, 0f, Time.deltaTime * _data.fadeSpeedVar.value);

			if (newColor.a <= 0.01)
				newColor.a = 0;

			//_meshRenderer.material.color = newColor;

			if (newColor.a <= 0)
				Destroy(gameObject);

			if (transform.localScale.z <= 0)
				Destroy(gameObject);
		}
	}

	private IEnumerator WaitForFade(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		_isFading = true;
	}

	#region VoiceRing Lerp
	private float LerpVolume()
	{
		return Mathf.Lerp(_currentVolume, _volume, Time.deltaTime * _data.volumeSpeedVar.value);
	}

	private float LerpVolume(float offsetFactor)
	{
		return Mathf.Lerp(_currentVolume, _volume * offsetFactor, Time.deltaTime * _data.volumeSpeedVar.value);
	}

	private float LerpVolume(float offsetFactor, float speedFactor)
	{
		return Mathf.Lerp(_currentVolume, _volume * offsetFactor, Time.deltaTime * _data.volumeSpeedVar.value * speedFactor);
	}

	private float LerpPitch()
	{
		return Mathf.Lerp(_currentPitch, _pitch, Time.deltaTime * _data.pitchSpeedVar.value);
	}

	private float LerpPitch(float offsetFactor)
	{
		return Mathf.Lerp(_currentPitch, _pitch * offsetFactor, Time.deltaTime * _data.pitchSpeedVar.value);
	}

	private float LerpPitch(float offsetFactor, float speedFactor)
	{
		return Mathf.Lerp(_currentPitch, _pitch * offsetFactor, Time.deltaTime * _data.pitchSpeedVar.value * speedFactor);
	}
	#endregion
}
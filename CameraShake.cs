using UnityEngine;
using Cinemachine;
using System.Collections;
using UnityEngine.Rendering;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance { get; private set; }

    private CinemachineVirtualCamera cinemachine;
    private float shakeTimer = 0f;
    private float shakeTimerTotal;
    private float startingIntensity;

    private void Awake()
    {
        instance = this;
        cinemachine = GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;

            CinemachineBasicMultiChannelPerlin cinemachineMultiBasicChannel = cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            // Smoothly reduce the shake intensity
            cinemachineMultiBasicChannel.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / shakeTimerTotal));
        }
        else
        {
            // Ensure the shake stops completely
            CinemachineBasicMultiChannelPerlin cinemachineMultiBasicChannel = cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineMultiBasicChannel.m_AmplitudeGain = 0f; // Explicitly stop the shaking
        }
    }


    public void ShakeCamera(float intensity, float frequency, float duration)
    {
        CinemachineBasicMultiChannelPerlin cinemachineMultiBasicChannel = cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineMultiBasicChannel.m_AmplitudeGain = intensity;
        cinemachineMultiBasicChannel.m_FrequencyGain = frequency;

        shakeTimer = duration;
        shakeTimerTotal = duration;
        startingIntensity = intensity;
    }

    public void StopShaking()
    {
        CinemachineBasicMultiChannelPerlin cinemachineMultiBasicChannel = cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineMultiBasicChannel.m_AmplitudeGain = 0f;
    }
}

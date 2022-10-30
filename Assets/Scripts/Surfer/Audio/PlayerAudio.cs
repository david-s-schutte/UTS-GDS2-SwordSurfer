using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Managers;
using Surfer.Audio;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] public FootstepParameters _footstepParameters;
    [SerializeField] private PlaySoundOneShot[] soundPlayer; // Scuffed implementation but should work for our purposes

    private readonly float _speedScale = 50f;
    private AudioManager _manager;
    private PlayerController _controller;
    
    private EventInstance windInstance;
    private EventInstance _grindInstance;
    private EventInstance hoverInstance;

    private void Start()
    {
        _manager = ManagerLocator.Get<AudioManager>();
        _controller = GetComponentInParent<PlayerController>();

        if (_controller != null)
        {
            _controller.OnModeChanged += PlayWind;
            _controller.OnGrindStateUpdated += PlayGrindingSound;
            _controller.OnJumpOccured += PlayJumpSound;
        }
    }


    private void OnDisable()
    {
        _controller.OnModeChanged -= PlayWind;
        _controller.OnGrindStateUpdated -= PlayGrindingSound;
        _controller.OnJumpOccured -= PlayJumpSound;
    }

    public void PlayWind(float speedValue, bool isSurfer)
    {
        var scaledSpeedValue = speedValue >= _speedScale ? 1f : speedValue / _speedScale;


        if (!isSurfer)
        {
            if (windInstance.isValid())
            {
                windInstance.setParameterByNameWithLabel("WindState", "NotBlowing");
            }

            if (hoverInstance.isValid())
            {
                hoverInstance.setParameterByNameWithLabel("Hovering", "Not Hovering");
            }

            return;
        }

        if (!windInstance.isValid()) // null checks for event instances
            windInstance = soundPlayer[1].PlaySoundOnce(soundPlayer[1].SelectedTrack);

        if (!hoverInstance.isValid())
            hoverInstance = soundPlayer[4].PlaySoundOnce(soundPlayer[4].SelectedTrack);
        
        Debug.Log(scaledSpeedValue);
        windInstance.setParameterByName("Wind", scaledSpeedValue);
        hoverInstance.setParameterByNameWithLabel("Hovering", "Hovering");
    }

    public void PlayJumpSound() => soundPlayer[3].PlaySoundOnce(soundPlayer[3].SelectedTrack);

    public void PlayGrindingSound(bool grindingStarted)
    {

        if (grindingStarted)
        {
            if (!_grindInstance.isValid())
                _grindInstance = soundPlayer[2].PlaySoundOnce(soundPlayer[2].SelectedTrack);
        }
        else
        {
            if (_grindInstance.isValid())
                _grindInstance.setParameterByNameWithLabel("Grinding", "Not Grinding");
        }
        
    }


    public void PlayFootstep() => soundPlayer[0].PlaySoundOnce(soundPlayer[0].SelectedTrack);
}
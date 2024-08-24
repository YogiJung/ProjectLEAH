using System;
using ProjectLeah.Runtime.Utils;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

namespace ProjectLeah.Samples
{
    public class ProtagonistExample : MonoBehaviour
    {
        private string device_name; //detail
        private int length_sec = 16; // detail
        private bool loop = true; //detail
        private int frequency = 44100; //detail

        private Protagonist protagonist;
        private AudioSource principal_mic;
        private AudioDirector audioDirector = AudioDirector.Instance;
        private void Awake()
        {
            if (GetComponent<AudioSource>() == null)
            {
                principal_mic = gameObject.AddComponent<AudioSource>();
            }
            else
            {
                principal_mic = GetComponent<AudioSource>();
            }
            if (GetComponent<AudioListener>() == null)
            {
                gameObject.AddComponent<AudioListener>();
            }
            device_name = Microphone.devices[0]; //detail
            protagonist = audioDirector.CreateProtagonist(this.gameObject);
            protagonist.EquipMic(principal_mic, device_name, length_sec, loop, frequency);
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            audioDirector.UpdateAudioData();
        }
    }
}
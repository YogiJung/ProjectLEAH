using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Audio;

namespace ProjectLeah.Runtime.Utils
{
    public class Protagonist
    {
        private bool loop = true;
        private int frequency = 44100;
        private string device_name;
        private int length_sec;
        private AudioSource principal_mic;
        
        private float[] principal_mic_data;

        public Protagonist(GameObject protagonistObject)
        {
            
        }
        
        public void EquipMic(AudioSource principal_mic,string device_name, int length_sec, bool? optional_loop, int? optional_frequency)
        {
            this.loop = optional_loop ?? this.loop;
            this.frequency = optional_frequency ?? this.frequency;
            principal_mic.clip = Microphone.Start(device_name, this.loop, length_sec, this.frequency);
            this.principal_mic = principal_mic;
            this.device_name = device_name;
            this.length_sec = length_sec;
            principal_mic_data = new float[frequency / 4];
            Debug.Log($"EquipMic Finished : {device_name}");
            
        }

        public void StopMic()
        {
            if (Microphone.IsRecording(device_name))
            {
                Microphone.End(device_name);
            }
        }

        public void StartMic()
        {
            if (!Microphone.IsRecording(device_name))
            {
                Microphone.Start(device_name, loop, length_sec, frequency);
            }
        }
        private int lastSamplePosition = 0;

        public void UpdateMicData(ConcurrentQueue<float[]> principal_mic_queue)
        {
            if (principal_mic != null)
            {
                int currentPosition = Microphone.GetPosition(device_name); 
                
                if (currentPosition < lastSamplePosition)
                {
                    lastSamplePosition = 0; 
                }
                
                if (currentPosition - lastSamplePosition < principal_mic_data.Length)
                {
                    return; 
                }
        
                int offset = Mathf.Max(0, currentPosition - principal_mic_data.Length);
                int validLength = Mathf.Min(principal_mic_data.Length, principal_mic.clip.samples - offset);

                if (validLength > 0)
                {
                    try
                    {
                        principal_mic.clip.GetData(principal_mic_data, offset);
                        
                        principal_mic_queue.Enqueue(principal_mic_data);
                        lastSamplePosition = currentPosition;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Failed to get audio data: " + e.Message);
                    }
                }
            }
        }




        public float[] GetMicData()
        {
            return principal_mic_data;
        }

        public int GetFrequency()
        {
            return frequency;
        }

        public int GetLengthSec()
        {
            return length_sec;
        }
    }
}
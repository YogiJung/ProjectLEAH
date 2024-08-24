using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using NUnit.Framework.Constraints;
using ProjectLeah.Runtime.AudioUtils;
using ProjectLeah.Runtime.Objects;
using ProjectLeah.Runtime.TypeReference;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace ProjectLeah.Runtime.Utils
{
    public class Deuteragonist
    {
        private AudioDirector audioDirector = AudioDirector.Instance;
        private Transform protagonistTransform;
        private GameObject protagonistObject;
        private AudioSource principal_mic;
        private double audioInterval;
        public float hearing_threadhold = 10.0f;
        private GameObject deuteragonistObject;
        private ManagedObject managedObject;
        private Timer timer;
        private bool inRangeFlag = false;
        private SettingFormat settingFormat;
        private BackPressureFlag backPressureFlag = new BackPressureFlag(0);
        private AudioSource audio_player;
        
        private ConcurrentQueue<float[]> audio_queue = new ConcurrentQueue<float[]>();
        
        public Deuteragonist(GameObject deuteragonistObject, MainType.ConnectionType connectionType,string Host, int Port, int n_of_api, int n_of_thread = 1)
        {
            managedObject = Leah.Instance.CreateObject(connectionType, Host, Port, n_of_api, n_of_thread);
            audio_player = deuteragonistObject.GetComponent<AudioSource>();
            this.deuteragonistObject = deuteragonistObject;
        }

        public void InitializeSetting(MainType.personalServerSetUpFlag personalServerSetUpFlag, List<API> APIList, int TCP_PORT, int UDP_PORT)
        {   
            protagonistObject = audioDirector.GetProtagonistObject();
            if (protagonistObject != null)
            {
                protagonistTransform = protagonistObject.transform;
                principal_mic = protagonistObject.GetComponent<AudioSource>();
                audioInterval = audioDirector.GetAudioInterval();
                settingFormat = managedObject.BuildSettingFormat(APIList,TCP_PORT, UDP_PORT ,personalServerSetUpFlag);
                managedObject.Connect(settingFormat, backPressureFlag);
                Debug.Log("Managed Object Connection ");
                setTimer(audioInterval);
                managedObject.Read();
            }
        }

        private void setTimer(double audioInterval)
        {
            timer = new Timer(audioInterval);
            timer.Elapsed += ExecuteAudioInterval;
            timer.AutoReset = true;
            timer.Enabled = true;
        }
        
        public void RadiusControl()
        {
            if (protagonistTransform != null && principal_mic != null)
            {
                float distance = Vector3.Distance(deuteragonistObject.transform.position, protagonistTransform.position);
                
                float volume = Mathf.Clamp01(1.0f - (distance / hearing_threadhold));
                inRangeFlag = volume > 0.1f;
            }
        }
        
        private void ExecuteAudioInterval(object source, ElapsedEventArgs e)
        {
                if (inRangeFlag)
                { 
                    if (audio_queue.TryDequeue(out float[] principal_mic_data))
                    {
                        ExchangeLines(principal_mic_data);
                    }
                }
        }

        public void ReceiveMicData(float[] principal_mic_data)
        {
            float[] received_audio_data = new float[principal_mic_data.Length];
            Array.Copy(principal_mic_data, received_audio_data, principal_mic_data.Length);
            audio_queue.Enqueue(received_audio_data);
        }
        
        public void ExchangeLines(float[] principal_mic_data)
        {

            try
            {
                if (backPressureFlag.flag == 0)
                {
                    string encoded_data = FormatCoder.EncodeMicData(principal_mic_data);
                    RequestFormat requestFormat = managedObject.BuildRequestFormat(encoded_data);
                    
                    managedObject.Send(requestFormat);
                }

                if (backPressureFlag.flag == 2)
                {
                    PlayAudio.PlayWAV(audio_player, Convert.FromBase64String(backPressureFlag.data), backPressureFlag);
                    backPressureFlag.CleanData();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"ExchangeLines Error: {ex}");
            }
            
        }

        public bool GetInRangeFlag()
        {
            return inRangeFlag;
        }
        
        
        
        
        
        

    }
}
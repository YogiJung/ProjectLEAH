using System.Collections.Concurrent;
using System.Collections.Generic;
using PlasticPipe.PlasticProtocol.Messages.Serialization;
using UnityEngine;

namespace ProjectLeah.Runtime.Utils
{
    public class AudioDirector
    {
        private static AudioDirector _instance;
        public static AudioDirector Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AudioDirector();
                }
                return _instance;
            }
        }

        private Protagonist protagonist;
        private GameObject protagonistObject;
        private List<Deuteragonist> deuteragonists;
        private ConcurrentQueue<float[]> principal_mic_queue = new ConcurrentQueue<float[]>();
        
        private AudioDirector()
        {
            deuteragonists = new List<Deuteragonist>();
        }

        public Protagonist CreateProtagonist(GameObject protagonistObject)
        {
            protagonist = new Protagonist(protagonistObject);
            this.protagonistObject = protagonistObject;
            return protagonist;
        }

        public Deuteragonist CreateDeuteragonist(GameObject deuteragonistObject, MainType.ConnectionType connectionType,string Host, int Port, int n_of_api, int n_of_thread = 1)
        {
            Deuteragonist deuteragonist = new Deuteragonist(deuteragonistObject, connectionType, Host, Port, n_of_api, n_of_thread);
            deuteragonists.Add(deuteragonist);
            return deuteragonist;
        }

        public GameObject GetProtagonistObject()
        {
            return protagonistObject;
        }

        public double GetAudioInterval()
        {
            if (protagonist != null)
            {
                double audioInterval = protagonist.GetLengthSec() / (double)protagonist.GetFrequency();
                return audioInterval;
            }
            double defaultAudioInterval = 16 / 44100.0;
            return defaultAudioInterval;
        }
        
        public void UpdateAudioData()
        {
            // Debug.Log("Update AUdioData");
            if (protagonist != null)
            {
                protagonist.UpdateMicData(principal_mic_queue);
                
                if (principal_mic_queue.TryDequeue(out float[] principal_mic_data))
                {
                    DistributeAudioData(principal_mic_data);
                }
                
            }
        }

        public void DistributeAudioData(float[] principal_mic_data)
        {
            // Debug.Log("Distribute Mic Data");
            foreach (Deuteragonist deuteragonist in deuteragonists)
            {
                if (deuteragonist.GetInRangeFlag())
                {
                    // Debug.Log("Distriute Foreach In");
                    deuteragonist.ReceiveMicData(principal_mic_data);
                }
            }
        }
        
    }
}
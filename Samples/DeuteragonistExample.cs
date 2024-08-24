using System;
using System.Collections.Generic;
using ProjectLeah.Runtime.Utils;
using UnityEngine;
namespace ProjectLeah.Samples
{
    public class DeuteragonistExample : MonoBehaviour
    {
        private string host = "127.0.0.1"; //detail
        private int port = 7777; //detail
        private MainType.ConnectionType connectionType = MainType.ConnectionType.DataStream; //detail
        private int n_of_api = 3; //detail
        
        //only if you use private server, put the correct port
        private int TCP_PORT = 8080; //detail
        private int UDP_PORT = 8181; //detail
        
        private MainType.personalServerSetUpFlag personalServerSetUpFlag =
            MainType.personalServerSetUpFlag.defaultServer; //detail

        private List<ProjectLeah.Runtime.TypeReference.API> APIList = new APIListBuilder.Builder()
            .Build("GoogleSTT", "Google User Authentication Json File", "DataStream", 1)
            .Build("ChatGPT", "ChatGPT Key", "DataStream", 2)
            .Build("ReplicaStudio", "Personal API Key", "DataStream", 3)
            .GetAPIList(); //detail
        

        private AudioSource audioSource;
        private Deuteragonist deuteragonist;
        private void Awake()
        {   
            audioSource = GetComponent<AudioSource>();
            
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            deuteragonist = AudioDirector.Instance.CreateDeuteragonist(this.gameObject, connectionType, host, port, n_of_api);
        }

        private void Start()
        {
            deuteragonist.InitializeSetting(personalServerSetUpFlag, APIList, TCP_PORT, UDP_PORT);
        }

        private void Update()
        {
            deuteragonist.RadiusControl();
        }
    }
}
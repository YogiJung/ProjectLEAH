namespace ProjectLeah.Runtime
{
    using System;
    using UnityEngine;
    
    public class Logger : MonoBehaviour
    {
        private static Logger instance;
    
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    
        public static Logger Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject logger = new GameObject("Logger");
                    instance = logger.AddComponent<Logger>();
                    DontDestroyOnLoad(logger);
                }
                
                return instance;
            }
        }
    
        public static void Log(string message)
        {
            if (Instance != null)
            {
                Instance.InternalLog(message);
            }
            else
            {
                Debug.LogError("Logger instance is not initialized.");
            }
        }
    
        private void InternalLog(string message)
        {
            Debug.Log(message);
        }
    
        public static void LogException(System.Exception e) {
            if (instance != null)
            {
                instance.InternalLog(e);
            }
            else
            {
                Debug.LogError("Logger instance is not initialized.");
            }
        }
    
        private void InternalLog(Exception e)
        {
            throw new NotImplementedException();
        }
    }

}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectLeah.Runtime.AudioUtils
{
    public class MainThreadDispatcher : MonoBehaviour
    {
        private static MainThreadDispatcher _instance;
        private Queue<Action> _actions = new Queue<Action>();

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        void Update()
        {
            while (_actions.Count > 0)
            {   
                Action action = null;
                lock (_actions)
                {
                    if (_actions.Count > 0)
                    {
                        action = _actions.Dequeue();
                    }
                }
                action?.Invoke();
            }
        }
        public void Enqueue(Action action)
        {
            lock (_actions)
            {
                _actions.Enqueue(action);
            }
        }
        public static MainThreadDispatcher Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject dispatcherObject = new GameObject("MainThreadDispatcher");
                    _instance = dispatcherObject.AddComponent<MainThreadDispatcher>();
                    DontDestroyOnLoad(dispatcherObject);
                }
                return _instance;
            }
        }
        
        public Coroutine StartMainThreadCoroutine(IEnumerator coroutine)
        {
            return StartCoroutine(coroutine);
        }

        public void StopMainThreadCoroutine(Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
    }
}
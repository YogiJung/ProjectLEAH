using System;
using System.Collections;
using System.Threading.Tasks;
using ProjectLeah.Runtime.TypeReference;
using UnityEngine;

namespace ProjectLeah.Runtime.AudioUtils
{
    public static class PlayAudio
    {
        public static async Task<int> PlayWAV(AudioSource audioSource, byte[] wavBytes, BackPressureFlag backPressureFlag)
        {
            
            int channels = BitConverter.ToInt16(wavBytes, 22);
            int sampleRate = BitConverter.ToInt32(wavBytes, 24);
            int byteRate = BitConverter.ToInt32(wavBytes, 28);
            int blockAlign = BitConverter.ToInt16(wavBytes, 32);
            int bitsPerSample = BitConverter.ToInt16(wavBytes, 34);
            int headerSize = 44; 
            float[] audioData = new float[(wavBytes.Length - headerSize) / blockAlign];
            for (int i = 0, j = headerSize; j < wavBytes.Length; i++, j += blockAlign)
            {
                short sample = BitConverter.ToInt16(wavBytes, j);
                audioData[i] = sample / 32768f;
            }
            
            var tcs = new TaskCompletionSource<int>();
            
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                if (audioSource == null)
                {
                    tcs.SetResult(0);
                    Debug.LogError("AudioSource component is not attached or is null.");
                }
                AudioClip audioClip = AudioClip.Create("WAVClip", audioData.Length, channels, sampleRate, false);
                if (audioClip == null)
                {
                    Debug.LogError("Failed to create AudioClip");
                    return;
                }
                audioClip.SetData(audioData, 0);
                try
                {
                    audioSource.clip = audioClip;
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
                audioSource.Play();
                MainThreadDispatcher.Instance.StartCoroutine(WaitForAudioEnd(audioSource, tcs, backPressureFlag));
                
            });
            return await tcs.Task;
        }
        
        
        public static async Task<int> PlayWAV(AudioSource audioSource, float[] wavFloatBytes, BackPressureFlag backPressureFlag)
        {
            if (audioSource == null)
            {
                Debug.LogError("AudioSource component is not attached or is null.");
                return 0;
            }

            byte[] wavBytes = FloatArrayToByteArray(wavFloatBytes);
            int channels = BitConverter.ToInt16(wavBytes, 22);
            int sampleRate = BitConverter.ToInt32(wavBytes, 24);
            int byteRate = BitConverter.ToInt32(wavBytes, 28);
            int blockAlign = BitConverter.ToInt16(wavBytes, 32);
            int bitsPerSample = BitConverter.ToInt16(wavBytes, 34);
            int headerSize = 44; 

            float[] audioData = new float[(wavBytes.Length - headerSize) / blockAlign];

            for (int i = 0, j = headerSize; j < wavBytes.Length; i++, j += blockAlign)
            {
                short sample = BitConverter.ToInt16(wavBytes, j);
                audioData[i] = sample / 32768f;
            }
            
            var tcs = new TaskCompletionSource<int>();
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                AudioClip audioClip = AudioClip.Create("WAVClip", audioData.Length, channels, sampleRate, false);
                if (audioClip == null)
                {
                    Debug.LogError("Failed to create AudioClip");
                    return;
                }
                
                audioClip.SetData(audioData, 0);
                audioSource.clip = audioClip;
                audioSource.Play();
                MainThreadDispatcher.Instance.StartCoroutine(WaitForAudioEnd(audioSource, tcs, backPressureFlag));
                
            });
            return await tcs.Task;
        }
        
        private static IEnumerator WaitForAudioEnd(AudioSource audioSource, TaskCompletionSource<int> tcs, BackPressureFlag backPressureFlag)
        {
            yield return new WaitWhile(() => audioSource.isPlaying);
            
            Debug.Log("Audio playback finished.");
            tcs.SetResult(0);
            backPressureFlag.flag = 0;
        }
        private static byte[] FloatArrayToByteArray(float[] floatArray)
        {
            int floatSize = sizeof(float);
            byte[] byteArray = new byte[floatArray.Length * floatSize];
            Buffer.BlockCopy(floatArray, 0, byteArray, 0, byteArray.Length);
            return byteArray;
        }
    }
}
using System;
using UnityEngine;

namespace ProjectLeah.Runtime.Utils
{
    public static class FormatCoder
    {
        public static string EncodeMicData(float[] principal_mic_data)
        {
            short[] shortBuffer = new short[principal_mic_data.Length];
            byte[] byteArray = new byte[principal_mic_data.Length];
            ConvertFloatArrayToShortArray(principal_mic_data, shortBuffer);
            byteArray = ConvertShortArrayToByteArray(shortBuffer);
            
            return Convert.ToBase64String(byteArray);
        }
        
        public static float[] DecodeMicData(string encoded_data)
        {
            byte[] byteArray = Convert.FromBase64String(encoded_data);
            
            float[] floatArray = new float[byteArray.Length / sizeof(float)];
            Buffer.BlockCopy(byteArray, 0, floatArray, 0, byteArray.Length);

            return floatArray;
        }
        
        private static byte[] ConvertShortArrayToByteArray(short[] shortArray)
        {
            byte[] byteArray = new byte[shortArray.Length * 2];
            Buffer.BlockCopy(shortArray, 0, byteArray, 0, byteArray.Length);
            return byteArray;
        }

        private static void ConvertFloatArrayToShortArray(float[] floatArray, short[] shortArray)
        {
            for (int i = 0; i < floatArray.Length; i++)
            {
                shortArray[i] = (short)(floatArray[i] * 32767);
            }
        }
    }
}
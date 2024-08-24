using ProjectLeah.Runtime.TypeReference;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace ProjectLeah.Runtime.Utils
{
    public static class Formatter
    {
        public static string RequestFormatToRequest(RequestFormat requestFormat)
        {
            string request = JsonConvert.SerializeObject(requestFormat);

            request += (char)0x03;
            return request;
        }

        public static string SettingFormatToSetting(SettingFormat settingFormat)
        {
            string setting = JsonConvert.SerializeObject(settingFormat);
            
            setting += (char)0x03; //0x03 is used as delimiter in this API
            return setting;
        }

        public static ResponseFormat ResponseToResponseFormat(string response)
        {
            try
            {
                if (!string.IsNullOrEmpty(response) && response.EndsWith("\x03"))
                {
                    response = response.Substring(0, response.Length - 1);
                }
                ResponseFormat responseFormat = JsonUtility.FromJson<ResponseFormat>(response);
                return responseFormat;
            }
            catch (JsonException ex)
            {
                Debug.LogError($"Failed to deserialize response: {ex}");
                return null;
            }
        }
        
        
    }
}
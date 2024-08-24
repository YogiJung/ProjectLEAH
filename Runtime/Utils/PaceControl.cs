using ProjectLeah.Runtime.TypeReference;
using UnityEngine;

namespace ProjectLeah.Runtime.Utils
{
    public static class PaceControl
    {
        public static int ControlPace(ResponseFormat responseFormat)
        {
            if (responseFormat.header.endpoint.Equals("flowControl"))
            {
                Debug.Log("Change to Flow Control");
                return responseFormat.backPressureflag;
            }

            if (responseFormat.header.endpoint.Equals("result"))
            {
                return 2;
            }
            return 0;
        }
    }
}
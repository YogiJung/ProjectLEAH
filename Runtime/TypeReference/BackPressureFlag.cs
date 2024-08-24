using UnityEngine.PlayerLoop;

namespace ProjectLeah.Runtime.TypeReference
{
    public class BackPressureFlag
    {
        public int flag { get; set; }
        public string data { get; set; }
        public BackPressureFlag(int flag = 0)
        {
            this.flag = flag;
        }

        public void CleanData()
        {
            data = "";
        }
    }

}
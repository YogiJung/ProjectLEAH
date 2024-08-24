using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ProjectLeah.Runtime.TypeReference
{
    [Serializable]
    public class SettingFormat
    {
        public Header header;
        public MainType.personalServerSetUpFlag personalServerSetUpFlag;
        public List<API> APIList = new List<API>();
        public int TCP_PORT;
        public int UDP_PORT;
        
    }
    
}
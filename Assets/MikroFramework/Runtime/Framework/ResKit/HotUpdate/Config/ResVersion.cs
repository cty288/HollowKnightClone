using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MikroFramework.ResKit
{
    [Serializable]
    public class ResVersion
    {
        public string Version;

        public List<ABMD5Base> ABMD5List;

    }

    [Serializable]
    public class ABMD5Base {
        public string AssetName;
        public string MD5;
        public float FileSize;
        public List<AssetData> assetDatas = new List<AssetData>();
    }
}

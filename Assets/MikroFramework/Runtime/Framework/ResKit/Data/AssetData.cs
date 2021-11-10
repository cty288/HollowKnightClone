using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.ResKit
{
    /// <summary>
    /// Data of an asset in an AssetBundle
    /// </summary>
    [Serializable]
    public class AssetData {
        public string Name;
        public string OwnerBundleName;

        public Type AssetType;
    }
}

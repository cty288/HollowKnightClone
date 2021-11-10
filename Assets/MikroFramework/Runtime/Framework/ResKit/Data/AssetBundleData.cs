using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.ResKit
{
    public enum AssetBundleLoadOption {
        UnKnown,
        Simulation,
        FromLocalFolder,
        FromHotUpdateFolder
    }
    /// <summary>
    /// Data of an AssetBundle
    /// </summary>
    public class AssetBundleData {
        public string Name;
        public List<AssetData> AssetDataList = new List<AssetData>();
        public string[] DependencyBundleNames;
        public AssetBundleLoadOption LoadOption;
        public string MD5;
    }
}

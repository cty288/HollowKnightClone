using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MikroFramework.DataStructures;
using MikroFramework.ResKit;
using UnityEngine;

namespace MikroFramework
{
    public class AssetDataTable : Table<AssetData> {
        public TableIndex<string, AssetData> NameIndex = new TableIndex<string, AssetData>(data=>data.Name);

        public AssetData GetAssetDataByResSearchKeys(ResSearchKeys resSearchKeys) {
            return NameIndex.Get(resSearchKeys.Address).FirstOrDefault(r => r.AssetType == resSearchKeys.ResType);
        }

        protected override void OnClear() {
            NameIndex.Clear();
        }

        public override void OnAdd(AssetData item) {
            NameIndex.Add(item);
        }

        public override void OnRemove(AssetData item) {
            NameIndex.Remove(item);
        }
    }
}

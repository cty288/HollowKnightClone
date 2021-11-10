using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MikroFramework.DataStructures;
using UnityEngine;

namespace MikroFramework.ResKit
{
    public class ResTable :Table<Res> {
        public TableIndex<string,Res> NameIndex { get; private set; }

        public ResTable() {
            NameIndex = new TableIndex<string, Res>(res => res.Name);
        }

        protected override void OnClear() {
            NameIndex.Clear();
        }

        public Res GetResWithSearchKeys(ResSearchKeys resSearchKeys) {
            IEnumerable<Res> results = NameIndex.Get(resSearchKeys.Address);
            if (results != null) {
                return results.FirstOrDefault(r => r.MatchResSearchKeysWithoutName(resSearchKeys));
            }

            return null;
        }

        public override void OnAdd(Res item) {
            NameIndex.Add(item);
        }

        public override void OnRemove(Res item) {
            NameIndex.Remove(item);
        }
    }
}

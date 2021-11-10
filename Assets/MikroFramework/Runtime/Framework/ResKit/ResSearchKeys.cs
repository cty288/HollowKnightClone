using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.Pool;

namespace MikroFramework.ResKit
{
    public class ResSearchKeys:IPoolable {
        /// <summary>
        /// Res Address (prefix + path)
        /// </summary>
        public string Address { get; private set; }

        public string OwnerBundleName { get; private set; }

        public Type ResType { get; private set; }

        public ResSearchKeys() { }
        public void OnRecycled() {
            Address = null;
            ResType = null;
            OwnerBundleName = null;
        }

        public bool IsRecycled { get; set; }
        public void RecycleToCache() {
            SafeObjectPool<ResSearchKeys>.Singleton.Recycle(this);
        }

        public static ResSearchKeys Allocate(string address, Type resType, string ownerBundleName=null) {
            ResSearchKeys resSearchKeys = SafeObjectPool<ResSearchKeys>.Singleton.Allocate();
            resSearchKeys.ResType = resType;
            resSearchKeys.Address = address;
            resSearchKeys.OwnerBundleName = ownerBundleName;
            return resSearchKeys;
        }
    }
}

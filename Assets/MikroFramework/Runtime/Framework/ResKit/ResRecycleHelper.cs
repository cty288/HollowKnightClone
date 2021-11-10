using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.Pool;

namespace MikroFramework.ResKit
{
    public static class ResRecycleHelper {
        public static void RecycleToPool(Res res) {
            Type resType = res.GetType();

            if (resType == typeof(ResourcesRes)) {
                (res as ResourcesRes).RecycleToCache();
            }else if (resType == typeof(AssetBundleRes)) {
                (res as AssetBundleRes).RecycleToCache();
            }
            else if (resType == typeof(AssetRes)) {
                (res as AssetRes).RecycleToCache();
            }
        }
    }
}

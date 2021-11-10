using System.Collections;
using System.Collections.Generic;
using Antlr.Runtime.Misc;
using UnityEngine;

namespace MikroFramework.ResKit
{
    public class ResFactory {
        private static Func<ResSearchKeys, Res> resCreator = s => null;

        /// <summary>
        /// Create a proper Res Object depends on assetName and assetBundleName
        /// For Resources asset: start with "resources://"
        /// For AssetBundle and its single asset: simply write AB file (asset) name
        /// For customized Res Class Object: Customize its create method by calling RegisterCustomRes()
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="assetBundleName"></param>
        /// <returns></returns>
        public static Res CreateRes(ResSearchKeys resSearchKeys) {
            Res res = null;

            if (resSearchKeys.Address.StartsWith("resources://")) {
                res = ResourcesRes.Allocate(resSearchKeys.Address,resSearchKeys.ResType);
            }
            else { //customized res
                res = resCreator.Invoke(resSearchKeys);
            }

            if (res == null) { //AssetBundles - no prefix
                if (resSearchKeys.OwnerBundleName != null || resSearchKeys.ResType!=typeof(AssetBundle)) {

                    res = AssetRes.Allocate(resSearchKeys.Address, resSearchKeys.OwnerBundleName,
                        resSearchKeys.ResType);
                }
                else
                {
                    res = AssetBundleRes.Allocate(resSearchKeys.Address);
                }
            }

            res.ResType = resSearchKeys.ResType;

            return res;
        }

        /// <summary>
        /// Register a customized resource creator in ResFactory
        /// </summary>
        /// <param name="customResCreator"></param>
        public static void RegisterCustomRes(Func<ResSearchKeys, Res> customResCreator) {
            resCreator = customResCreator;
        }
    }
}

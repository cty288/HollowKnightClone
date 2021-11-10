using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.Architecture
{
    public interface ICanGetUtility : IBelongToArchitecture
    {
       
    }

    public static class CanGetUtilityExtension {
        public static T GetUtility<T>(this ICanGetUtility self) where T : class, IUtility {
            return self.GetArchitecture().GetUtility<T>();
        }
    }
}

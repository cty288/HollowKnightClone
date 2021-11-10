using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace MikroFramework
{
    public interface ICanSendQuery : IBelongToArchitecture{
        
    }

    public static class CanSendQueryExtension {
        public static T SendQuery<T>(this ICanSendQuery self, IQuery<T> query) {
            return self.GetArchitecture().SendQuery(query);
        }
    }
}

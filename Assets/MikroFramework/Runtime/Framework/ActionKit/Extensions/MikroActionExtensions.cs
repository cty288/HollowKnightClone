using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.ActionKit
{
    public static class MikroActionExtensions {

        public static T Delay<T>(this IExtensible<T> self, float delayTime) where T:MikroAction,IExtensible<T> {
            return self.AddAction(DelayAction.Allocate(delayTime));
        }

        public static T Event<T>(this IExtensible<T> self, Action callback) where T : MikroAction, IExtensible<T> {
            return self.AddAction(CallbackAction.Allocate(callback));
        }

        public static T Until<T>(this IExtensible<T> self, Func<bool> triggeredCondition, Action untilCallback=null) where T : MikroAction, IExtensible<T> {
            UntilAction untilAction=UntilAction.Allocate(triggeredCondition);
            untilAction.OnEndedCallback = untilCallback;
            return self.AddAction(untilAction);
        }

        /// <summary>
        /// Reset self and execute again
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static T ResetSelf<T>(this IExtensible<T> self) where T:MikroAction, IExtensible<T> {
            return self.AddAction(ResetAction.Allocate((T) self, ()=>((T)self).Execute()));
        }



        public static T CustomizedAction<T,CustomizedAction>(this IExtensible<T> self) where T: MikroAction, IExtensible<T>
        where  CustomizedAction: MikroAction, new() {
            return self.AddAction(new CustomizedAction());
        }
    }
}

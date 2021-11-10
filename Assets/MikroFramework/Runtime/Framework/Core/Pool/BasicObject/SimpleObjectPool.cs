using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Factory;
using UnityEngine;

namespace MikroFramework.Pool {
   
    public class SimpleObjectPool<T> : ObjectPool<T> {
        private Action<T> resetMethod;

        public SimpleObjectPool(Func<T> createMethod, Action<T> resetMethod = null, int initialCount = 0) {
            factory = new CustomObjectFactory<T>(createMethod);
            this.resetMethod = resetMethod;
            for (int i = 0; i < initialCount; i++) {
                cachedStack.Push(factory.Create());
            }
        }

        public override bool Recycle(T obj) {
            if (resetMethod != null) {
                resetMethod(obj);
            }

            cachedStack.Push(obj);
            return true;
        }

    }

}

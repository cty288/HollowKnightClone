using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.Event;
using MikroFramework.Pool;

namespace MikroFramework
{
    public class CallbackRecord : IPoolable
    {
        public EventType EventType;
        public Action<MikroMessage> OnEventReceived;


        void IPoolable.OnRecycled() {
            OnEventReceived = null;
        }

        public bool IsRecycled { get; set; }

        public void RecycleToCache()
        {
            SafeObjectPool<CallbackRecord>.Singleton.Recycle(this);
        }

        /// <summary>
        /// Allocate a CallbackRecord from the pool
        /// </summary>
        /// <returns></returns>
        public static CallbackRecord Allocate() {
            return SafeObjectPool<CallbackRecord>.Singleton.Allocate();
        }
    }
}

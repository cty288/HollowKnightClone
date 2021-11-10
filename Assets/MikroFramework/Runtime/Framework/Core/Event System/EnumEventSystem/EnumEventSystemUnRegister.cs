using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.Pool;

namespace MikroFramework.Event { 

    public class EnumEventSystemUnRegister : IUnRegister {
        public Action<MikroMessage> Callback;
        public EventType EventType;
        public void UnRegister() {
            EventCenter.RemoveListener(EventType,Callback);
            Callback = null;
        }

        public EnumEventSystemUnRegister(EventType eventType, Action<MikroMessage> callback) {
            this.EventType = eventType;
            this.Callback = callback;
        }

    }
}

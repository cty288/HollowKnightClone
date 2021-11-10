using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikroFramework.Event {
    public class EventProperty {
        private Action ev;

        public void Trigger() {
            ev?.Invoke();
        }

        public void Register(Action callback) {
            ev += callback;
        }

        public void UnRegister(Action callback) {
            ev -= callback;
        }
    }

    public class EventProperty<T> {
        private Action<T> ev;

        public void Trigger(T msg)
        {
            ev?.Invoke(msg);
        }

        public void Register(Action<T> callback)
        {
            ev += callback;
        }

        public void UnRegister(Action<T> callback)
        {
            ev -= callback;
        }
    }

    public class EventProperty<T,Y>
    {
        private Action<T,Y> ev;

        public void Trigger(T msg1, Y msg2)
        {
            ev?.Invoke(msg1,msg2);
        }

        public void Register(Action<T,Y> callback)
        {
            ev += callback;
        }

        public void UnRegister(Action<T,Y> callback)
        {
            ev -= callback;
        }
    }

    public class EventProperty<T, Y, X>
    {
        private Action<T, Y, X> ev;

        public void Trigger(T msg1, Y msg2, X msg3)
        {
            ev?.Invoke(msg1, msg2,msg3);
        }

        public void Register(Action<T, Y, X> callback)
        {
            ev += callback;
        }

        public void UnRegister(Action<T, Y, X> callback)
        {
            ev -= callback;
        }
    }
}

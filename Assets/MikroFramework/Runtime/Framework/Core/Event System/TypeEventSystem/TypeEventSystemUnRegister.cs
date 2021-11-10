using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.Pool;

namespace MikroFramework.Event
{
    public class TypeEventSystemUnRegister<T> : IUnRegister
    {
        public ITypeEventSystem TypeEventSystem;
        public Action<T> OnEvent;

        public TypeEventSystemUnRegister(ITypeEventSystem typeEventSystem, Action<T> onEvent) {
            this.TypeEventSystem = typeEventSystem;
            this.OnEvent = onEvent;
        }
        public void UnRegister()
        {
            TypeEventSystem.UnRegister<T>(OnEvent);
            TypeEventSystem = null;
            OnEvent = null;
        }

    }

}

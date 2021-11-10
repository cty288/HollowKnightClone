using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikroFramework.Event
{
    public interface ITypeEventSystem
    {
        void Send<T>() where T : new();
        void Send<T>(T e);
        IUnRegister Register<T>(Action<T> onEvent);
        void UnRegister<T>(Action<T> onEvent);
    }
}

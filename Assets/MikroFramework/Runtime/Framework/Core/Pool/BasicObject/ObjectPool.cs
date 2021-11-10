using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.Factory;

namespace MikroFramework.Pool
{
    public abstract class ObjectPool<T> : IPool<T>
    {
        protected Stack<T> cachedStack = new Stack<T>();
        protected IObjectFactory<T> factory;

        public int CurrentCount
        {
            get { return cachedStack.Count; }
        }
        public virtual T Allocate()
        {
            return cachedStack.Count > 0 ? cachedStack.Pop() : factory.Create();
        }

        public abstract bool Recycle(T obj);
    }

}

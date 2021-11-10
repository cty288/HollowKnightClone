using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikroFramework.Pool
{
    public interface IPool<T>
    {
        /// <summary>
        /// Allocate an object from the ObjectPool
        /// </summary>
        /// <returns>allocated object</returns>
        T Allocate();

        /// <summary>
        /// Recycle the object back to its pool.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>If recycle success</returns>
        bool Recycle(T obj);
    }
}

using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace MikroFramework
{
    public abstract class AbstractQuery<T> : IQuery<T> {
        private IArchitecture architecture;
        public T Do() {
            return OnDo();
        }

        protected abstract T OnDo();

        IArchitecture IBelongToArchitecture.GetArchitecture() {
            return architecture;
        }

        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) {
            this.architecture = architecture;
        }
    }
}

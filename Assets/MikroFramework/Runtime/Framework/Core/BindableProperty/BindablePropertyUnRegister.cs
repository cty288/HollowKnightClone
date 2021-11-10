using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.Event;
using MikroFramework.Pool;

namespace MikroFramework.BindableProperty
{
    class BindablePropertyUnRegister<T>:IUnRegister
    {
        public BindableProperty<T> Bindable { get; set; }

        public Action<T> OnValueChanged { get; set; }

        public BindablePropertyUnRegister(BindableProperty<T> bindable, Action<T> onValueChanged) {
            this.Bindable = bindable;
            this.OnValueChanged = onValueChanged;
        }

        public void UnRegister() {
            Bindable.UnRegisterOnValueChanged(OnValueChanged);
            Bindable = null;
        }

    }
}

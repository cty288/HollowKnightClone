using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikroFramework.ActionKit
{
    public interface IExtensible<T> where T: MikroAction, IExtensible<T> {
        T AddAction(MikroAction action);

    }
}

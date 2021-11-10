using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikroFramework.Architecture
{
    public interface IModel:IBelongToArchitecture, ICanSetArchitecture, ICanGetUtility, ICanSendEvent {
        void Init();
    }
}

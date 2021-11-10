using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikroFramework.FSM
{
    [Serializable]
    public class FSMJsonInfo {
        public List<FSMStateJsonInfo> FsmJsonInfos = new List<FSMStateJsonInfo>();
    }
}

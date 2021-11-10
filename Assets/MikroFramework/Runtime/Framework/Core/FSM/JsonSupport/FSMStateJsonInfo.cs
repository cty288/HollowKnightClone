using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikroFramework.FSM
{
    [Serializable]
    public class FSMStateJsonInfo {
        public string Name;
        public Dictionary<string, string> Translations = new Dictionary<string, string>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikroFramework.Event
{

    public class MikroMessage
    {
        public static MikroMessage Create(params object[] messages)
        {
            MikroMessage message = new MikroMessage();
            message.messages.AddRange(messages);
            return message;
        }


        private List<object> messages = new List<object>();
        public List<object> Messages => messages;

        public object GetMessage(int index)
        {
            if (messages.Count == 0) return null;
            if (index >= messages.Count || index < 0) return null;

            return messages[index];
        }

        public object GetSingleMessage()
        {
            if (messages.Count > 0)
            {
                return messages[0];
            }

            return null;
        }
        protected MikroMessage()
        {
        }
    }

}

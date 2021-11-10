using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MikroFramework.ActionKit
{
    public static class MonoBehaviorActionExtensions {
        public static Sequence Sequence(this MonoBehaviour self) {
            return MikroFramework.ActionKit.Sequence.Allocate();
        }

        public static Spawn Spawn(this MonoBehaviour self) {
            return MikroFramework.ActionKit.Spawn.Allocate();
        }

        public static RepeatSequence Repeat(this MonoBehaviour self, int repeatTime = 1) {
            return RepeatSequence.Allocate(repeatTime);
        }
    }
}

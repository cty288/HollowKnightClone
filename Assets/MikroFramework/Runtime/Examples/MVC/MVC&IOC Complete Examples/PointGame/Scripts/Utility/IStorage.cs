using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace MikroFramework.Examples
{
    public interface IStorage : IUtility {
        void SaveInt(string key, int value);
        int LoadInt(string key, int defaultValue = 0);
    }

}

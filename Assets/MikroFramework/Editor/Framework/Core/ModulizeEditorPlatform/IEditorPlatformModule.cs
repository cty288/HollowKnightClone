using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.EditorModulization
{
    public interface IEditorPlatformModule { 
        EditorPlatformElement ElementInfo { get; }
        void OnGUI();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.CodeGen {
    public static class BindingNamingRules
    {
        public static Dictionary<string, string[]> rules = new Dictionary<string, string[]>() {
            //each name prefix corresponds to a bind component
            {"img", new []{"Image"}},
            {"btn", new []{"Button"}},
            {"text", new []{"Text", "TMP_Text"}},
            {"obj", new []{"GameObject"}},
            {"tr", new []{"Transform"}},
            {"input", new []{ "InputField", "TMP_InputField"}},
            {"dropdown",new []{ "Dropdown","TMP_Dropdown" } },
            {"audio",new []{ "AudioSource" }},
            {"light",new[]{"Light"} }
        };

        
    }
}


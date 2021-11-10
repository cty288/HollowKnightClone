using System.Collections;
using System.Collections.Generic;
using MikroFramework.EditorModulization;
using UnityEngine;


namespace MikroFramework.ServiceLocator {
    public class ModuleContainerModule : IEditorPlatformModule {
        public EditorPlatformElement ElementInfo { get; } =
            new EditorPlatformElement(1, "Module Container - Instructions");

        public void OnGUI() {
            GUILayout.Label("How to use ModuleContainer", new GUIStyle() {
                fontSize = 20,
                fontStyle = FontStyle.Bold
            });

            GUILayout.Label("1. Specify the interface of your module",new GUIStyle(){fontStyle = FontStyle.Bold});
            GUILayout.Label("public interface IModule{}");

            GUILayout.Label("2. Create a default module cache", new GUIStyle() { fontStyle = FontStyle.Bold });
            GUILayout.Label("DefaultModuleCache cache = new DefaultModuleCache()");

            GUILayout.Label("3. Create a default module factory", new GUIStyle() { fontStyle = FontStyle.Bold });
            GUILayout.Label("AssemblyModuleFactory factory = new AssemblyModuleFactory(typeof(IModule).Assembly, typeof(IModule));");

            GUILayout.Label("4. Create a container instance", new GUIStyle() { fontStyle = FontStyle.Bold });
            GUILayout.Label("ModuleContainer container = new ModuleContainer(cache,factory)");

            GUILayout.Label("5. Get Module/Modules", new GUIStyle() { fontStyle = FontStyle.Bold });
            GUILayout.Label("IHelloModule helloModule = container.GetModule<IHelloModule>");
            GUILayout.Label("IEnumerable<IModule> modules = container.GetAllModules<IModule>");
        }
    }
}


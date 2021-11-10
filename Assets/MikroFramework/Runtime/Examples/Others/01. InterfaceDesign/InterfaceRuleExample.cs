using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class CanDoEverything {
        public void DoSth1() {
            Debug.Log("DoSomething1");
        }
        public void DoSth2()
        {
            Debug.Log("DoSomething2");
        }
        public void DoSth3()
        {
            Debug.Log("DoSomething3");
        }
    }

    public interface IHasEverything {
        CanDoEverything canDoEverything { get; }
    }


    public interface ICanDoSomething1 : IHasEverything {

    }

    public static class ICanDoSomething1Extension {
        public static void DoSomething1(this ICanDoSomething1 self) {
            self.canDoEverything.DoSth1();
        }
    }

    public interface ICanDoSomething2 : IHasEverything
    {

    }

    public static class ICanDoSomething2Extension
    {
        public static void DoSomething2(this ICanDoSomething2 self)
        {
            self.canDoEverything.DoSth2();
        }
    }

    public interface ICanDoSomething3 : IHasEverything
    {

    }

    public static class ICanDoSomething3Extension
    {
        public static void DoSomething3(this ICanDoSomething3 self)
        {
            self.canDoEverything.DoSth3();
        }
    }

    public class InterfaceRuleExample : MonoBehaviour {
        public class OnlyCanDo1 : ICanDoSomething1 {
            CanDoEverything IHasEverything.canDoEverything { get; } = new CanDoEverything();
        }

        public class OnlyCanDo23 : ICanDoSomething2,ICanDoSomething3 {
            CanDoEverything IHasEverything.canDoEverything { get; } = new CanDoEverything();
        }

        private void Start() {
            var onlyCanDo1 = new OnlyCanDo1();
            onlyCanDo1.DoSomething1();

            var onlyDo23 = new OnlyCanDo23();
            onlyDo23.DoSomething2();
            onlyDo23.DoSomething3();
        }
    }
}

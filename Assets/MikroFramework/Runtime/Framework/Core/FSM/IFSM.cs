using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace MikroFramework.FSM
{
    public interface IFSM {
        Dictionary<string, FSMState> StateDict { get; }

        IFSM AddState(FSMState state);

        IFSM AddState(string stateName);

        IFSM AddTranslation(FSMTranslation translation);

        IFSM AddTranslation([CanBeNull] string fromStateName, [NotNull] string translationName,
            [NotNull] string toStateName, [CanBeNull] FSM.FSMTranslationCallback callback);

        void Start(FSMState state);

        void Start(string stateName);

        void HandleEvent(string name);

        void Clear();

    }
}

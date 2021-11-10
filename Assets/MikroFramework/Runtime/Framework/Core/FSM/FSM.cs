using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using MikroFramework.Event;
using UnityEngine;
using EventType = MikroFramework.Event.EventType;

namespace MikroFramework.FSM
{
    /// <summary>
    /// FSM (Finite state machine). Support both string and enum based states
    /// </summary>
    public class FSM : IFSM {

        private class CouldBeAny : Attribute { }

        private class CouldNotBeAny : Attribute { }

        public delegate void FSMTranslationCallback();
        
        private FSMState currentState;

        /// <summary>
        /// All states
        /// </summary>
        private Dictionary<string, FSMState> stateDictionary = new Dictionary<string, FSMState>();

        public EventProperty<string, string> OnStateChanged = new EventProperty<string, string>();

        public Dictionary<string, FSMState> StateDict {
            get {
                return stateDictionary;
            }
        }

        /// <summary>
        /// Add a state using a FSMState object
        /// </summary>
        /// <param name="state"></param>
        public IFSM AddState(FSMState state) {
            stateDictionary[state.name] = state;
            return this;
        }


        /// <summary>
        /// Add a new state using state name
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public IFSM AddState(string stateName) {
            FSMState allocatedState=FSMState.Allocate(stateName);
            return AddState(allocatedState);
        }

       
        /// <summary>
        /// Add a new state using a state enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stateEnum"></param>
        /// <returns></returns>
        public FSM AddState<StateEnum>(StateEnum stateEnum) where StateEnum : System.Enum {
            string stateName = stateEnum.ToString();
            return AddState(stateEnum);
        }


        /// <summary>
        /// Add a translation to a state. If the "fromState" of the translation is null, the translation to be
        /// added to all states by default
        /// </summary>
        /// <param name="translation"></param>
        public IFSM AddTranslation(FSMTranslation translation) {
            if (translation.fromState == null) {
                var valueEnumerator = stateDictionary.Values.GetEnumerator();
                //recycle all states
                while (valueEnumerator.MoveNext()) {
                    FSMState state = valueEnumerator.Current;
                    if (state != null && state.name != translation.toState.name) {
                        stateDictionary[valueEnumerator.Current.name].AddTranslation(translation);
                    }
                }
            }
            else {
                stateDictionary[translation.fromState.name].AddTranslation(translation);
            }

            return this;

        }


        /// <summary>
        /// Add a new translation to the state machine, given state enum and translation (event) enum.
        /// If the corresponding state could not be fount in the state machine, the machine will
        /// auto create a new one.
        /// </summary>
        /// <typeparam name="StateEnum"></typeparam>
        /// <typeparam name="EventEnum"></typeparam>
        /// <param name="fromState">If the state enum is "Any", any state in this state machine can translate to the target state</param>
        /// <param name="translationEnum"></param>
        /// <param name="toState"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public FSM AddTranslation<StateEnum, EventEnum>([CouldBeAny] StateEnum fromState,
            [NotNull] EventEnum translationEnum, [CouldNotBeAny] StateEnum toState,
            [CanBeNull] FSMTranslationCallback callback) where StateEnum:Enum where EventEnum : Enum {

            string fromStateName = fromState.ToString();

            if (fromStateName.ToLower() == "any") {
                fromStateName = null;
            }

            string toStateName = toState.ToString();

            if (toStateName.ToLower() == "any") {
                Debug.LogError("toState enum could not be Any!");
            }

            string translationName = translationEnum.ToString();

            return AddTranslation(fromStateName, translationName, toStateName, callback) as FSM;
        }

        /// <summary>
        /// Add a new translation to the state machine, given state names and translation name.
        /// If the corresponding state name could not be fount in the state machine, the machine will
        /// auto create a new one.
        /// </summary>
        /// <param name="fromStateName"></param>
        /// <param name="translationName"></param>
        /// <param name="toStateName"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IFSM AddTranslation([CanBeNull] string fromStateName,[NotNull] string translationName,
            [NotNull] string toStateName,[CanBeNull] FSMTranslationCallback callback) {

            FSMState fromState=null;
            FSMState toState=null;

            if (!string.IsNullOrEmpty(fromStateName)) {
                bool exists = stateDictionary.TryGetValue(fromStateName, out fromState);
                if (!exists) {
                    fromState=FSMState.Allocate(fromStateName);
                    AddState(fromState);
                }
            }

            if (string.IsNullOrEmpty(toStateName)) {
                Debug.LogError("toStateName could not be null or empty!");
            }

            bool toStateExists = stateDictionary.TryGetValue(toStateName, out toState);
            if (!toStateExists) {
                toState = FSMState.Allocate(toStateName);
                AddState(toState);
            }

            FSMTranslation translation =   FSMTranslation.Allocate(fromState, translationName, toState, callback);
            return AddTranslation(translation);
        }


        

        /// <summary>
        /// Start the state machine with a initial state
        /// </summary>
        /// <param name="state"></param>
        public void Start(FSMState state) {
            currentState = state;
        }

        /// <summary>
        /// Start the state machine with a initial state enum
        /// </summary>
        /// <typeparam name="StateEnum"></typeparam>
        /// <param name="state">The enum could not be "Any"</param>
        public void Start<StateEnum>(StateEnum state) where StateEnum:Enum{
            if (state.ToString().ToLower() == "any") {
                Debug.LogError("Start state could not be Any!");
            }
            Start(state.ToString());
        }

        public void Start(string stateName) {
            if (stateDictionary.ContainsKey(stateName)) {
                Start(stateDictionary[stateName]);
            }
            else {
                Debug.LogError($"Couldn't find state {stateName} in state machine when starting the machine!");
            }
        }

        /// <summary>
        /// Trigger the event that switch the current state
        /// </summary>
        /// <param name="name"></param>
        public void HandleEvent(string name) {
            if (currentState != null && currentState.TranslationDictionary.ContainsKey(name)) {
                Debug.Log("From state: "+currentState.name);

                currentState.TranslationDictionary[name].callback?.Invoke();
                OnStateChanged.Trigger(currentState.name,currentState.TranslationDictionary[name].toState.name);

                currentState = currentState.TranslationDictionary[name].toState;

                Debug.Log("To state: " +currentState.name);
            }
        }

        /// <summary>
        /// Trigger the event that switch the current state, given the event enum
        /// </summary>
        /// <typeparam name="EventEnum"></typeparam>
        /// <param name="eventEnum"></param>
        public void HandleEvent<EventEnum>(EventEnum eventEnum) where EventEnum:Enum{
            HandleEvent(eventEnum.ToString());
        }

        public void Clear() {
            var valueEnumerator = stateDictionary.Values.GetEnumerator();
            //recycle all states
            while (valueEnumerator.MoveNext()) {
                FSMState state = valueEnumerator.Current;
                state.RecycleToCache();
            }

            stateDictionary.Clear();
            valueEnumerator.Dispose();
        }
    }
}

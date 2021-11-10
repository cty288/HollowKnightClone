
using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.BindableProperty;
using MikroFramework.Event;
using UnityEditor;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class UseBindablePropertyExample:MonoBehaviour
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("MikroFramework/Examples/BindableProperty/1. UseBindableProperty", false, 1)]
        private static void OnClicked() {
            UnityEditor.EditorApplication.isPlaying = true;
            new GameObject().AddComponent<UseBindablePropertyExample>();
        }
#endif
        private PlayerInfo playerInfo;
        private void Start() {
            playerInfo = new PlayerInfo();
            playerInfo.Name.Value = "TestName";
            playerInfo.Age.Value = 10;
            playerInfo.State.Value = new PlayerState() {hp = 10, mp = 20};

            playerInfo.Age.RegisterOnValueChaned(age => {
                Debug.Log($"New age: {age}");
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            playerInfo.State.RegisterOnValueChaned((state) => {
                Debug.Log($"State changed! HP:{state.hp}, MP: {state.mp}");
            }).UnRegisterWhenGameObjectDestroyed(this.gameObject);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                playerInfo.State.Value=new PlayerState(){hp=playerInfo.State.Value.hp+1, mp = playerInfo.State.Value.mp};
            }

            if (Input.GetKeyDown(KeyCode.A)) {
                playerInfo.Age.Value++;
            }
        }
    }

    [Serializable]
    public class PlayerInfo {
        public BindableProperty<string> Name { get; set; } = new BindableProperty<string>(){Value = ""};
        public BindableProperty<int> Age { get; set; } = new BindableProperty<int>(){Value = 0};
        public BindableProperty<PlayerState> State { get; set; }  = new BindableProperty<PlayerState>(){Value = new PlayerState()};
    }


    [Serializable]
    public class PlayerState:IEquatable<PlayerState> {
        public int hp;
        public int mp;

        public bool Equals(PlayerState other) {
            if (ReferenceEquals(null, other)) return false;
            return hp == other.hp && mp == other.mp;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PlayerState) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (hp * 397) ^ mp;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using MikroFramework.FSM;
using UnityEngine;

namespace MikroFramework
{
    public class FSMSerializationExample : MonoBehaviour
    {
        enum GameState
        {
            Any,
            Idle,
            Run,
            Jump,
            DoubleJump,
            Die
        }

        enum Events
        {
            PressW,
            PressS,
            PressUp,
            PressDown,
            PressD
        }

        FSM.FSM fsm = new FSM.FSM();
        void Start()
        {

            //Idle->"w"->run
            //Run->"up"->jump
            //Run->"s"->Idle
            //Jump->"up"->doubleJump
            //jump->"down"->run
            //doubleJump->"down"->run
            //"d"->die
            
            
            //WriteJson();
            ReadJson();
        }


        private void WriteJson() {
            fsm.AddTranslation(GameState.Idle, Events.PressW, GameState.Run, null)
                .AddTranslation(GameState.Run, Events.PressUp, GameState.Jump, null)
                .AddTranslation(GameState.Run, Events.PressS, GameState.Idle, null)
                .AddTranslation(GameState.Jump, Events.PressUp, GameState.DoubleJump, null)
                .AddTranslation(GameState.Jump, Events.PressDown, GameState.Run, null)
                .AddTranslation(GameState.DoubleJump, Events.PressDown, GameState.Run, null)
                .AddTranslation(GameState.Any, Events.PressD, GameState.Die, () => { Debug.Log("die"); })
                .ToJson(Application.dataPath+"/MikroFramework/Runtime/Examples/FSM/fsm.json");
        }

        private void ReadJson() {
            fsm = FSMJsonExtension.ReadJson(Application.dataPath + "/MikroFramework/Runtime/Examples/FSM/fsm.json");
            fsm.Start(GameState.Idle);
            fsm.OnStateChanged.Register(OnStateChanged);
        }

        private void OnStateChanged(string oldState, string newState) {
            if (oldState == GameState.Idle.ToString()) {
                Debug.Log($"From {oldState} to {newState}");
            }
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                fsm.HandleEvent(Events.PressW);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                fsm.HandleEvent(Events.PressS);
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                fsm.HandleEvent(Events.PressUp);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                fsm.HandleEvent(Events.PressDown);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                fsm.HandleEvent(Events.PressD);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                fsm.Clear();
                Debug.Log("FSM Cleared");
            }
        }
    }
}

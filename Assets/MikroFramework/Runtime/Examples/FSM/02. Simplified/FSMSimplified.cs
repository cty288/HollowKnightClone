using System.Collections;
using System.Collections.Generic;
using MikroFramework.FSM;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class FSMSimplified :MonoBehaviour
    {
        FSM.FSM fsm = new FSM.FSM();
        void Start()
        {
            //Idle
            //Run
            //Jump
            //DoubleJump
            //Die

            //Idle->"w"->run
            //Run->"up"->jump
            //Run->"s"->Idle
            //Jump->"up"->doubleJump
            //jump->"down"->run
            //doubleJump->"down"->run
            //"d"->die


            fsm.AddTranslation("idle", "w", "run", null)
                .AddTranslation("run", "up", "jump", null).
                AddTranslation("run", "s", "idle", null)
                .AddTranslation("jump", "up", "double_jump", null)
                .AddTranslation("jump","down","run",null)
                .AddTranslation("double_jump","down","run",null)
                .AddTranslation(null,"d","die",()=>{Debug.Log("die");})
                .Start("idle");
           
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                fsm.HandleEvent("w");
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                fsm.HandleEvent("s");
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                fsm.HandleEvent("up");
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                fsm.HandleEvent("down");
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                fsm.HandleEvent("d");
            }

            if (Input.GetKeyDown(KeyCode.R)) {
                fsm.Clear();
                Debug.Log("FSM Cleared");
            }
        }
    }
}

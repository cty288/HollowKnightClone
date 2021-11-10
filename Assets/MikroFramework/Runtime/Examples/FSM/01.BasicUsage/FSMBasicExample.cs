using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.FSM;
using UnityEngine;

namespace MikroFramework
{
    public class FSMBasicExample : MonoBehaviour
    {
        FSM.FSM fsm = new FSM.FSM();
        void Start() {
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

           

            FSMState idleState = FSMState.Allocate("idle");
            FSMState runState = FSMState.Allocate("run");
            FSMState jump = FSMState.Allocate("jump");
            FSMState doubleJump = FSMState.Allocate("double_jump");
            FSMState die = FSMState.Allocate("die");
           
            //fsm.addState(statename).Allocate(name, targetStateName, callback). Allocate(...).Allocate(...)
            
            //fsm.addState(statename).addState.....
            //fsm.AddTranslation(fromStateName, translationName, toStateName).AddTranslation...

            FSMTranslation idleToRun=FSMTranslation.Allocate(idleState,"w",runState,null);
            FSMTranslation runToJump = FSMTranslation.Allocate(runState, "up", jump, () => { });
            FSMTranslation runToIdle = FSMTranslation.Allocate(runState, "s", idleState, () => { });
            FSMTranslation jumpToDoubleJump = FSMTranslation.Allocate(jump, "up", doubleJump, () => { });
            FSMTranslation jumpToRun = FSMTranslation.Allocate(jump, "down", runState, () => { Debug.Log("Jump to run"); });
            FSMTranslation doubleJumpToRun = FSMTranslation.Allocate(doubleJump, "down", runState, () => { });
            FSMTranslation dieTranslation=FSMTranslation.Allocate(null, "d", die,()=>{});

            fsm.AddState(idleState);
            fsm.AddState(runState);
            fsm.AddState(jump);
            fsm.AddState(doubleJump);
            fsm.AddState(die);

            fsm.AddTranslation(idleToRun);
            fsm.AddTranslation(runToJump);
            fsm.AddTranslation(runToIdle);
            fsm.AddTranslation(jumpToDoubleJump);
            fsm.AddTranslation(jumpToRun);
            fsm.AddTranslation(doubleJumpToRun);
            fsm.AddTranslation(dieTranslation);

            fsm.Start(idleState);
            
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.W)) {
                fsm.HandleEvent("w");
            }

            if (Input.GetKeyDown(KeyCode.S)) {
                fsm.HandleEvent("s");
            }

            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                fsm.HandleEvent("up");
            }

            if (Input.GetKeyDown(KeyCode.DownArrow)) {
                fsm.HandleEvent("down");
            }

            if (Input.GetKeyDown(KeyCode.D)) {
                fsm.HandleEvent("d");
            }
        }
    }
}

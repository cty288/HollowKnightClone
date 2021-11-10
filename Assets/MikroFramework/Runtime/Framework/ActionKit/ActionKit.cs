using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.ActionKit
{
    public class ActionKit {
        public void Play(MikroAction action) { 
            ActionPlayer.Singleton.Play(action);
        }




        #region Obsolete
        [Obsolete]
        /// <summary>
        /// Player that play actions
        /// </summary>
        private ActionPlayer actionPlayer = ActionPlayer.Singleton;
        [Obsolete]
        /// <summary>
        /// Timeline that record all timeline items
        /// </summary>
        private Timeline timeline = new Timeline();

        [Obsolete]
        /// <summary>
        /// Timer for the timeline
        /// </summary>
        private Timer timer = new Timer();

        [Obsolete]
        /// <summary>
        /// Record an action to the timeline
        /// </summary>
        /// <param name="startAtSeconds"></param>
        /// <param name="action"></param>
        public void AddAction(float startAtSeconds, MikroAction action)
        {
            timeline.Record(new Timepoint()
            {
                StartSeconds = startAtSeconds,
                Action = action
            });
        }

        [Obsolete]
        public void Play()
        {
            timer.StartTimer();
            timer.UpdateTimeData();

           UpdateActionExecutor.ActionKitUpdateTrigger.Singleton
                .OnUpdate += () => {
                timer.UpdateTimeData();

                List<Timepoint> calenderItems = timeline.GetAvailableCalenderItem(timer.CurrentSeconds);

                foreach (Timepoint calenderItem in calenderItems)
                {
                    actionPlayer.Play(calenderItem.Action);
                    timeline.CompleteCalenderItem(calenderItem);
                }
            };
        }
        #endregion

    }

    
}

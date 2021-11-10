using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MikroFramework.Pool;
using UnityEngine;

namespace MikroFramework.ActionKit
{
    public class Timeline:ActionContainer {
        private List<Timepoint> recordedTimepoints = new List<Timepoint>();
        private List<Timepoint> timePoints = new List<Timepoint>();

        private float startSeconds = 0;

       

        protected override void OnBegin() {
            startSeconds = Time.time;
           
        }

        public static Timeline Allocate() {
            return SafeObjectPool<Timeline>.Singleton.Allocate();
        }

        protected override void RecycleBackToPool() {
            SafeObjectPool<Timeline>.Singleton.Recycle(this);
        }

        public override IEnumerable ActionRecorder {
            get {
                return recordedTimepoints;
            }
        }

        public override void Update() {
            float currentSeconds = Time.time - startSeconds;

            Timepoint[] availableTimePoints =
                timePoints.Where(item => item.StartSeconds <= currentSeconds).ToArray();

            foreach (Timepoint availableTimePoint in availableTimePoints) {
                ActionPlayer.Singleton.Play(availableTimePoint.Action);
            }

            foreach (Timepoint availableTimePoint in availableTimePoints) {
                timePoints.Remove(availableTimePoint);
            }

            Finished.Value = timePoints.Count == 0;
        }

        public void AddAction(float startSeconds, MikroAction action) {
            Timepoint timepoint = Timepoint.Allocate();

            timepoint.StartSeconds = startSeconds;
            timepoint.Action = action;
            timepoint.Action.SetAutoRecycle(AutoRecycle);
            timePoints.Add(timepoint);
            recordedTimepoints.Add(timepoint);
        }

       
        protected override void SetAutoRecycleForRecordedActions(bool autoRecycle) {
            recordedTimepoints.ForEach(timepoint => { timepoint.Action.SetAutoRecycle(autoRecycle); });
        }

      

        public override void Reset() {
            base.Reset();
            recordedTimepoints.ForEach(timePoint => {
                timePoint.Action.Reset();
            });
            timePoints.Clear();
            timePoints.AddRange(recordedTimepoints);
            startSeconds = 0;
          
        }

        protected override void OnDispose() {
            recordedTimepoints.ForEach(timepoint => {
                timepoint.Action.RecycleToCache();
                    timepoint.RecycleToCache();
            });
            timePoints.Clear();
            recordedTimepoints.Clear();
            startSeconds = 0;
        }

        #region Obsoletes
        [Obsolete("This method is deprecated. Use AddAction() instead")]
        public void Record(Timepoint item)
        {
            timePoints.Add(item);
        }

        [Obsolete("This method is deprecated.")]
        public void CompleteCalenderItem(Timepoint item)
        {
            timePoints.Remove(item);
        }

        [Obsolete("This method is deprecated.")]
        public List<Timepoint> GetAvailableCalenderItem(float currentSeconds)
        {
            return timePoints.Where(item => item.StartSeconds <= currentSeconds).ToList();
        }
        #endregion

    }
}

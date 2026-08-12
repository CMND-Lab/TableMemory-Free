using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UXF;

namespace TableMemory
{
    public class TrackingLossTracker : Tracker
    {
        private int trackingLost = 0;
        public override string MeasurementDescriptor => "tracking_loss";

        public override IEnumerable<string> CustomHeader => new string[]
        {
            "tracking_loss"
        };
        // Start is called before the first frame update
        void Start()
        {
            InputTracking.trackingLost += OnTrackingLost;
            InputTracking.trackingAcquired += OnTrackingAcquired;
        }

        private void OnTrackingAcquired(XRNodeState obj)
        {
            if (obj.nodeType == XRNode.RightHand)
            {
                trackingLost = 0;
            }
        }

        // private void Update()
        // {
        //     if (trackingLost == 1)
        //     {
        //         Debug.Log(trackingLost.ToString());
        //     }
        // }

        private void OnTrackingLost(XRNodeState obj)
        {
            if (obj.nodeType == XRNode.RightHand)
            {
                trackingLost = 1;
            }
        }

        protected override UXFDataRow GetCurrentValues()
        {
            var values = new UXFDataRow()
            {
                ("tracking_loss", trackingLost.ToString())
            };

            return values;
        }
    }
}
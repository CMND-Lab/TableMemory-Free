using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UXF;

namespace TableMemory
{
    public class VelocityControllerTracker : Tracker
    {
        private UnityEngine.XR.InputDevice RightController;
        private Vector3 RightControllerVelocity;
        private Vector3 RightControllerAngularVelocity;
        public override string MeasurementDescriptor => "velocity";
        public override IEnumerable<string> CustomHeader => new string[]
        {
            "vel_x",
            "vel_y",
            "vel_z",
            "ang_vel_x",
            "ang_vel_y",
            "ang_vel_z"
        };
        private void Start()
                {
                    RightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
                }
        
        protected override UXFDataRow GetCurrentValues()
        {
            RightController.TryGetFeatureValue(CommonUsages.deviceVelocity, out RightControllerVelocity);
            RightController.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out RightControllerAngularVelocity);
            
            var values = new UXFDataRow()
            {
                ("vel_x", RightControllerVelocity.x),
                ("vel_y", RightControllerVelocity.y),
                ("vel_z", RightControllerVelocity.z),
                ("ang_vel_x", RightControllerAngularVelocity.x),
                ("ang_vel_y", RightControllerAngularVelocity.y),
                ("ang_vel_z", RightControllerAngularVelocity.z)
            };

            return values;
        }
    }
}
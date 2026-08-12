using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.XR;
using UXF;

namespace TableMemory
{
    public class PositionControllerTracker : Tracker
    {
        private UnityEngine.XR.InputDevice RightController;
        private Vector3 RightControllerPosition;
        private Quaternion RightControllerRotationQ;
        private Vector3 RightControllerRotation;
        public override string MeasurementDescriptor => "velocity";
        
        public override IEnumerable<string> CustomHeader => new string[]
        {
            "pos_x",
            "pos_y",
            "pos_z",
            "rot_x",
            "rot_y",
            "rot_z"
        };
        private void Start()
        {
            RightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        }

        
        
        
        protected override UXFDataRow GetCurrentValues()
        {
            RightController.TryGetFeatureValue(CommonUsages.devicePosition, out RightControllerPosition);
            RightController.TryGetFeatureValue(CommonUsages.deviceRotation, out RightControllerRotationQ);
            RightControllerRotation = RightControllerRotationQ.eulerAngles;
            
            var values = new UXFDataRow()
            {
                ("pos_x", RightControllerPosition.x),
                ("pos_y", RightControllerPosition.y),
                ("pos_z", RightControllerPosition.z),
                ("rot_x", RightControllerRotation.x),
                ("rot_y", RightControllerRotation.y),
                ("rot_z", RightControllerRotation.z)
            };

            return values;
        }
    }
}


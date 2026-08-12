using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Labs.SuperScience;
using UnityEngine;
using UnityEngine.XR;
using UXF;

namespace TableMemory
{
    public class SuperScienceTracker : Tracker
    {
        [SerializeField]
        Transform m_ToTrack;
        
        private PhysicsTracker m_MotionData = new PhysicsTracker();
        private Vector3 m_Velocity;
        private Vector3 m_Acceleration;
        private Vector3 m_AngularVelocity;
        private Vector3 m_AngularAcceleration;

        public override string MeasurementDescriptor => "super_science";
        
        public override IEnumerable<string> CustomHeader => new string[]
        {
            "vel_x",
            "vel_y",
            "vel_z",
            "acc_x",
            "acc_y",
            "acc_z",
            "ang_vel_x",
            "ang_vel_y",
            "ang_vel_z",
            "ang_acc_x",
            "ang_acc_y",
            "ang_acc_z"
        };
        private void Start()
        {
            var position = m_ToTrack.position;
            m_MotionData.Reset(position, m_ToTrack.rotation, Vector3.zero, Vector3.zero);
        }

        private void Update()
        {
            var position = m_ToTrack.position;
            m_MotionData.Update(position, m_ToTrack.rotation, Time.smoothDeltaTime);
            // Debug.Log(m_MotionData.Velocity.ToString());
            // Debug.Log(m_MotionData.AngularVelocity.ToString());
            // Debug.Log(m_MotionData.Acceleration.ToString());
            // Debug.Log(m_MotionData.AngularAcceleration.ToString());
        }

        
        
        
        
        protected override UXFDataRow GetCurrentValues()
        {
            Vector3 v = m_MotionData.Velocity;
            Vector3 w = m_MotionData.AngularVelocity;
            Vector3 a = m_MotionData.Acceleration;
            Vector3 q = m_MotionData.AngularAcceleration;
            
            var values = new UXFDataRow()
            {
                ("vel_x", v.x),
                ("vel_y", v.y),
                ("vel_z", v.z),
                ("acc_x", a.x),
                ("acc_y", a.y),
                ("acc_z", a.z),
                ("ang_vel_x", w.x),
                ("ang_vel_y", w.y),
                ("ang_vel_z", w.z),
                ("ang_acc_x", q.x),
                ("ang_acc_y", q.y),
                ("ang_acc_z", q.z),
            };

            return values;
        }
    }
}
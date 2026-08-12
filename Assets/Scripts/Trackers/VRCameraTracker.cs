using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

namespace TableMemory
{
    public class VRCameraTracker : Tracker
    {
        [SerializeField] 
        Transform ToTrack;
        
        public override string MeasurementDescriptor => "movement";

        public override IEnumerable<string> CustomHeader => new string[]
            {
                "pos_x",
                "pos_y",
                "pos_z",
                "rot_x",
                "rot_y",
                "rot_z"
            };
        
        
        protected override UXFDataRow GetCurrentValues()
        {
            Vector3 p = ToTrack.position;
            Quaternion qr = ToTrack.rotation;
            Vector3 r = qr.eulerAngles;
            
            var values = new UXFDataRow()
            {
                ("pos_x", p.x),
                ("pos_y", p.y),
                ("pos_z", p.z),
                ("rot_x", r.x),
                ("rot_y", r.y),
                ("rot_z", r.z)
            };

            return values;
        }

        
    }
}


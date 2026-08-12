using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

namespace TableMemory
{
    public class FPSTracker : Tracker
    {
        
        public override string MeasurementDescriptor => "fps";
            
        public override IEnumerable<string> CustomHeader => new string[]
        {
            "fps"
        };
        

        protected override UXFDataRow GetCurrentValues()
        {
            float fps = Time.unscaledDeltaTime;
            
            var values = new UXFDataRow()
            {
                ("fps", fps)
            };

            return values;
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.Community.StockFit.Base.Math.Structures
{
    public class TransformationMaxtrix
    {
        public RotationMatrix Rotation { get; set; }
        public Vector Translation { get; set; }
        public double Scale { get; set; }

        public TransformationMaxtrix(RotationMatrix rotation, Vector translation, double scale)
        {
            Rotation = rotation;
            Translation = translation;
            Scale = scale;
        }
    }
}

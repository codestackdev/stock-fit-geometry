//**********************
//Stock Fit Geometry
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

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

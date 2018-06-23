using CodeStack.Community.StockFit.Base.Math.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.Community.StockFit.Base.Math
{
    public interface IVectorMathService
    {
        Vector TransformVector(Vector vector, TransformationMaxtrix transform);

        Point TransformPoint(Point point, TransformationMaxtrix transform);

        TransformationMaxtrix GetTransformBetweenVectorsAroundPoint(
            Vector firstVector, Vector secondVector, Point point);
    }
}

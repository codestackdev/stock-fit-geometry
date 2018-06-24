//**********************
//Stock Fit Geometry
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

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

        TransformationMaxtrix InverseTransformationMatrix(TransformationMaxtrix transform);

        Point ProjectPointOnVector(Point pt, Vector vec, Point pointOnVec);
    }
}

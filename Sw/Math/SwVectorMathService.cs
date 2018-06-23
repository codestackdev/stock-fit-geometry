using CodeStack.Community.StockFit.Base.Math;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeStack.Community.StockFit.Base.Math.Structures;

namespace CodeStack.Community.StockFit.Sw.Math
{
    public class SwVectorMathService : IVectorMathService
    {
        private IMathUtility m_MathUtils;

        public SwVectorMathService(IMathUtility mathUtils)
        {
            m_MathUtils = mathUtils;
        }
        
        public Vector TransformVector(Vector vector, TransformationMaxtrix transform)
        {
            IMathVector vec = m_MathUtils.CreateVector(vector.ToArray()) as IMathVector;
            vec = vec.MultiplyTransform(transform) as IMathVector;
            return new Vector(vec.ArrayData as double[]);
        }

        public Point TransformPoint(Point point, TransformationMaxtrix transform)
        {
            IMathPoint mathPt = m_MathUtils.CreatePoint(point.ToArray()) as IMathPoint;
            var swTransform = m_MathUtils.CreateTransFormTransformationMaxtrix(transform);
            mathPt = mathPt.MultiplyTransform(transform) as IMathPoint;
            return new Point(mathPt.ArrayData as double[]);
        }

        public TransformationMaxtrix GetTransformBetweenVectorsAroundPoint(
            Vector firstVector, Vector secondVector, Point point)
        {
            IMathVector mathVec1 = m_MathUtils.CreateVector(firstVector.ToArray()) as IMathVector;
            IMathVector mathVec2 = m_MathUtils.CreateVector(secondVector.ToArray()) as IMathVector;
            IMathVector crossVec = mathVec1.Cross(mathVec2) as IMathVector;

            double dot = mathVec1.Dot(mathVec2);
            double vec1Len = mathVec1.GetLength();
            double vec2Len = mathVec2.GetLength();

            double angle = System.Math.Acos(dot / vec1Len * vec2Len);

            IMathPoint mathPt = m_MathUtils.CreatePoint(point.ToArray()) as IMathPoint;

            var mathTransform = m_MathUtils.CreateTransformRotateAxis(mathPt, crossVec, angle) as IMathTransform;

            return mathTransform.ToTransformationMaxtrix();
        }

        public TransformationMaxtrix InverseTransformationMatrix(TransformationMaxtrix transform)
        {
            var swTransform = m_MathUtils.CreateTransFormTransformationMaxtrix(transform);
            swTransform = swTransform.IInverse();
            return swTransform.ToTransformationMaxtrix();
        }
    }
}

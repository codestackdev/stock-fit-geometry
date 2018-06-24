//**********************
//Stock Fit Geometry
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using CodeStack.Community.StockFit.Base.Math.Structures;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolidWorks.Interop.sldworks
{
    public static class MathUtilityEx
    {
        public static IMathTransform CreateTransFormTransformationMaxtrix(
            this IMathUtility mathUtils, TransformationMaxtrix transform)
        {
            var arrData = new double[]
            {
                transform.Rotation.M11, transform.Rotation.M12, transform.Rotation.M13,
                transform.Rotation.M21, transform.Rotation.M22, transform.Rotation.M23,
                transform.Rotation.M31, transform.Rotation.M32, transform.Rotation.M33,
                transform.Translation.X, transform.Translation.Y, transform.Translation.Z,
                transform.Scale,
                0, 0, 0
            };

            return mathUtils.CreateTransform(arrData) as IMathTransform;
        }

        public static TransformationMaxtrix ToTransformationMaxtrix(this IMathTransform swTransform)
        {
            if (swTransform == null)
            {
                throw new ArgumentNullException(nameof(swTransform));
            }

            var matrixData = swTransform.ArrayData as double[];

            return new TransformationMaxtrix(
                new RotationMatrix(
                    matrixData[0], matrixData[1], matrixData[2],
                    matrixData[3], matrixData[4], matrixData[5],
                    matrixData[6], matrixData[7], matrixData[8]),
                    new Vector(matrixData[9], matrixData[10], matrixData[11]),
                    matrixData[12]);
        }
    }
}

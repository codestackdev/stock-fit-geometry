using CodeStack.Community.StockFit.Base.Math.Structures;
using CodeStack.Community.StockFit.Stocks.Cylinder;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.Community.StockFit.Sw
{
    internal class StockTools
    {
        private ISldWorks m_App;

        internal StockTools(ISldWorks app)
        {
            m_App = app;
        }

        internal void CreateCylindricalBody(IPartDoc part, CylinderParams cylParams)
        {
            var cylTempBody = m_App.IGetModeler().CreateBodyFromCyl(new double[]
                    {
                        cylParams.Origin.X, cylParams.Origin.Y, cylParams.Origin.Z,
                        cylParams.Axis.X, cylParams.Axis.Y, cylParams.Axis.Z,
                        cylParams.Radius, cylParams.Height
                    }) as IBody2;

            IFeature feat = part.CreateFeatureFromBody3(cylTempBody, false,
                (int)swCreateFeatureBodyOpts_e.swCreateFeatureBodySimplify) as IFeature;

            IBody2 body = feat.GetBody() as IBody2;

            body.MaterialPropertyValues2 = new double[] { 1, 1, 0, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5 };

        }

        internal IBody2 GetBodyToProcess(IPartDoc part, object inputObj)
        {
            if (inputObj is IFace2)
            {
                return (inputObj as IFace2).GetBody() as IBody2;
            }
            else
            {
                object[] solidBodies = part.GetBodies2((int)swBodyType_e.swSolidBody, true) as object[];

                if (solidBodies != null && solidBodies.Length == 1)
                {
                    return solidBodies[0] as IBody2;
                }
            }

            throw new NullReferenceException("Failed to find the input body. Either select cylindrical face or use single body part");
        }

        internal Vector GetDirection(IModelDoc2 model, object inputObj)
        {
            if (inputObj is IFace2)
            {
                ISurface surf = (inputObj as IFace2).GetSurface() as ISurface;

                if (surf.IsCylinder())
                {
                    double[] cylParams = surf.CylinderParams as double[];

                    return new Vector(cylParams[3], cylParams[4], cylParams[5]);
                }
            }
            else if (inputObj is IFeature)
            {
                IRefPlane refPlane = (inputObj as IFeature).GetSpecificFeature2() as IRefPlane;

                if (refPlane != null)
                {
                    IMathUtility mathUtils = m_App.GetMathUtility() as IMathUtility;
                    IMathVector vec = mathUtils.CreateVector(new double[] { 0, 0, 1 }) as IMathVector;
                    vec = vec.MultiplyTransform(refPlane.Transform) as IMathVector;

                    return new Vector(vec.ArrayData as double[]);
                }
            }

            throw new NullReferenceException("Failed to find the direction. Please select cylindrical face or plane");
        }
    }
}

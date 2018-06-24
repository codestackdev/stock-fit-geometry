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
    public interface ISwRoundStockTool
    {
        IBody2 CreateCylindricalStock(IPartDoc part, object inputObj);
        IBody2 GetScopeBody(IPartDoc part, object inputObj);
    }

    public class RoundStockTool : ISwRoundStockTool
    {
        private ISldWorks m_App;
        private CylindricalStockFitExtractor m_CylExt;

        public RoundStockTool(ISldWorks app, CylindricalStockFitExtractor cylExt)
        {
            m_App = app;
            m_CylExt = cylExt;
        }

        public IBody2 CreateCylindricalStock(IPartDoc part, object inputObj)
        {
            var body = GetScopeBody(part, inputObj);

            var dir = GetDirection(inputObj);

            var cylParams = m_CylExt.GetStockParameters(new SolidBodyGeometry(body), dir);

            return CreateCylindricalBody(part, cylParams);
        }

        private IBody2 CreateCylindricalBody(IPartDoc part, CylinderParams cylParams)
        {
            var cylTempBody = m_App.IGetModeler().CreateBodyFromCyl(new double[]
                    {
                        cylParams.Origin.X, cylParams.Origin.Y, cylParams.Origin.Z,
                        cylParams.Axis.X, cylParams.Axis.Y, cylParams.Axis.Z,
                        cylParams.Radius, cylParams.Height
                    }) as IBody2;

            return cylTempBody;
        }

        public IBody2 GetScopeBody(IPartDoc part, object inputObj)
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

        private Vector GetDirection(object inputObj)
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

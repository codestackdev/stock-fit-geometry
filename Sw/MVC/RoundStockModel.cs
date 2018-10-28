//**********************
//Stock Fit Geometry
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using CodeStack.Community.StockFit.Base.Math.Structures;
using CodeStack.Community.StockFit.Stocks.Cylinder;
using CodeStack.Community.StockFit.Sw;
using CodeStack.Community.StockFit.Sw.Options;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.Community.StockFit.MVC
{
    public class RoundStockModel
    {
        private readonly ISldWorks m_App;
        private readonly IModeler m_Modeler;
        private readonly CylindricalStockFitExtractor m_CylExt;

        public RoundStockModel(ISldWorks app, CylindricalStockFitExtractor cylExt)
        {
            m_App = app;
            m_Modeler = app.IGetModeler();
            m_CylExt = cylExt;
        }

        public IBody2 CreateCylindricalStock(CylinderParams cylParams)
        {
            return m_Modeler.CreateCylinder(new SwEx.MacroFeature.Data.Point(cylParams.Origin.ToArray()),
                new SwEx.MacroFeature.Data.Vector(cylParams.Axis.ToArray()),
                cylParams.Radius, cylParams.Height);
        }

        public CylinderParams GetCylinderParameters(IPartDoc part, object inputObj, bool concentric, double step)
        {
            CylinderParams cylParams;
            var body = GetScopeBody(part, inputObj);

            var dir = GetDirection(inputObj);

            cylParams = m_CylExt.GetStockParameters(new SolidBodyGeometry(body), dir);

            if (concentric && (inputObj is IFace2) && (inputObj as IFace2).IGetSurface().IsCylinder())
            {
                var surfCylParams = (inputObj as IFace2).IGetSurface().CylinderParams as double[];
                var pt = new Point(surfCylParams[0], surfCylParams[1], surfCylParams[2]);
                cylParams = m_CylExt.ReCenter(dir, pt, cylParams);
            }

            if (step > 0)
            {
                cylParams = m_CylExt.FitToStockSizeByStep(cylParams, step);
            }

            return cylParams;
        }

        private IBody2 m_TempBody;

        public void ShowPreview(IPartDoc part, object inputObj, bool concentric, double step)
        {
            HidePreview(part);

            try
            {
                var cylParams = GetCylinderParameters(part, inputObj, concentric, step);
                m_TempBody = CreateCylindricalStock(cylParams);
            }
            catch
            {
            }

            if (m_TempBody != null)
            {
                const int COLORREF_YELLOW = 65535;

                m_TempBody.Display3(part, COLORREF_YELLOW,
                    (int)swTempBodySelectOptions_e.swTempBodySelectOptionNone);

                m_TempBody.MaterialPropertyValues2 = new double[] { 1, 1, 0, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5 };

                (part as IModelDoc2).GraphicsRedraw2();
            }
        }

        public void HidePreview(IPartDoc part)
        {
            if (m_TempBody != null)
            {
                m_TempBody.Hide(part);
                m_TempBody = null;
                GC.Collect();
            }
        }

        //private IBody2 CreateBody(RoundStockFeatureParameters par, out Exception err)
        //{
        //    err = null;

        //    try
        //    {
        //        CreateCylindricalStock()
        //        CylinderParams cylParams;
        //        var step = m_Setts.StockSteps.FirstOrDefault(s => s.Key == par.StockStep).Value;

        //        return m_StockTool.CreateCylindricalStock(m_Part, par.Direction,
        //            par.ConcenticWithCylindricalFace, step, out cylParams);
        //    }
        //    catch (Exception ex)
        //    {
        //        err = ex;

        //        return null;
        //    }
        //}

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

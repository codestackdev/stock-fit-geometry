//**********************
//Stock Fit Geometry
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using CodeStack.Community.StockFit.Base.Math.Structures;
using CodeStack.Community.StockFit.MVC;
using CodeStack.Community.StockFit.Stocks.Cylinder;
using CodeStack.Community.StockFit.Sw.MVC;
using CodeStack.Community.StockFit.Sw.Options;
using CodeStack.Community.StockFit.Sw.Pmp;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CodeStack.Community.StockFit.Sw
{
    [ComVisible(true)]
    [Guid("47827004-8897-49F5-9C65-5B845DC7F5AC")]
    [ProgId(Id)]
    public class RoundStockMacroFeature : ISwComFeature
    {
        public const string Id = "CodeStack.StockMacroFeature";

        public object Edit(object app, object modelDoc, object feature)
        {
            var featData = (feature as IFeature).GetDefinition() as IMacroFeatureData;

            if (featData.AccessSelections(modelDoc, null))
            {
                try
                {
                    var param = GetParameters(feature as IFeature);

                    var ctrl = ServicesContainer.Instance.GetService<RoundStockController>();

                    ctrl.Process(modelDoc as IPartDoc, param, feature as IFeature);

                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public object Regenerate(object app, object modelDoc, object feature)
        {
            try
            {
                var stockModel = ServicesContainer.Instance.GetService<RoundStockModel>();

                var param = GetParameters(feature as IFeature);
                
                CylinderParams cylParams;

                var setts = ServicesContainer.Instance.GetService<RoundStockFeatureSettings>();

                var step = setts.StockSteps.FirstOrDefault(s => s.Key == param.StockStep).Value;

                var body = stockModel.CreateCylindricalStock(
                    modelDoc as IPartDoc, param.Direction,
                    param.ConcenticWithCylindricalFace, step, out cylParams);

                //temp
                SetProperties(modelDoc, param, cylParams);
                //

                var featData = (feature as IFeature).GetDefinition() as IMacroFeatureData;
                
                var dispDims = featData.GetDisplayDimensions() as object[];

                if (dispDims != null && dispDims.Length == 2)
                {
                    SetDimensions(app as ISldWorks, featData.CurrentConfiguration.Name,
                        cylParams, dispDims.Cast<IDisplayDimension>().ToArray());
                }
                else
                {
                    throw new NullReferenceException("Failed to get display dimensions");
                }

                if (param.CreateSolidBody)
                {
                    UpdateBodyEntitiesIds(body, featData);

                    return body;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private static void UpdateBodyEntitiesIds(IBody2 body, IMacroFeatureData featData)
        {
            object faces;
            object edges;
            featData.GetEntitiesNeedUserId(body, out faces, out edges);

            if (faces is object[])
            {
                int nextId = 0;

                foreach (Face2 face in faces as object[])
                {
                    featData.SetFaceUserId(face, nextId++, 0);
                }
            }

            if (edges is object[])
            {
                int nextId = 0;

                foreach (Edge edge in edges as object[])
                {
                    featData.SetEdgeUserId(edge, nextId++, 0);
                }
            }
        }

        private void SetDimensions(ISldWorks app, string confName,
            CylinderParams cylParams, IDisplayDimension[] dispDims)
        {
            var mathUtil = app.IGetMathUtility();

            var radDiam = dispDims[0] as IDisplayDimension;
            var heightDiam = dispDims[1] as IDisplayDimension;

            var dummyPt = mathUtil.CreatePoint(new double[3]) as IMathPoint;

            var heightDirVec = mathUtil.CreateVector(cylParams.Axis.ToArray()) as MathVector;
            var startPt = mathUtil.CreatePoint(cylParams.Origin.ToArray()) as IMathPoint;
            var endPt = MovePoint(startPt, heightDirVec, cylParams.Height);
            
            MathVector diamDirVec = null;

            var yVec = new Vector(0, 1, 0);
            if (cylParams.Axis.IsSame(yVec))
            {
                var xVec = new double[] { 1, 0, 0 };
                diamDirVec = mathUtil.CreateVector(xVec) as MathVector;
            }
            else
            {
                diamDirVec = (mathUtil.CreateVector(yVec.ToArray()) as MathVector).Cross(heightDirVec) as MathVector;
            }

            var diamExtVec = diamDirVec.Cross(heightDirVec) as MathVector;

            var circlePt = MovePoint(startPt, diamDirVec, cylParams.Radius);
            
            SetAndReleaseDimension(radDiam, diamDirVec, diamExtVec, 
                new IMathPoint[]
                {
                    startPt, circlePt, dummyPt
                }, cylParams.Radius, confName);

            SetAndReleaseDimension(heightDiam, heightDirVec, diamDirVec,
                new IMathPoint[]
                {
                    startPt, endPt, dummyPt
                }, cylParams.Height, confName);
        }

        private static IMathPoint MovePoint(IMathPoint pt, MathVector dir, double dist)
        {
            var moveVec = dir.Normalise().Scale(dist);

            return pt.AddVector(moveVec) as IMathPoint;
        }

        private void SetAndReleaseDimension(IDisplayDimension dispDim, MathVector dimDir,
            MathVector extDir, IMathPoint[] refPts, double val, string confName)
        {
            var dim = dispDim.GetDimension2(0);
            dim.DrivenState = (int)swDimensionDrivenState_e.swDimensionDriven;
            dim.ReadOnly = true;

            dim.SetSystemValue3(val,
                (int)swSetValueInConfiguration_e.swSetValue_InSpecificConfigurations,
                new string[] { confName });

            dim.DimensionLineDirection = dimDir;
            dim.ExtensionLineDirection = extDir;
            dim.ReferencePoints = refPts;

            //NOTE: releasing the pointers as unreleased pointer might cause crash
            Marshal.ReleaseComObject(dim);
            Marshal.ReleaseComObject(dispDim);
            dim = null;
            dispDim = null;
            GC.Collect();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private static void SetProperties(object modelDoc, RoundStockFeatureParameters param, CylinderParams cylParams)
        {
            var metersToInch = new Func<double, double>((m) => m * 39.37007874);

            var model = modelDoc as IModelDoc2;
            var activeConf = model.ConfigurationManager.ActiveConfiguration.Name;

            model.SetPropertyValue("StockVisible", Convert.ToInt32(param.CreateSolidBody).ToString(), activeConf);
            model.SetPropertyValue("StockDiameter", metersToInch(cylParams.Radius * 2).ToString(), activeConf);
            model.SetPropertyValue("StockLength", metersToInch(cylParams.Height).ToString(), activeConf);
        }

        private RoundStockFeatureParameters GetParameters(IFeature feat)
        {
            var featData = (feat as IFeature).GetDefinition() as IMacroFeatureData;

            object paramNames = null;
            object paramValues = null;
            object paramTypes = null;

            featData.GetParameters(out paramNames, out paramTypes, out paramValues);

            object selObj;
            object selObjType;
            object selMarks;
            object selDrViews;
            object compXforms;

            featData.GetSelections3(out selObj, out selObjType, out selMarks, out selDrViews, out compXforms);

            var param = new RoundStockFeatureParameters();

            param.CreateSolidBody = Convert.ToBoolean(
                GetParameterValue(paramNames, paramValues, 
                nameof(RoundStockFeatureParameters.CreateSolidBody)));

            param.ConcenticWithCylindricalFace = Convert.ToBoolean(
                GetParameterValue(paramNames, paramValues,
                nameof(RoundStockFeatureParameters.ConcenticWithCylindricalFace)));

            param.StockStep = GetParameterValue(paramNames, paramValues,
                nameof(RoundStockFeatureParameters.StockStep));

            if (selObj is object[] && (selObj as object[]).Length > 0)
            {
                param.Direction = (selObj as object[]).First();
            }
            else
            {
                throw new NullReferenceException("Referenced entity is missing");
            }

            return param;
        }

        private string GetParameterValue(object paramNames, object paramValues, string name)
        {
            if (!(paramNames is string[]))
            {
                throw new ArgumentNullException(nameof(paramNames));
            }

            if (!(paramValues is string[]))
            {
                throw new ArgumentNullException(nameof(paramValues));
            }

            var paramNamesList = (paramNames as string[]).ToList();

            var index = paramNamesList.IndexOf(name);

            if (index != -1)
            {
                var paramValsArr = paramValues as string[];

                if (paramValsArr.Length > index)
                {
                    return paramValsArr[index];
                }
                else
                {
                    throw new IndexOutOfRangeException($"Parameter {name} doesn't have a value");
                }
            }
            else
            {
                throw new IndexOutOfRangeException($"Failed to read parameter {name}");
            }
        }

        public object Security(object app, object modelDoc, object feature)
        {
            return (int)swMacroFeatureSecurityOptions_e.swMacroFeatureSecurityByDefault;
        }
    }
}

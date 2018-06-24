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
    public class StockMacroFeatureService : ISwComFeature
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

                    var ctrl = ServicesContainer.Instance.GetService<StockFeaturePageController>();

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
                var param = GetParameters(feature as IFeature);

                var stockTool = ServicesContainer.Instance.GetStockTool();

                var body = stockTool.CreateCylindricalStock(modelDoc as IPartDoc, param.Direction);

                if (param.CreateSolidBody)
                {
                    return body;
                }
            }
            catch(Exception ex)
            {
                return ex.Message;
            }

            return true;
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

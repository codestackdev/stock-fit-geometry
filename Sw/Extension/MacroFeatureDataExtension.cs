//**********************
//Stock Fit Geometry
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SolidWorks.Interop.sldworks
{
    /// <summary>
    /// Attribute decorates public property to indicate that this parameter needs to be retrieved from macro feature selection
    /// (i.e. <see cref="IMacroFeatureData.GetSelections3(out object, out object, out object, out object, out object)"/> method
    /// </summary>
    /// <remarks>This attribute is extracted in <see cref="MacroFeatureDataExtension.DeserializeParameters{TParams}(IMacroFeatureData)"/>
    /// and <see cref="MacroFeatureDataExtension.SerializeParameters{TParams}(IMacroFeatureData, TParams)"/> methods</remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class MacroFeatureParameterSelectionAttribute : System.Attribute
    {
        /// <summary>
        /// Index of the selection in the selection array
        /// </summary>
        public int SelectionIndex { get; private set; }

        public MacroFeatureParameterSelectionAttribute(int selIndex)
        {
            SelectionIndex = selIndex;
        }
    }

    public static class MacroFeatureDataExtension
    {
        /// <summary>
        /// Serializes the parameters into macro feature native parameters
        /// </summary>
        /// <typeparam name="TParams">Parameters type to serialize</typeparam>
        /// <param name="featData">Pointer to feature data</param>
        /// <param name="parameters">Pointer to parameters</param>
        /// <exception cref="NullReferenceException"/>
        /// <exception cref="NotSupportedException"/>
        /// <exception cref="IndexOutOfRangeException"/>
        public static void SerializeParameters<TParams>(this IMacroFeatureData featData, TParams parameters)
            where TParams : class, new()
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Deserializes the parameters from macro feature native parameters
        /// </summary>
        /// <typeparam name="TParams">Parameters type to deserialize</typeparam>
        /// <param name="featData">Pointer to feature data</param>
        /// <returns>Pointer to parameters</returns>
        /// <exception cref="NullReferenceException"/>
        /// <exception cref="NotSupportedException"/>
        /// <exception cref="IndexOutOfRangeException"/>
        public static TParams DeserializeParameters<TParams>(this IMacroFeatureData featData)
            where TParams : class, new()
        {
            object retParamNames = null;
            object retParamValues = null;
            object paramTypes = null;
            object retSelObj;
            object selObjType;
            object selMarks;
            object selDrViews;
            object compXforms;

            featData.GetParameters(out retParamNames, out paramTypes, out retParamValues);
            featData.GetSelections3(out retSelObj, out selObjType, out selMarks, out selDrViews, out compXforms);

            var paramNames = retParamNames as string[];
            var paramValues = retParamValues as string[];
            var selObjects = retSelObj as object[];

            var resParams = new TParams();

            foreach (var prp in resParams.GetType().GetProperties())
            {
                var prpType = prp.PropertyType;

                object val = null;

                if (prpType.IsPrimitive || prpType == typeof(string))
                {
                    var selAtt = prp.TryGetAttribute<MacroFeatureParameterSelectionAttribute>();

                    if (selAtt != null)
                    {
                        var selIndex = selAtt.SelectionIndex;

                        if (selObjects != null && selObjects.Length > selIndex)
                        {
                            val = selObjects[selIndex];
                        }
                        else
                        {
                            throw new NullReferenceException($"Referenced entity is missing at index {selIndex} for {prpType.Name}");
                        }
                    }
                    else
                    {
                        var paramVal = GetParameterValue(paramNames, paramValues, prp.Name);
                        val = Convert.ChangeType(paramVal, prpType);
                        
                    }
                }
                else
                {
                    throw new NotSupportedException(
                        $"{prp.Name} is not supported as the parameter of macro feature. Currently only primitive types and string are supported");
                }

                prp.SetValue(resParams, val, null);
            }
            
            return resParams;
        }

        private static string GetParameterValue(string[] paramNames, string[] paramValues, string name)
        {
            if (!(paramNames is string[]))
            {
                throw new ArgumentNullException(nameof(paramNames));
            }

            if (!(paramValues is string[]))
            {
                throw new ArgumentNullException(nameof(paramValues));
            }

            var paramNamesList = paramNames.ToList();

            var index = paramNamesList.IndexOf(name);

            if (index != -1)
            {
                if (paramValues.Length > index)
                {
                    return paramValues[index];
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
    }
}

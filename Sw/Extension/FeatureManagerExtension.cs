//**********************
//Stock Fit Geometry
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SolidWorks.Interop.sldworks
{
    /// <summary>
    /// Dimension data to add to macro feature
    /// </summary>
    public class MacroFeatureDimension
    {
        private static readonly swDimensionType_e[] m_SupportedTypes = new swDimensionType_e[]
        {
            swDimensionType_e.swAngularDimension,
            swDimensionType_e.swLinearDimension,
            swDimensionType_e.swRadialDimension
        };

        public swDimensionType_e Type { get; private set; }
        public double Value { get; private set; }
        
        public MacroFeatureDimension(swDimensionType_e type, double value)
        {
            if (!m_SupportedTypes.Contains(type))
            {
                throw new NotSupportedException($"Dimension {type} is not supported");
            }

            Type = type;
            Value = value;
        }
    }

    /// <summary>
    /// Icons information to add to macro feature
    /// </summary>
    public class MacroFeatureIcons
    {
        public string Regular { get; private set; }
        public string Suppressed { get; private set; }
        public string Highlighted { get; private set; }

        public MacroFeatureIcons(string regular, string suppressed, string highlighted)
        {
            Regular = regular;
            Suppressed = suppressed;
            Highlighted = highlighted;
        }

        public MacroFeatureIcons(string icon) : this(icon, icon, icon)
        {
        }
    }

    public static class FeatureManagerExtension
    {
        /// <summary>
        /// Inserts new macro feature
        /// </summary>
        /// <typeparam name="TParams">Type of parameters to serialize to macro feature</typeparam>
        /// <param name="featMgr">Pointer to Feature manager</param>
        /// <param name="baseName">Base name to be used for newly generated feature. Name will be automatically appended by the next available index</param>
        /// <param name="progId">COM Prog id of the macro feature</param>
        /// <param name="parameters">Parameters to serialize to macro feature</param>
        /// <param name="dims">Dimensions data to associate with macro feature</param>
        /// <param name="icons">Icons data for macro feature</param>
        /// <param name="options">Options</param>
        /// <returns>Newly created feature</returns>
        public static IFeature InsertComFeature<TParams>(this IFeatureManager featMgr,
            string baseName, string progId, TParams parameters, MacroFeatureDimension[] dims, 
            MacroFeatureIcons icons, swMacroFeatureOptions_e options)
            where TParams : class, new()
        {
            string[] paramNames;
            int[] paramTypes;
            string[] paramValues;
            DispatchWrapper[] selection;

            MacroFeatureDataExtension.ParseParameters<TParams>(parameters,
                out paramNames, out paramTypes, out paramValues, out selection);

            var dimTypes = dims.Select(d => (int)d.Type).ToArray();
            var dimValues = dims.Select(d => d.Value).ToArray();

            var iconsArr = new string[] { icons.Regular, icons.Suppressed, icons.Highlighted };

            using (var selSet = new SelectionSet(featMgr.Document.ISelectionManager))
            {
                if (selection.Any())
                {
                    var selRes = selSet.AddRange(selection);

                    Debug.Assert(selRes);
                }

                return featMgr.InsertMacroFeature3(baseName,
                    progId, null, paramNames, paramTypes,
                    paramValues, dimTypes, dimValues, null, iconsArr, (int)options) as IFeature;
            }
        }
    }
}

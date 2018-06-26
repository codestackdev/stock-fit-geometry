//**********************
//Stock Fit Geometry
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/stock-fit-geometry/blob/master/LICENSE
//**********************

using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.Community.StockFit.Sw.CommandManager.Attributes
{
    [Flags]
    public enum swWorkspaceTypes_e
    {
        NoDocuments = 1,
        Part = 2 << 0,
        Assembly = 2 << 1,
        Drawing = 2 << 2,
        AllDocuments = Part | Assembly | Drawing,
        All = AllDocuments | NoDocuments
    }

    public class CommandItemInfoAttribute : Attribute
    {
        public swCommandItemType_e MenuToolbarVisibility { get; private set; }
        public swWorkspaceTypes_e SupportedWorkspaces { get; private set; }

        public CommandItemInfoAttribute(swCommandItemType_e itemType, swWorkspaceTypes_e suppWorkspaces)
        {
            MenuToolbarVisibility = itemType;
            SupportedWorkspaces = suppWorkspaces;
        }
    }
}

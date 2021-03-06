// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpec software and documentation
// --
// --  ERTMSFormalSpec is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpec is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DataDictionary.Constants;
using Enum = DataDictionary.Types.Enum;

namespace GUI.DataDictionaryView
{
    public class EnumerationValuesTreeNode : EnumerationTreeNode
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public EnumerationValuesTreeNode(Enum item, bool buildSubNodes)
            : base(item, buildSubNodes, "Values", true, false)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="subNodes"></param>
        /// <param name="recursive">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(List<BaseTreeNode> subNodes, bool recursive)
        {
            // Do not use the base version
            SubNodesBuilt = true;

            foreach (EnumValue value in Item.Values)
            {
                subNodes.Add(new EnumerationValueTreeNode(value, recursive));
            }
            subNodes.Sort();
        }

        public void AddEnumValueHandler(object sender, EventArgs args)
        {
            Item.appendValues(EnumValue.CreateDefault(Item.Values));
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem> {new MenuItem("Add", AddEnumValueHandler)};

            return retVal;
        }
    }
}
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
using DataDictionary.Generated;
using GUI.SpecificationView;
using Enum = DataDictionary.Types.Enum;
using Paragraph = DataDictionary.Specification.Paragraph;

namespace GUI.DataDictionaryView
{
    public class SubEnumerationsTreeNode : ModelElementTreeNode<Enum>
    {
        private class ItemEditor : NamedEditor
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public SubEnumerationsTreeNode(Enum item, bool buildSubNodes)
            : base(item, buildSubNodes, "Enumerations", true)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="subNodes"></param>
        /// <param name="recursive">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(List<BaseTreeNode> subNodes, bool recursive)
        {
            base.BuildSubNodes(subNodes, recursive);

            foreach (Enum enumeration in Item.SubEnums)
            {
                subNodes.Add(new EnumerationTreeNode(enumeration, recursive));
            }
            subNodes.Sort();
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor CreateEditor()
        {
            return new ItemEditor();
        }

        public void AddHandler(object sender, EventArgs args)
        {
            DataDictionaryTreeView treeView = BaseTreeView as DataDictionaryTreeView;
            if (treeView != null)
            {
                Enum enumeration = (Enum) acceptor.getFactory().createEnum();
                enumeration.Name = "<Enumeration" + (GetNodeCount(false) + 1) + ">";
                Item.appendSubEnums(enumeration);
            }
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem> {new MenuItem("Add", AddHandler)};

            return retVal;
        }

        /// <summary>
        ///     Accepts drop of a tree node, in a drag & drop operation
        /// </summary>
        /// <param name="sourceNode"></param>
        public override void AcceptDrop(BaseTreeNode sourceNode)
        {
            base.AcceptDrop(sourceNode);

            if (sourceNode is EnumerationTreeNode)
            {
                EnumerationTreeNode enumerationTreeNode = sourceNode as EnumerationTreeNode;
                Enum enumeration = enumerationTreeNode.Item;

                enumerationTreeNode.Delete();
                Item.appendSubEnums(enumeration);
            }
            else if (sourceNode is ParagraphTreeNode)
            {
                ParagraphTreeNode node = sourceNode as ParagraphTreeNode;
                Paragraph paragraph = node.Item;

                Enum enumeration = Enum.CreateDefault(Item.SubEnums);
                Item.appendSubEnums(enumeration);
                enumeration.FindOrCreateReqRef(paragraph);
            }
        }
    }
}
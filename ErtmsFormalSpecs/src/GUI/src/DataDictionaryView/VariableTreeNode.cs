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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Functions;
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using GUI.Converters;
using GUI.Properties;
using GUI.StateDiagram;
using WeifenLuo.WinFormsUI.Docking;
using Dictionary = DataDictionary.Dictionary;
using Function = DataDictionary.Functions.Function;
using Parameter = DataDictionary.Parameter;
using StateMachine = DataDictionary.Types.StateMachine;
using Type = DataDictionary.Types.Type;
using Variable = DataDictionary.Variables.Variable;

namespace GUI.DataDictionaryView
{
    public class VariableTreeNode : ReqRelatedTreeNode<Variable>
    {
        /// <summary>
        ///     The editor for Train data variables
        /// </summary>
        private class ItemEditor : ReqRelatedEditor
        {
            [Category("Description")]
            public override string Name
            {
                get { return base.Name; }
                set { base.Name = value; }
            }

            /// <summary>
            ///     The variable type
            /// </summary>
            [Category("Description")]
            [Editor(typeof (TypeUITypedEditor), typeof (UITypeEditor))]
            [TypeConverter(typeof (TypeUITypeConverter))]
            // ReSharper disable once UnusedMember.Local
            public Variable Type
            {
                get { return Item; }
                set
                {
                    Item = value;
                    RefreshNode();
                }
            }

            /// <summary>
            ///     The variable default value
            /// </summary>
            [Category("Description")]
            [Editor(typeof (DefaultValueUITypedEditor), typeof (UITypeEditor))]
            [TypeConverter(typeof (DefaultValueUITypeConverter))]
            // ReSharper disable once UnusedMember.Local
            public Variable DefaultValue
            {
                get { return Item; }
                set
                {
                    Item = value;
                    RefreshNode();
                }
            }

            /// <summary>
            ///     The variable mode
            /// </summary>
            [Category("Description"), TypeConverter(typeof (VariableModeConverter))]
            // ReSharper disable once UnusedMember.Local
            public acceptor.VariableModeEnumType Mode
            {
                get { return Item.getVariableMode(); }
                set { Item.setVariableMode(value); }
            }

            /// <summary>
            ///     The variable value
            /// </summary>
            [Category("Description")]
            [Editor(typeof (VariableValueUITypedEditor), typeof (UITypeEditor))]
            [TypeConverter(typeof (VariableValueUITypeConverter))]
            // ReSharper disable once UnusedMember.Local
            public Variable Value
            {
                get { return Item; }
                set
                {
                    Item = value;
                    RefreshNode();
                }
            }

            [Category("Display")]
            public Size Size
            {
                get { return new Size(Item.Width, Item.Height); }
                set
                {
                    Item.Width = value.Width;
                    Item.Height = value.Height;

                }
            }

            [Category("Display")]
            public Point Location
            {
                get { return new Point(Item.X, Item.Y); }
                set
                {
                    Item.X = value.X;
                    Item.Y = value.Y;
                }
            }

            [Category("Display")]
            public bool Hidden
            {
                get { return Item.Hidden; }
                set { Item.Hidden = value; }
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        /// <param name="encounteredTypes">the types that have already been encountered in the path to create this variable </param>
        public VariableTreeNode(Variable item, bool buildSubNodes, HashSet<Type> encounteredTypes)
            : base(item, buildSubNodes)
        {
            encounteredTypes.Add(item.Type);
            encounteredTypes.Remove(item.Type);
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name"></param>
        /// <param name="encounteredTypes">the types that have already been encountered in the path to create this variable </param>
        /// <param name="buildSubNodes"></param>
        public VariableTreeNode(Variable item, bool buildSubNodes, string name, HashSet<Type> encounteredTypes)
            : base(item, buildSubNodes, name, false)
        {
            encounteredTypes.Add(item.Type);
            encounteredTypes.Remove(item.Type);
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor CreateEditor()
        {
            return new ItemEditor();
        }

        /// <summary>
        ///     Display the associated state diagram
        /// </summary>
        public void ViewDiagram()
        {
            if (Item.Type is StateMachine)
            {
                StateDiagramWindow window = new StateDiagramWindow();
                GuiUtils.MdiWindow.AddChildWindow(window);
                window.StatePanel.SetStateMachine(Item);
                window.Text = Item.Name + @" " + Resources.VariableTreeNode_ViewDiagram_state_diagram;
            }
        }

        protected void ViewStateDiagramHandler(object sender, EventArgs args)
        {
            ViewDiagram();
        }


        protected override ModelElement FindOrCreateUpdate()
        {
            ModelElement retVal = null;

            Dictionary dictionary = GetPatchDictionary();
            if (dictionary != null)
            {
                retVal = dictionary.FindByFullName(Item.FullName) as ModelElement;
                if (retVal == null)
                {
                    // If the element does not already exist in the patch, add a copy to it
                    retVal = Item.CreateVariableUpdate(dictionary);
                }
                // Navigate to the element, whether it was created or not
                EfsSystem.Instance.Context.SelectElement(retVal, this, Context.SelectionCriteria.DoubleClick);
            }

            return retVal;
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            MenuItem updateItem = new MenuItem("Update...");
            updateItem.MenuItems.Add(new MenuItem("Update", AddUpdate));
            updateItem.MenuItems.Add(new MenuItem("Remove", RemoveInUpdate));
            retVal.Add(updateItem);
            retVal.Add(new MenuItem("Delete", DeleteHandler));
            retVal.AddRange(base.GetMenuItems());

            Function function = Item.Value as Function;
            if (function != null)
            {
                InterpretationContext context = new InterpretationContext(Item);
                if (function.FormalParameters.Count == 1)
                {
                    Parameter parameter = (Parameter) function.FormalParameters[0];
                    Graph graph = function.CreateGraph(context, parameter, null);
                    if (graph != null && graph.Segments.Count != 0)
                    {
                        retVal.Insert(6, new MenuItem("Display", DisplayHandler));
                    }
                }
                else if (function.FormalParameters.Count == 2)
                {
                    Surface surface = function.CreateSurface(context, null);
                    if (surface != null && surface.Segments.Count != 0)
                    {
                        retVal.Insert(6, new MenuItem("Display", DisplayHandler));
                    }
                }
            }
            else
            {
                retVal.Insert(5, new MenuItem("-"));
                retVal.Insert(6, new MenuItem("Display", DisplayHandler));
            }

            if (Item.Type is StateMachine)
            {
                retVal.Insert(5, new MenuItem("-"));
                retVal.Insert(6, new MenuItem("View state diagram", ViewStateDiagramHandler));
            }

            return retVal;
        }

        /// <summary>
        ///     Displays the variable value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void DisplayHandler(object sender, EventArgs args)
        {
            Function function = Item.Value as Function;
            if (function != null)
            {
                GraphView.GraphView view = new GraphView.GraphView();
                GuiUtils.MdiWindow.AddChildWindow(view);
                view.Functions.Add(function);
                view.Refresh();
            }
            else
            {
                StructureEditor.Window window = new StructureEditor.Window();
                window.SetVariable(Item);

                if (GuiUtils.MdiWindow.DataDictionaryWindow != null)
                {
                    GuiUtils.MdiWindow.AddChildWindow(window);
                    window.Show(GuiUtils.MdiWindow.DataDictionaryWindow.Pane, DockAlignment.Right, 0.20);
                }
            }
        }
    }
}
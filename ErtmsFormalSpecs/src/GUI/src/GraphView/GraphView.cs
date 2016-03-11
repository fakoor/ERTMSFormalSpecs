// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpecs software and documentation
// --
// --  ERTMSFormalSpecs is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpecs is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using DataDictionary;
using DataDictionary.Functions;
using DataDictionary.Interpreter;
using GUI.DataDictionaryView;
using GUI.Properties;
using GUI.Shortcuts;
using GUIUtils.GraphVisualization.Functions;
using GUIUtils.GraphVisualization.Graphs;
using Utils;
using WeifenLuo.WinFormsUI.Docking;
using Function = DataDictionary.Functions.Function;
using Graph = DataDictionary.Functions.Graph;
using Util = DataDictionary.Util;

namespace GUI.GraphView
{
    public partial class GraphView : BaseForm
    {
        /// <summary>
        ///     The functions to be displayed in this graph view
        /// </summary>
        public List<Function> Functions { get; set; }

        /// <summary>
        /// The train position
        /// </summary>
        public TrainPosition TrainPosition { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public GraphView()
        {
            InitializeComponent();

            AllowDrop = true;
            DragEnter += GraphView_DragEnter;
            DragDrop += GraphView_DragDrop;

            Functions = new List<Function>();

            TrainPosition = new TrainPosition();

            DockAreas = DockAreas.Document;

            GraphVisualiser.InitializeChart();

            foreach (TabPage tabPage in tabControl1.TabPages)
            {
                tabPage.MouseEnter += (s, e) => tabPage.Focus();
            }
        }

        private void GraphView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void GraphView_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("WindowsForms10PersistentObject", false))
            {
                BaseTreeNode sourceNode = e.Data.GetData("WindowsForms10PersistentObject") as BaseTreeNode;
                if (sourceNode != null)
                {
                    FunctionTreeNode functionTreeNode = sourceNode as FunctionTreeNode;
                    if (functionTreeNode != null)
                    {
                        AddFunction(functionTreeNode.Item, null);
                    }
                    else
                    {
                        ShortcutTreeNode shortcutTreeNode = sourceNode as ShortcutTreeNode;
                        if (shortcutTreeNode != null)
                        {
                            AddFunction(shortcutTreeNode.Item.GetReference() as Function, null);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Adds a new function to this graph
        /// </summary>
        /// <param name="function"></param>
        /// <param name="explain"></param>
        private void AddFunction(Function function, ExplanationPart explain)
        {
            if (function != null)
            {
                InterpretationContext context = new InterpretationContext(function);
                if (function.FormalParameters.Count == 1)
                {
                    Parameter parameter = (Parameter) function.FormalParameters[0];
                    Graph graph = function.CreateGraph(context, parameter, explain);
                    if (graph != null)
                    {
                        Functions.Add(function);
                        Refresh();
                    }
                }
                else if (function.FormalParameters.Count == 2)
                {
                    Surface surface = function.CreateSurface(context, explain);
                    if (surface != null)
                    {
                        Functions.Add(function);
                        Refresh();
                    }
                    else
                    {
                        MessageBox.Show(
                            Resources.GraphView_AddFunction_Cannot_add_this_function_to_the_display_view,
                            Resources.GraphView_AddFunction_Cannot_display_function,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        ///     Allows to refresh the view, according to the fact that the structure for the model could change
        /// </summary>
        public override void Refresh()
        {
            Display();
        }

        /// <summary>
        ///     Indicates that a change event should be displayed
        /// </summary>
        /// <param name="modelElement"></param>
        /// <param name="changeKind"></param>
        /// <returns></returns>
        protected override bool ShouldDisplayChange(IModelElement modelElement, Context.ChangeKind changeKind)
        {
            bool retVal = modelElement == null || DisplayedModel == null || DisplayedModel.IsParent(modelElement);

            return retVal;
        }

        /// <summary>
        ///     Allows to refresh the view, when the value of a model changed
        /// </summary>
        /// <param name="modelElement"></param>
        /// <param name="changeKind"></param>
        /// <returns>True if the view should be refreshed</returns>
        public override bool HandleValueChange(IModelElement modelElement, Context.ChangeKind changeKind)
        {
            bool retVal = base.HandleValueChange(modelElement, changeKind);

            if (retVal)
            {
                Refresh();
            }

            return retVal;
        }

        /// <summary>
        ///     Displays the graph
        /// </summary>
        /// <returns></returns>
        public void Display()
        {
            Util.DontNotify(() =>
            {
                GraphVisualiser.Reset();
                String name = null;

                // Computes the expected end to display
                double expectedEndX = 0;
                Dictionary<Function, Graph> graphs = new Dictionary<Function, Graph>();
                foreach (Function function in Functions)
                {
                    InterpretationContext context = new InterpretationContext(function);
                    if (function.FormalParameters.Count == 1)
                    {
                        Parameter parameter = (Parameter) function.FormalParameters[0];
                        Graph graph = function.CreateGraph(context, parameter, null);
                        if (graph != null)
                        {
                            expectedEndX = Math.Max(expectedEndX, graph.ExpectedEndX());
                            graphs.Add(function, graph);
                        }
                    }
                }

                double expectedEndY = 0;
                Dictionary<Function, Surface> surfaces = new Dictionary<Function, Surface>();
                foreach (Function function in Functions)
                {
                    InterpretationContext context = new InterpretationContext(function);
                    if (function.FormalParameters.Count == 2)
                    {
                        Surface surface = function.CreateSurface(context, null);
                        if (surface != null)
                        {
                            expectedEndX = Math.Max(expectedEndX, surface.ExpectedEndX());
                            expectedEndY = Math.Max(expectedEndY, surface.ExpectedEndY());
                            surfaces.Add(function, surface);
                        }
                    }
                }

                try
                {
                    int maxX = Int32.Parse(Tb_MaxX.Text);
                    expectedEndX = Math.Min(expectedEndX, maxX);
                }
                catch (Exception)
                {
                }

                // Creates the graphs
                foreach (KeyValuePair<Function, Graph> pair in graphs)
                {
                    Function function = pair.Key;
                    Graph graph = pair.Value;

                    if (function != null && graph != null)
                    {
                        EfsProfileFunction efsProfileFunction = new EfsProfileFunction(graph);
                        if (function.Name.Contains("Gradient"))
                        {
                            GraphVisualiser.AddGraph(new EfsGradientProfileGraph(GraphVisualiser, efsProfileFunction,
                                function.FullName));
                        }
                        else
                        {
                            GraphVisualiser.AddGraph(new EfsProfileFunctionGraph(GraphVisualiser, efsProfileFunction,
                                function.FullName));
                        }

                        if (name == null)
                        {
                            name = function.Name;
                        }
                    }
                }

                // Creates the surfaces
                foreach (KeyValuePair<Function, Surface> pair in surfaces)
                {
                    Function function = pair.Key;
                    Surface surface = pair.Value;

                    if (surface != null)
                    {
                        EfsSurfaceFunction efsSurfaceFunction = new EfsSurfaceFunction(surface);
                        GraphVisualiser.AddGraph(new EfsSurfaceFunctionGraph(GraphVisualiser, efsSurfaceFunction,
                            function.FullName));
                        if (name == null)
                        {
                            name = function.Name;
                        }
                    }
                }

                Train train = new Train (GraphVisualiser);
                train.InitializeTrain(TrainPosition.GetDistance(), TrainPosition.GetSpeed(), TrainPosition.GetUnderReadingAmount(), TrainPosition.GetOverReadingAmount());
                GraphVisualiser.AddGraph (train);
                GraphVisualiser.Annotations.Add (train.TrainLineAnnotation);
                GraphVisualiser.Annotations.Add (train.TrainAnnotation);

                if (name != null)
                {
                    try
                    {
                        double val = double.Parse(Tb_MinX.Text);
                        GraphVisualiser.SetMinX(val);
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        double val = double.Parse(Tb_MaxX.Text);
                        GraphVisualiser.SetMaxX(val);
                    }
                    catch (Exception)
                    {
                    }

                    if (Cb_AutoYSize.Checked)
                    {
                        GraphVisualiser.SetMaxY(double.NaN);
                    }
                    else
                    {
                        double height;
                        if (double.TryParse(Tb_MaxY.Text, out height))
                        {
                            GraphVisualiser.SetMaxY(height);
                        }
                        else
                        {
                            GraphVisualiser.SetMaxY(double.NaN);
                        }
                    }

                    GraphVisualiser.SetMinY2 (double.NaN);
                    GraphVisualiser.SetMaxY2 (double.NaN);
                    double top, bottom;
                    if (double.TryParse (Tb_MinGrad.Text, out bottom))
                    {
                        GraphVisualiser.SetMinY2 (bottom);
                    }
                    if(double.TryParse (Tb_MaxGrad.Text, out top))
                    {
                        GraphVisualiser.SetMaxY2 (top);
                    }

                    GraphVisualiser.DrawGraphs(expectedEndX);
                }
            });
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        /// <summary>
        ///     Handles the zoom on the chart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GraphVisualiser_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                Chart chart = sender as Chart;
                if (chart != null)
                {
                    if (e.Location.X >= 0 && e.Location.X <= chart.Size.Width &&
                        e.Location.Y >= 0 && e.Location.Y <= chart.Size.Height)
                    {
                        GraphVisualiser.HandleZoom(sender, e, 0);
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Handles Cb_AutoYSize check change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cb_AutoYSize_CheckedChanged(object sender, EventArgs e)
        {
            Lbl_MaxY.Enabled = !Cb_AutoYSize.Checked;
            Tb_MaxY.Enabled = !Cb_AutoYSize.Checked;
            ValueChanged(sender, e);
        }
    }
}
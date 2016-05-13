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
using System.Collections;
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Tests;
using GUIUtils;
using Reports.Tests;

namespace GUI.Report
{
    public partial class TestReport : Form
    {
        private readonly TestsCoverageReportHandler _reportHandler;

        /// <summary>
        ///     Constructor
        /// </summary>
        public TestReport()
        {
            InitializeComponent();
            _reportHandler = new TestsCoverageReportHandler((Dictionary) null);
            TxtB_Path.Text = _reportHandler.FileName;
        }


        /// <summary>
        ///     Constructor: creates a report for the dictionary
        /// </summary>
        /// <param name="aDictionary"></param>
        public TestReport(Dictionary aDictionary)
        {
            InitializeComponent();
            _reportHandler = new TestsCoverageReportHandler(aDictionary) {Dictionary = aDictionary};
            InitializeCheckBoxes(1);
            CB_ActivatedRulesInSteps.Checked = false;
            CB_ActivatedRulesInSteps.Enabled = false;
            TxtB_Path.Text = _reportHandler.FileName;
        }


        /// <summary>
        ///     Constructor: creates a report for the selected frame
        /// </summary>
        /// <param name="aFrame"></param>
        public TestReport(Frame aFrame)
        {
            InitializeComponent();
            _reportHandler = new TestsCoverageReportHandler(aFrame.Dictionary) {Frame = aFrame};
            InitializeCheckBoxes(1);
            TxtB_Path.Text = _reportHandler.FileName;
        }


        /// <summary>
        ///     Constructor: creates a report for the selected sub sequence
        /// </summary>
        /// <param name="aSubSequence"></param>
        public TestReport(SubSequence aSubSequence)
        {
            InitializeComponent();
            _reportHandler = new TestsCoverageReportHandler(aSubSequence.Dictionary) {SubSequence = aSubSequence};
            InitializeCheckBoxes(2);
            TxtB_Path.Text = _reportHandler.FileName;
        }


        /// <summary>
        ///     Consctructor: creates a report for a selected test case
        /// </summary>
        /// <param name="aTestCase"></param>
        public TestReport(TestCase aTestCase)
        {
            InitializeComponent();
            _reportHandler = new TestsCoverageReportHandler(aTestCase.Dictionary) {TestCase = aTestCase};
            InitializeCheckBoxes(3);
            TxtB_Path.Text = _reportHandler.FileName;
        }


        /// <summary>
        ///     Gives the list of all the controls of the form
        ///     (situated on the main form or on its group boxes)
        /// </summary>
        public ArrayList AllControls
        {
            get
            {
                ArrayList retVal = new ArrayList();

                retVal.AddRange(Controls);
                retVal.AddRange(GrB_Filters.Controls);

                return retVal;
            }
        }


        /// <summary>
        ///     Initialize the check boxes of the form (by (un)checking
        ///     or (de)activating them) according to user selection
        /// </summary>
        /// <param name="level"></param>
        private void InitializeCheckBoxes(int level)
        {
            foreach (Control control in AllControls)
            {
                if (control is CheckBox)
                {
                    CheckBox cb = control as CheckBox;
                    string[] tags = cb.Tag.ToString().Split('.');
                    string cbProperty = tags[0]; /* the property can be either FILTER (frame, sub sequence, ...
                                                  * either STAT (for a given filter, do we have to show its ativated rules ?..) */
                    int cbLevel;
                    Int32.TryParse(tags[1], out cbLevel);
                    if (cbLevel < level) /* for example, if we create a report for a sub sequence, we
                                          * will deselect the "frame" check box */
                    {
                        cb.Enabled = false;
                    }
                    else if (cbLevel == level || cbProperty.Equals("FILTER"))
                        /* we select all the filters of the level >= current level
                                                                               * and the stats of the current level */
                    {
                        cb.Enabled = true;
                        cb.Checked = true;
                    }
                    else
                    {
                        cb.Enabled = true;
                    }
                }
            }
        }


        /// <summary>
        ///     Method called in case of check event of one of the check boxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null)
            {
                string[] tags = cb.Tag.ToString().Split('.');
                string cbProperty = tags[0];
                if (cbProperty.Equals("FILTER"))
                {
                    int cbLevel;
                    Int32.TryParse(tags[1], out cbLevel);
                    if (cb.Checked)
                    {
                        SelectCheckBoxes(cbLevel); /* we enable all the check boxes of the selected level */
                    }
                    else
                    {
                        DeselectCheckBoxes(cbLevel); /* we disable the statistics check boxes of the selected level */
                    }
                }
            }
        }


        /// <summary>
        ///     Enables all the check boxes of the selected level
        ///     and the check box corresponding to the filter of selected level + 1
        /// </summary>
        /// <param name="level">Level of the checked check box</param>
        private void SelectCheckBoxes(int level)
        {
            foreach (Control control in AllControls)
            {
                if (control is CheckBox)
                {
                    CheckBox cb = control as CheckBox;
                    string[] tags = cb.Tag.ToString().Split('.');
                    string cbProperty = tags[0];
                    int cbLevel;
                    Int32.TryParse(tags[1], out cbLevel);
                    if ((cbLevel == level && cbProperty.Equals("STAT")) ||
                        (cbLevel == level + 1 && cbProperty.Equals("FILTER")))
                    {
                        if ((cb.Name != "CB_ActivatedRulesInSteps" &&
                             cb.Name != "CB_Log") ||
                            (_reportHandler.Frame != null ||
                             _reportHandler.SubSequence != null ||
                             _reportHandler.TestCase != null))
                        {
                            cb.Enabled = true;
                        }
                    }
                }
            }
        }


        /// <summary>
        ///     Disables the check boxes corresponding to the statistics of the selected level
        /// </summary>
        /// <param name="level">Level of the unckecked check box</param>
        private void DeselectCheckBoxes(int level)
        {
            foreach (Control control in AllControls)
            {
                if (control is CheckBox)
                {
                    CheckBox cb = control as CheckBox;
                    string[] tags = cb.Tag.ToString().Split('.');
                    string cbProperty = tags[0];
                    int cbLevel;
                    Int32.TryParse(tags[1], out cbLevel);
                    if (cbLevel > level || (cbLevel == level && !cbProperty.Equals("FILTER")))
                    {
                        cb.Checked = false;
                        cb.Enabled = false;
                    }
                }
            }
        }


        /// <summary>
        ///     Creates a report config with user's choices
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_CreateReport_Click(object sender, EventArgs e)
        {
            _reportHandler.Name = "Tests coverage report";

            _reportHandler.AddFrames = CB_Frames.Checked;
            _reportHandler.AddActivatedRulesInFrames = CB_ActivatedRulesInFrames.Checked;

            _reportHandler.AddSubSequences = CB_SubSequences.Checked;
            _reportHandler.AddActivatedRulesInSubSequences = CB_ActivatedRulesInSubSequences.Checked;

            _reportHandler.AddTestCases = CB_TestCases.Checked;
            _reportHandler.AddActivatedRulesInTestCases = CB_ActivatedRulesInTestCases.Checked;

            _reportHandler.AddSteps = CB_Steps.Checked;
            _reportHandler.AddActivatedRulesInSteps = CB_ActivatedRulesInSteps.Checked;

            Hide();

            ReportUtil.CreateReport(Owner, _reportHandler);
        }

        /// <summary>
        ///     Permits to select the name and the path of the report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_SelectFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
            };
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                _reportHandler.FileName = saveFileDialog.FileName;
                TxtB_Path.Text = _reportHandler.FileName;
            }
        }
    }
}
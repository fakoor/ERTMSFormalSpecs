﻿// ------------------------------------------------------------------------------
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
using System.Windows.Forms;

namespace GUI.RuleDisabling
{
    public partial class Frm_SelectRule : Form
    {
        public DataDictionary.RuleCheck.RuleChecksEnum? SelectedRule = null;

        public Frm_SelectRule()
        {
            InitializeComponent();

            CBB_DisableableRuleChecks.DropDownStyle = ComboBoxStyle.DropDownList;
            CBB_DisableableRuleChecks.Items.AddRange(Enum.GetNames(typeof(DataDictionary.RuleCheck.DisableableRuleChecksEnum)));
        }

        private void Btn_Select_Click(object sender, EventArgs e)
        {
            DataDictionary.RuleCheck.RuleChecksEnum value;
            if (Enum.TryParse(CBB_DisableableRuleChecks.SelectedItem.ToString(), out value))
            {
                SelectedRule = value;
            }
            else
            {
                SelectedRule = null;
            }
            Hide();
        }
    }
}

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

using DataDictionary;
using GUIUtils.LongOperations;

namespace GUI.LongOperations
{
    /// <summary>
    ///     Refreshes the model
    /// </summary>
    public class RefreshModel : BaseLongOperation
    {
        public override void ExecuteWork()
        {
            EfsSystem.Instance.Context.HandleChangeEvent(null, Context.ChangeKind.ModelChange);
        }

        /// <summary>
        /// Executes a refresh model
        /// </summary>
        public static void Execute()
        {
            RefreshModel refreshModel = new RefreshModel();
            refreshModel.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Refreshing model", false);
        }
    }
}
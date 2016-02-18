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

using System.Windows.Forms;
using DataDictionary;

namespace GUIUtils.LongOperations
{
    public class RefactorAndRelocateOperation : BaseLongOperation
    {
        /// <summary>
        ///     The element to be refactored
        /// </summary>
        private ModelElement Model { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="model"></param>
        public RefactorAndRelocateOperation(ModelElement model)
        {
            Model = model;
        }

        /// <summary>
        ///     Generates the file in the background thread
        /// </summary>
        public override void ExecuteWork()
        {
            EfsSystem.Instance.Compiler.Compile_Synchronous(false, true);
            EfsSystem.Instance.Compiler.RefactorAndRelocate(Model);
        }

        /// <summary>
        ///     Executes the operation in background using a progress handler
        /// </summary>
        /// <param name="mainForm">The enclosing form</param>
        /// <param name="message">The message to display on the dialog window</param>
        /// <param name="allowCancel">Indicates that the opeation can be canceled</param>
        public override void ExecuteUsingProgressDialog(Form mainForm, string message, bool allowCancel = true)
        {
            base.ExecuteUsingProgressDialog(mainForm, message, allowCancel);

            // Long operations do not notify the listeners. 
            // Update the entire model
            EfsSystem.Instance.Context.HandleChangeEvent(null);
        }
    }
}
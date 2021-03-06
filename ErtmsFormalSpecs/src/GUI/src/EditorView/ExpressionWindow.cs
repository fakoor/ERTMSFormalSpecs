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
using Function = DataDictionary.Functions.Function;

namespace GUI.EditorView
{
    /// <summary>
    ///     An expression editor window
    /// </summary>
    public class ExpressionWindow : Window
    {
        protected override string EditorName
        {
            get { return "Expression editor"; }
        }

        /// <summary>
        ///     Allows to refresh the view, when the selected model changed
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true if refresh should be performed</returns>
        public override bool HandleSelectionChange(Context.SelectionContext context)
        {
            bool retVal = base.HandleSelectionChange(context);

            DisplayedModel = context.Element;

            IEditable editable = DisplayedModel as IEditable;
            if (editable != null)
            {
                SyntaxHighlight = editable.SyntaxHightlight;
                AutoComplete = editable.SyntaxHightlight;
                setChangeHandler(new EditableTextChangeHandler(editable));

            }
            else
            {
                IExpressionable expressionable = DisplayedModel as IExpressionable;
                if (expressionable != null && !(expressionable is Function))
                {
                    SyntaxHighlight = true;
                    AutoComplete = true;
                    setChangeHandler(new ExpressionableTextChangeHandler((ModelElement) expressionable));
                }
                else
                {
                    setChangeHandler(null);
                }
            }

            return retVal;
        }
    }
}
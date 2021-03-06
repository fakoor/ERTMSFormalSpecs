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

namespace GUIUtils.LongOperations
{
    public class CheckDeadModelOperation : BaseLongOperation
    {
        /// <summary>
        ///     Checks for dead model
        /// </summary>
        public override void ExecuteWork()
        {
            MarkingHistory.PerformMark(() =>
            {
                foreach (Dictionary dictionary in EfsSystem.Instance.Dictionaries)
                {
                    dictionary.CheckDeadModel();
                }
            });
        }
    }
}
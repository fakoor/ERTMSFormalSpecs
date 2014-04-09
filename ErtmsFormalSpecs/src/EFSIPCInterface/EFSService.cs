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
namespace EFSIPCInterface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DataDictionary.Tests.Runner;
    using DataDictionary;
    using System.ServiceModel;

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class EFSService : IEFSService
    {
        /// <summary>
        /// Provides the runner on which the service is applied
        /// </summary>
        private Runner Runner
        {
            get
            {
                EFSSystem efsSystem = EFSSystem.INSTANCE;
                if (efsSystem.Runner == null)
                {
                    bool explain = false;
                    bool logEvents = true;
                    efsSystem.Runner = new Runner(explain, logEvents, 100, 10000);
                }

                return efsSystem.Runner;
            }
        }

        /// <summary>
        /// Provides the value of a specific variable
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public Value GetVariableValue(string variableName)
        {
            Value retVal = null;

            DataDictionary.Variables.IVariable variable = EFSSystem.INSTANCE.findByFullName(variableName) as DataDictionary.Variables.IVariable;
            if (variable != null)
            {
                retVal = convertOut(variable.Value);
            }

            return retVal;
        }

        /// <summary>
        /// Converts a DataDictionary.Values.IValue into an EFSIPCInterface.Value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private Value convertOut(DataDictionary.Values.IValue value)
        {
            // Handles the boolean case
            {
                DataDictionary.Values.BoolValue v = value as DataDictionary.Values.BoolValue;
                if (v != null)
                {
                    return new BoolValue(v.Val);
                }
            }

            // Handles the integer case
            {
                DataDictionary.Values.IntValue v = value as DataDictionary.Values.IntValue;
                if (v != null)
                {
                    return new IntValue(v.Val);
                }
            }

            // Handles the double case
            {
                DataDictionary.Values.DoubleValue v = value as DataDictionary.Values.DoubleValue;
                if (v != null)
                {
                    return new DoubleValue(v.Val);
                }
            }

            // Handles the string case
            {
                DataDictionary.Values.StringValue v = value as DataDictionary.Values.StringValue;
                if (v != null)
                {
                    return new StringValue(v.Val);
                }
            }

            // Handles the list case
            {
                DataDictionary.Values.ListValue v = value as DataDictionary.Values.ListValue;
                if (v != null)
                {
                    List<Value> list = new List<Value>();

                    foreach (DataDictionary.Values.IValue item in v.Val)
                    {
                        list.Add(convertOut(item));
                    }

                    return new ListValue(list);
                }
            }

            // Handles the structure case
            {
                DataDictionary.Values.StructureValue v = value as DataDictionary.Values.StructureValue;
                if (v != null)
                {
                    Dictionary<string, Value> record = new Dictionary<string, Value>();

                    foreach (KeyValuePair<string, Utils.INamable> pair in v.Val)
                    {
                        DataDictionary.Variables.IVariable variable = pair.Value as DataDictionary.Variables.IVariable;
                        if (variable != null)
                        {
                            record.Add(variable.Name, convertOut(variable.Value));
                        }
                    }

                    return new StructureValue(record);
                }
            }

            return null;
        }

        /// <summary>
        /// Sets the value of a specific variable
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="value"></param>
        public void SetVariableValue(string variableName, Value value)
        {
        }

        /// <summary>
        /// Activates the execution of a single cycle, as the given priority level
        /// </summary>
        /// <param name="priority"></param>
        public void Cycle(Priority priority)
        {

            Runner.ExecuteOnePriority(convertPriority(priority));
        }

        /// <summary>
        /// Converts an interface priority to a Runner priority
        /// </summary>
        /// <param name="priority"></param>
        private DataDictionary.Generated.acceptor.RulePriority convertPriority(Priority priority)
        {
            DataDictionary.Generated.acceptor.RulePriority retVal = DataDictionary.Generated.acceptor.RulePriority.defaultRulePriority;

            switch (priority)
            {
                case Priority.Verification:
                    retVal = DataDictionary.Generated.acceptor.RulePriority.aVerification;
                    break;

                case Priority.UpdateInternal:
                    retVal = DataDictionary.Generated.acceptor.RulePriority.aUpdateINTERNAL;
                    break;

                case Priority.Process:
                    retVal = DataDictionary.Generated.acceptor.RulePriority.aProcessing;
                    break;

                case Priority.UpdateOutput:
                    retVal = DataDictionary.Generated.acceptor.RulePriority.aUpdateOUT;
                    break;

                case Priority.CleanUp:
                    retVal = DataDictionary.Generated.acceptor.RulePriority.aCleanUp;
                    break;
            }

            return retVal;
        }
    }
}

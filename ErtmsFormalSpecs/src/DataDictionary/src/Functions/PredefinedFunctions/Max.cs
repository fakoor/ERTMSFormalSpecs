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

using System.Collections.Generic;
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Values;
using DataDictionary.Variables;

namespace DataDictionary.Functions.PredefinedFunctions
{
    /// <summary>
    ///     Creates a new function which describes the maximum value of two functions
    /// </summary>
    public class Max : FunctionOnGraph
    {
        /// <summary>
        ///     The first parameter
        /// </summary>
        public Parameter First { get; private set; }

        /// <summary>
        ///     The second parameter
        /// </summary>
        public Parameter Second { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="efsSystem"></param>
        public Max(EfsSystem efsSystem)
            : base(efsSystem, "MAX")
        {
            First = (Parameter) acceptor.getFactory().createParameter();
            First.Name = "First";
            First.Type = EFSSystem.AnyType;
            First.setFather(this);
            FormalParameters.Add(First);

            Second = (Parameter) acceptor.getFactory().createParameter();
            Second.Name = "Second";
            Second.Type = EFSSystem.AnyType;
            Second.setFather(this);
            FormalParameters.Add(Second);
        }

        /// <summary>
        ///     Perform additional checks based on the parameter types
        /// </summary>
        /// <param name="root">The element on which the errors should be reported</param>
        /// <param name="context">The evaluation context</param>
        /// <param name="actualParameters">The parameters applied to this function call</param>
        public override void AdditionalChecks(ModelElement root, InterpretationContext context,
            Dictionary<string, Expression> actualParameters)
        {
            CheckFunctionalParameter(root, context, actualParameters[First.Name], 1);
            CheckFunctionalParameter(root, context, actualParameters[Second.Name], 1);

            Function function1 = actualParameters[First.Name].GetExpressionType() as Function;
            Function function2 = actualParameters[Second.Name].GetExpressionType() as Function;

            if (function1 != null && function2 != null)
            {
                if (function1.FormalParameters.Count == 1 && function2.FormalParameters.Count == 1)
                {
                    Parameter p1 = (Parameter) function1.FormalParameters[0];
                    Parameter p2 = (Parameter) function2.FormalParameters[0];

                    if (p1.Type != p2.Type && p1.Type != EFSSystem.DoubleType && p2.Type != EFSSystem.DoubleType)
                    {
                        root.AddError("The formal parameters for the functions provided as parameter are not the same");
                    }
                }

                if (function1.ReturnType != function2.ReturnType && function1.ReturnType != EFSSystem.DoubleType &&
                    function2.ReturnType != EFSSystem.DoubleType)
                {
                    root.AddError("The return values for the functions provided as parameter are not the same");
                }
            }
        }

        /// <summary>
        ///     Provides the graph of this function if it has been statically defined
        /// </summary>
        /// <param name="context">the context used to create the graph</param>
        /// <param name="parameter"></param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public override Graph CreateGraph(InterpretationContext context, Parameter parameter, ExplanationPart explain)
        {
            Graph retVal = null;

            IValue firstValue = context.FindOnStack(First).Value;
            IValue secondValue = context.FindOnStack(Second).Value;

            Graph firstGraph = createGraphForValue(context, firstValue, explain, parameter);
            if (firstGraph != null)
            {
                Graph secondGraph = createGraphForValue(context, secondValue, explain, parameter);
                if (secondGraph != null)
                {
                    retVal = firstGraph.Max(secondGraph) as Graph;
                }
                else
                {
                    Second.AddError("Cannot create graph for " + Second);
                }
            }
            else
            {
                First.AddError("Cannot create graph for " + First);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the value of the function
        /// </summary>
        /// <param name="context"></param>
        /// <param name="actuals">the actual parameters values</param>
        /// <param name="explain"></param>
        /// <returns>The value for the function application</returns>
        public override IValue Evaluate(InterpretationContext context, Dictionary<Actual, IValue> actuals,
            ExplanationPart explain)
        {
            IValue retVal;

            int token = context.LocalScope.PushContext();
            AssignParameters(context, actuals);

            Function function = (Function) acceptor.getFactory().createFunction();
            function.Name = "MAX (" + getName(First) + ", " + getName(Second) + ")";
            function.Enclosing = EFSSystem;
            Parameter parameter = (Parameter) acceptor.getFactory().createParameter();
            parameter.Name = "X";
            parameter.Type = EFSSystem.DoubleType;
            function.appendParameters(parameter);
            function.ReturnType = EFSSystem.DoubleType;
            function.Graph = CreateGraph(context, parameter, explain);

            retVal = function;
            context.LocalScope.PopContext(token);

            return retVal;
        }
    }
}
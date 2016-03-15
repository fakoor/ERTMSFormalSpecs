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
using DataDictionary.Functions;
using DataDictionary.Interpreter.Filter;
using DataDictionary.RuleCheck;
using DataDictionary.Types;
using DataDictionary.Values;
using DataDictionary.Variables;
using Utils;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Interpreter
{
    public class StabilizeExpression : Expression, ISubDeclarator
    {
        /// <summary>
        ///     The expression to stabilize
        /// </summary>
        public Expression Expression { get; private set; }

        /// <summary>
        ///     The initial value for the stabilisation algorithm
        /// </summary>
        public Expression InitialValue { get; private set; }

        /// <summary>
        ///     The condition which indicates that the stabilization is complete
        /// </summary>
        public Expression Condition { get; private set; }

        /// <summary>
        ///     The value of the last iteration
        /// </summary>
        private IVariable LastIteration { get; set; }

        /// <summary>
        ///     The value of the current iteration
        /// </summary>
        private IVariable CurrentIteration { get; set; }

        /// <summary>
        ///     The list of all values iterated through. Checks for cycles.
        /// </summary>
        private List<IValue> IterationsList { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="root"></param>
        /// <param name="log"></param>
        /// <param name="expression">The expression to stabilize</param>
        /// <param name="initialValue">The initial value for this stabilisation computation</param>
        /// <param name="condition">The condition which indicates that the stabilisation is not complete</param>
        /// <param name="parsingData">Additional information about the parsing process</param>
        public StabilizeExpression(ModelElement root, ModelElement log, Expression expression, Expression initialValue,
            Expression condition, ParsingData parsingData)
            : base(root, log, parsingData)
        {
            Expression = SetEnclosed(expression);
            InitialValue = SetEnclosed(initialValue);
            Condition = SetEnclosed(condition);

            LastIteration = CreateBoundVariable("PREVIOUS", null);
            CurrentIteration = CreateBoundVariable("CURRENT", null);

            InitDeclaredElements();
        }

        /// <summary>
        ///     Initialises the declared elements
        /// </summary>
        public void InitDeclaredElements()
        {
            DeclaredElements = new Dictionary<string, List<INamable>>();

            ISubDeclaratorUtils.AppendNamable(this, LastIteration);
            ISubDeclaratorUtils.AppendNamable(this, CurrentIteration);
        }

        /// <summary>
        ///     The elements declared by this declarator
        /// </summary>
        public Dictionary<string, List<INamable>> DeclaredElements { get; private set; }

        /// <summary>
        ///     Appends the INamable which match the name provided in retVal
        /// </summary>
        /// <param name="name"></param>
        /// <param name="retVal"></param>
        public void Find(string name, List<INamable> retVal)
        {
            ISubDeclaratorUtils.Find(this, name, retVal);
        }

        /// <summary>
        ///     Performs the semantic analysis of the expression
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <param name="expectation">Indicates the kind of element we are looking for</param>
        /// <returns>True if semantic analysis should be continued</returns>
        public override bool SemanticAnalysis(INamable instance, BaseFilter expectation)
        {
            bool retVal = base.SemanticAnalysis(instance, expectation);

            if (retVal)
            {
                // InitialValue
                if (InitialValue != null)
                {
                    InitialValue.SemanticAnalysis(instance, IsRightSide.INSTANCE);
                    StaticUsage.AddUsages(InitialValue.StaticUsage, Usage.ModeEnum.Read);

                    LastIteration.Type = InitialValue.GetExpressionType();
                    CurrentIteration.Type = InitialValue.GetExpressionType();
                    StaticUsage.AddUsage(InitialValue.GetExpressionType(), Root, Usage.ModeEnum.Type);
                }
                else
                {
                    AddError("Initial value not provided", RuleChecksEnum.SemanticAnalysisError);
                }

                // Expression
                if (Expression != null)
                {
                    Expression.SemanticAnalysis(instance, AllMatches.INSTANCE);
                    StaticUsage.AddUsages(Expression.StaticUsage, Usage.ModeEnum.Read);
                }
                else
                {
                    Root.AddError("Accumulator expression value not provided");
                }

                // Condition
                if (Condition != null)
                {
                    Condition.SemanticAnalysis(instance, AllMatches.INSTANCE);
                    StaticUsage.AddUsages(Condition.StaticUsage, Usage.ModeEnum.Read);
                }
                else
                {
                    Root.AddError("Stop condition not provided");
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the type of this expression
        /// </summary>
        /// <returns></returns>
        public override Type GetExpressionType()
        {
            return InitialValue.GetExpressionType();
        }

        /// <summary>
        ///     Provides the value associated to this Expression
        /// </summary>
        /// <param name="context">The context on which the value must be found</param>
        /// <param name="explain">The explanation to fill, if any</param>
        /// <returns></returns>
        protected internal override IValue GetValue(InterpretationContext context, ExplanationPart explain)
        {
            ExplanationPart stabilizeExpressionExplanation = ExplanationPart.CreateSubExplanation(explain, this);

            IterationsList = new List<IValue>();
            LastIteration.Value = InitialValue.GetValue(context, explain);

            int i = 0;
            bool stop = false;
            while (!stop)
            {
                i = i + 1;
                ExplanationPart iterationExplanation =
                    ExplanationPart.CreateSubExplanation(stabilizeExpressionExplanation, "Iteration " + i);
                ExplanationPart iteratorValueExplanation = ExplanationPart.CreateSubExplanation(iterationExplanation,
                    "Iteration expression value = ");
                int token = context.LocalScope.PushContext();
                context.LocalScope.SetVariable(LastIteration);
                CurrentIteration.Value = Expression.GetValue(context, iteratorValueExplanation);
                ExplanationPart.SetNamable(iteratorValueExplanation, CurrentIteration.Value);

                ExplanationPart stopValueExplanation = ExplanationPart.CreateSubExplanation(iterationExplanation,
                    "Stop expression value = ");
                context.LocalScope.SetVariable(CurrentIteration);
                BoolValue stopCondition = Condition.GetValue(context, stopValueExplanation) as BoolValue;
                ExplanationPart.SetNamable(stopValueExplanation, stopCondition);
                if (stopCondition != null)
                {
                    stop = stopCondition.Val;
                }
                else
                {
                    AddError("Cannot evaluate condition " + Condition, RuleChecksEnum.ExecutionFailed);
                    stop = true;
                }

                if (!stop && IterationsList.Exists(x => x.LiteralName == CurrentIteration.Value.LiteralName))
                {
                    // Cycle found !!!
                    IterationsList.Add(CurrentIteration.Value);
                    string cycleReport = "Execution cycled: ";

                    bool keepvalues = false;
                    foreach (IValue value in IterationsList)
                    {
                        if (keepvalues)
                        {
                            cycleReport += ", " + value.LiteralName;
                        }
                        else if (value.LiteralName == CurrentIteration.Value.LiteralName)
                        {
                            keepvalues = true;
                            cycleReport += value.LiteralName;
                        }
                    }

                    ExplanationPart.CreateSubExplanation(stabilizeExpressionExplanation, cycleReport);
                    CurrentIteration.Value = EfsSystem.Instance.EmptyValue;
                    stop = true;
                }
                else
                {
                    IterationsList.Add(CurrentIteration.Value);
                }

                context.LocalScope.PopContext(token);
                LastIteration.Value = CurrentIteration.Value;
            }
            ExplanationPart.SetNamable(stabilizeExpressionExplanation, CurrentIteration.Value);

            return CurrentIteration.Value;
        }

        /// <summary>
        ///     Fills the list provided with the element matching the filter provided
        /// </summary>
        /// <param name="retVal">The list to be filled with the element matching the condition expressed in the filter</param>
        /// <param name="filter">The filter to apply</param>
        public override void Fill(List<INamable> retVal, BaseFilter filter)
        {
            Expression.Fill(retVal, filter);
            InitialValue.Fill(retVal, filter);
            Condition.Fill(retVal, filter);
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public override void GetExplain(TextualExplanation explanation, bool explainSubElements = true)
        {
            explanation.Write("STABILIZE ");
            explanation.Write(Expression);
            explanation.Write(" INITIAL_VALUE ");
            explanation.Write(InitialValue);
            explanation.Write(" STOP_CONDITION ");
            explanation.Write(Condition);
        }

        /// <summary>
        ///     Checks the expression and appends errors to the root tree node when inconsistencies are found
        /// </summary>
        public override void CheckExpression()
        {
            base.CheckExpression();

            InitialValue.CheckExpression();
            Type initialValueType = InitialValue.GetExpressionType();
            if (initialValueType != null)
            {
                Expression.CheckExpression();
                Type expressionType = Expression.GetExpressionType();
                if (expressionType != null)
                {
                    if (expressionType != initialValueType)
                    {
                        AddError("Expression " + Expression + " has not the same type (" + expressionType.FullName +
                                 " than initial value " + InitialValue + " type " + initialValueType.FullName, RuleChecksEnum.SemanticAnalysisError);
                    }
                }
                else
                {
                    AddError("Cannot determine type of expression " + Expression, RuleChecksEnum.SemanticAnalysisError);
                }

                Type conditionType = Condition.GetExpressionType();
                if (conditionType != null)
                {
                    if (!(conditionType is BoolType))
                    {
                        AddError("Condition " + Condition + " does not evaluate to a boolean", RuleChecksEnum.SemanticAnalysisError);
                    }
                }
                else
                {
                    AddError("Cannot determine type of condition " + Condition, RuleChecksEnum.SyntaxError);
                }
            }
            else
            {
                AddError("Cannot determine type of the initial value " + InitialValue, RuleChecksEnum.SemanticAnalysisError);
            }
        }

        /// <summary>
        ///     Creates the graph associated to this expression, when the given parameter ranges over the X axis
        /// </summary>
        /// <param name="context">The interpretation context</param>
        /// <param name="parameter">The parameters of *the enclosing function* for which the graph should be created</param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public override Graph CreateGraph(InterpretationContext context, Parameter parameter, ExplanationPart explain)
        {
            Graph retVal = Graph.createGraph(GetValue(context, explain), parameter, explain);

            return retVal;
        }
    }
}
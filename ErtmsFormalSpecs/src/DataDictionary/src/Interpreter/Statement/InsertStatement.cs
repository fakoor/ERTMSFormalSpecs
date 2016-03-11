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
using DataDictionary.Interpreter.Filter;
using DataDictionary.Rules;
using DataDictionary.Tests.Runner;
using DataDictionary.Types;
using DataDictionary.Values;
using DataDictionary.Variables;
using Utils;

namespace DataDictionary.Interpreter.Statement
{
    public class InsertStatement : Statement
    {
        /// <summary>
        ///     The value to insert
        /// </summary>
        public Expression Value { get; private set; }

        /// <summary>
        ///     The list on which the value should be inserted
        /// </summary>
        public Expression ListExpression { get; private set; }

        /// <summary>
        ///     The element which should belong to the list to be replaced when the list is full
        /// </summary>
        public Expression ReplaceElement { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="root">The root element for which this element is built</param>
        /// <param name="log"></param>
        /// <param name="value">The value to insert</param>
        /// <param name="listExpression">The list to alter</param>
        /// <param name="replaceElement">The element to be replaced, if any</param>
        /// <param name="parsingData">Additional information about the parsing process</param>
        public InsertStatement(ModelElement root, ModelElement log, Expression value, Expression listExpression,
            Expression replaceElement, ParsingData parsingData)
            : base(root, log, parsingData)
        {
            Value = SetEnclosed(value);
            ListExpression = SetEnclosed(listExpression);
            ReplaceElement = SetEnclosed(replaceElement);
        }

        /// <summary>
        ///     Performs the semantic analysis of the statement
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <returns>True if semantic analysis should be continued</returns>
        public override bool SemanticAnalysis(INamable instance = null)
        {
            bool retVal = base.SemanticAnalysis(instance);

            if (retVal)
            {
                // Value
                if (Value != null)
                {
                    Value.SemanticAnalysis(instance);
                    StaticUsage.AddUsages(Value.StaticUsage, Usage.ModeEnum.Read);
                }
                else
                {
                    AddError("Value to insert not provided");
                }

                // ListExpression
                if (ListExpression != null)
                {
                    ListExpression.SemanticAnalysis(instance, IsLeftSide.INSTANCE);
                    StaticUsage.AddUsages(ListExpression.StaticUsage, Usage.ModeEnum.ReadAndWrite);
                }
                else
                {
                    AddError("List expression not provided");
                }

                // ReplaceElement
                if (ReplaceElement != null)
                {
                    ReplaceElement.SemanticAnalysis(instance);
                    StaticUsage.AddUsages(ReplaceElement.StaticUsage, Usage.ModeEnum.Read);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the list of elements read by this statement
        /// </summary>
        /// <param name="retVal">the list to fill</param>
        public override void ReadElements(List<ITypedElement> retVal)
        {
            retVal.AddRange(Value.GetVariables());
            retVal.AddRange(ListExpression.GetVariables());
        }

        /// <summary>
        ///     Provides the statement which modifies the variable
        /// </summary>
        /// <param name="variable"></param>
        /// <returns>null if no statement modifies the element</returns>
        public override VariableUpdateStatement Modifies(ITypedElement variable)
        {
            return null;
        }

        /// <summary>
        ///     Provides the list of update statements induced by this statement
        /// </summary>
        /// <param name="retVal">the list to fill</param>
        public override void UpdateStatements(List<VariableUpdateStatement> retVal)
        {
        }

        /// <summary>
        ///     Checks the statement for semantical errors
        /// </summary>
        public override void CheckStatement()
        {
            if (ListExpression != null)
            {
                ListExpression.CheckExpression();

                if (ListExpression.Ref is Parameter)
                {
                    Root.AddError("Cannot change the list value which is a parameter (" + ListExpression + ")");
                }

                Collection targetListType = ListExpression.GetExpressionType() as Collection;
                if (targetListType != null)
                {
                    if (Value != null)
                    {
                        Type elementType = Value.GetExpressionType();
                        if (elementType != targetListType.Type)
                        {
                            Root.AddError("Inserted element type does not corresponds to list type");
                        }
                    }

                    if (ReplaceElement != null)
                    {
                        ReplaceElement.CheckExpression();

                        Type replaceElementType = ReplaceElement.GetExpressionType();
                        if (replaceElementType != null)
                        {
                            if (targetListType.Type != null)
                            {
                                if (replaceElementType != targetListType.Type)
                                {
                                    Root.AddError("The replace element type (" + replaceElementType.FullName +
                                                  ") does not correspond to the list type (" +
                                                  targetListType.Type.FullName +
                                                  ")");
                                }
                            }
                            else
                            {
                                Root.AddError("Cannot determine list element's type for " + targetListType.FullName);
                            }
                        }
                        else
                        {
                            Root.AddError("Cannot determine replacement element type");
                        }
                    }
                }
                else
                {
                    Root.AddError("Cannot determine collection type of " + ListExpression);
                }
            }
            else
            {
                Root.AddError("List should be specified");
            }


            if (Value != null)
            {
                Value.CheckExpression();
            }
            else
            {
                Root.AddError("Value should be specified");
            }
        }

        /// <summary>
        ///     Provides the changes performed by this statement
        /// </summary>
        /// <param name="context">The context on which the changes should be computed</param>
        /// <param name="changes">The list to fill with the changes</param>
        /// <param name="explanation">The explanatino to fill, if any</param>
        /// <param name="apply">Indicates that the changes should be applied immediately</param>
        /// <param name="runner"></param>
        public override void GetChanges(InterpretationContext context, ChangeList changes, ExplanationPart explanation,
            bool apply, Runner runner)
        {
            IVariable variable = ListExpression.GetVariable(context);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            changes.Add(new InsertInListChange(context, this, variable, explanation), apply, runner);
        }

        /// <summary>
        /// Provides the change for this insert statement
        /// </summary>
        /// <param name="context"></param>
        /// <param name="variable"></param>
        /// <param name="explanation"></param>
        /// <param name="apply"></param>
        /// <param name="runner"></param>
        /// <returns></returns>
        public Change GetChange(InterpretationContext context, IVariable variable, ExplanationPart explanation, bool apply, Runner runner)
        {
            Change retVal = null;

            // Explain what happens in this statement
            explanation = ExplanationPart.CreateSubExplanation(explanation, this);

            if (variable != null)
            {
                // HacK : ensure that the value is a correct rigth side
                // and keep the result of the right side operation
                ListValue listValue = variable.Value.RightSide(variable, false, false) as ListValue;
                variable.Value = listValue;
                if (listValue != null)
                {
                    ExplanationPart.CreateSubExplanation(explanation, "Input data = ", listValue);

                    IValue value = Value.GetExpressionValue(context, explanation);
                    if (value != null)
                    {
                        if (!listValue.Val.Contains(value))
                        {
                            ListValue newListValue = new ListValue(listValue);
                            if (newListValue.Val.Count < newListValue.CollectionType.getMaxSize())
                            {
                                ExplanationPart.CreateSubExplanation(explanation, "Inserting", value);
                                newListValue.Val.Add(value.RightSide(variable, true, true));                                
                            }
                            else
                            {
                                // List is full, try to remove an element before inserting the new element
                                if (ReplaceElement != null)
                                {
                                    IValue removeValue = ReplaceElement.GetExpressionValue(context, explanation);
                                    ExplanationPart.CreateSubExplanation(explanation, "Replaced element", removeValue);
                                    int index = newListValue.Val.IndexOf(removeValue);
                                    if (index >= 0)
                                    {
                                        ExplanationPart.CreateSubExplanation(explanation, "Replacing", value);
                                        newListValue.Val[index] = value.RightSide(variable, true, true);
                                    }
                                    else
                                    {
                                        Root.AddError("Cannot remove replacing element " + removeValue.Name);
                                    }
                                }
                                else
                                {
                                    Root.AddError("Cannot add new element in list value : list is full");
                                }
                            }

                            retVal = new Change(variable, variable.Value, newListValue);
                            ExplanationPart.CreateSubExplanation(explanation, Root, retVal);
                        }
                        else
                        {
                            ExplanationPart.CreateSubExplanation(explanation, "NOT added : Already present in collection", value);
                        }
                    }
                    else
                    {
                        Root.AddError("Cannot find value for " + Value);
                    }
                }
                else
                {
                    Root.AddError("Variable " + ListExpression + " does not contain a list value");
                }
            }
            else
            {
                Root.AddError("Cannot find variable for " + ListExpression);
            }

            return retVal;
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public override void GetExplain(TextualExplanation explanation, bool explainSubElements = true)
        {
            explanation.Write("INSERT ");
            explanation.Write(Value);
            explanation.Write(" IN ");
            explanation.Write(ListExpression);

            if (ReplaceElement != null)
            {
                explanation.Write(" WHEN FULL REPLACE");
                explanation.Write(ReplaceElement);
            }
        }

        /// <summary>
        ///     Provides a real short description of this statement
        /// </summary>
        /// <returns></returns>
        public override string ShortShortDescription()
        {
            return ListExpression.ToString();
        }

        /// <summary>
        ///     Provides the main model elemnt affected by this statement
        /// </summary>
        /// <returns></returns>
        public override ModelElement AffectedElement()
        {
            return ListExpression.Ref as ModelElement;
        }
    }
}
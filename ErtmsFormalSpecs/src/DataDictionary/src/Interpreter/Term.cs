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
using DataDictionary.Types;
using DataDictionary.Values;
using DataDictionary.Variables;
using Utils;

namespace DataDictionary.Interpreter
{
    public class Term : InterpreterTreeNode, IReference
    {
        /// <summary>
        ///     The designator of this term
        /// </summary>
        public Designator Designator { get; private set; }

        /// <summary>
        ///     The literal value of this designator
        /// </summary>
        public Expression LiteralValue { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="root">The root element for which this model is built</param>
        /// <param name="log"></param>
        /// <param name="designator"></param>
        /// <param name="parsingData">Additional information about the parsing process</param>
        public Term(ModelElement root, ModelElement log, Designator designator, ParsingData parsingData)
            : base(root, log, parsingData)
        {
            Designator = SetEnclosed(designator);
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="root">The root element for which this model is built</param>
        /// <param name="log"></param>
        /// <param name="literal"></param>
        /// <param name="parsingData">Additional information about the parsing process</param>
        public Term(ModelElement root, ModelElement log, Expression literal, ParsingData parsingData)
            : base(root, log, parsingData)
        {
            LiteralValue = SetEnclosed(literal);
        }

        /// <summary>
        /// Indicates whether this expression references an instance
        /// </summary>
        /// <returns></returns>
        public bool IsInstance ()
        {
            bool retVal = false;
            if (LiteralValue != null)
            {
                retVal = true;
            }
            else if (Designator != null)
            {
                retVal = Designator.IsInstance ();
            }
            return retVal;
        }

        /// <summary>
        ///     Provides the possible references for this term (only available during semantic analysis)
        /// </summary>
        /// <param name="instance">the instance on which this element should be found.</param>
        /// <param name="expectation">the expectation on the element found</param>
        /// <param name="last">indicates that this is the last element in a dereference chain</param>
        /// <returns></returns>
        public ReturnValue GetReferences(INamable instance, BaseFilter expectation, bool last)
        {
            ReturnValue retVal = null;

            if (Designator != null)
            {
                retVal = Designator.GetReferences(instance, expectation, last);
            }
            else if (LiteralValue != null)
            {
                retVal = LiteralValue.GetReferences(instance, expectation, last);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the possible references types for this expression (used in semantic analysis)
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <param name="expectation">Indicates the kind of element we are looking for</param>
        /// <param name="last">indicates that this is the last element in a dereference chain</param>
        /// <returns></returns>
        public ReturnValue GetReferenceTypes(INamable instance, BaseFilter expectation, bool last)
        {
            ReturnValue retVal = null;

            if (Designator != null)
            {
                retVal = new ReturnValue();

                foreach (ReturnValueElement element in Designator.GetReferences(instance, expectation, last).Values)
                {
                    if (element.Value is Type)
                    {
                        const bool asType = true;
                        retVal.Add(element.Value, null, asType);
                    }
                }
            }
            else if (LiteralValue != null)
            {
                retVal = LiteralValue.GetReferenceTypes(instance, expectation, true);
            }

            return retVal;
        }

        /// <summary>
        ///     Performs the semantic analysis of the term
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <param name="expectation">Indicates the kind of element we are looking for</param>
        /// <param name="lastElement">Indicates that this element is the last one in a dereference chain</param>
        /// <returns>True if semantic analysis should be continued</returns>
        public void SemanticAnalysis(INamable instance, BaseFilter expectation, bool lastElement)
        {
            if (Designator != null)
            {
                Designator.SemanticAnalysis(instance, expectation, lastElement);
                StaticUsage = Designator.StaticUsage;
            }
            else if (LiteralValue != null)
            {
                LiteralValue.SemanticAnalysis(instance, expectation);
                StaticUsage = LiteralValue.StaticUsage;
            }
        }

        /// <summary>
        ///     The model element referenced by this term.
        /// </summary>
        public INamable Ref
        {
            get
            {
                INamable retVal = null;

                if (Designator != null)
                {
                    retVal = Designator.Ref;
                }
                else if (LiteralValue != null)
                {
                    retVal = LiteralValue.Ref;
                }

                return retVal;
            }
            set
            {
                if (Designator != null)
                {
                    Designator.Ref = value;
                }
                else if (LiteralValue != null)
                {
                    LiteralValue.Ref = value;
                }                
            }
        }

        /// <summary>
        ///     Provides the type of this expression
        /// </summary>
        /// <returns></returns>
        public Type GetExpressionType()
        {
            Type retVal = null;

            if (Designator != null)
            {
                retVal = Designator.GetDesignatorType();
            }
            else if (LiteralValue != null)
            {
                retVal = LiteralValue.GetExpressionType();
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the variable referenced by this expression, if any
        /// </summary>
        /// <param name="context">The context on which the variable must be found</param>
        /// <returns></returns>
        public IVariable GetVariable(InterpretationContext context)
        {
            IVariable retVal = null;

            if (Designator != null)
            {
                retVal = Designator.GetVariable(context);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the value associated to this Expression
        /// </summary>
        /// <param name="context">The context on which the value must be found</param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public IValue GetValue(InterpretationContext context, ExplanationPart explain)
        {
            IValue retVal = null;

            if (Designator != null)
            {
                retVal = Designator.GetValue(context);
            }
            else if (LiteralValue != null)
            {
                retVal = LiteralValue.GetValue(context, explain);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the element called by this term, if any
        /// </summary>
        /// <param name="context">The context on which the variable must be found</param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public ICallable GetCalled(InterpretationContext context, ExplanationPart explain)
        {
            ICallable retVal = null;

            if (Designator != null)
            {
                retVal = Designator.GetCalled(context);
            }

            return retVal;
        }

        /// <summary>
        ///     Fills the list provided with the element matching the filter provided
        /// </summary>
        /// <param name="retVal">The list to be filled with the element matching the condition expressed in the filter</param>
        /// <param name="filter">The filter to apply</param>
        public void Fill(List<INamable> retVal, BaseFilter filter)
        {
            if (Designator != null)
            {
                Designator.Fill(retVal, filter);
            }
            else if (LiteralValue != null)
            {
                LiteralValue.Fill(retVal, filter);
            }
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public override void GetExplain(TextualExplanation explanation, bool explainSubElements = true)
        {
            if (Designator != null)
            {
                explanation.Write(Designator);
            }
            else if (LiteralValue != null)
            {
                explanation.Write(LiteralValue);
            }
        }

        /// <summary>
        ///     Checks the expression and appends errors to the root tree node when inconsistencies are found
        /// </summary>
        public void CheckExpression()
        {
            if (Designator != null)
            {
                Designator.CheckExpression();
            }
            else if (LiteralValue != null)
            {
                LiteralValue.CheckExpression();
            }
        }
    }
}
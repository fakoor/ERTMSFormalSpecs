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
using DataDictionary.RuleCheck;
using DataDictionary.Types;
using DataDictionary.Values;
using Utils;

namespace DataDictionary.Interpreter
{
    public class NumberExpression : Expression
    {
        /// <summary>
        ///     The number image
        /// </summary>
        public string Image { get; private set; }

        /// <summary>
        ///     The image of the value
        /// </summary>
        public IValue Value { get; private set; }

        /// <summary>
        ///     The value type
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="root"></param>
        /// <param name="log"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="parsingData">Additional information about the parsing process</param>
        public NumberExpression(ModelElement root, ModelElement log, string value, Type type, ParsingData parsingData)
            : base(root, log, parsingData)
        {
            Image = value;
            Type = type;
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
                // Value
                if (Type != null)
                {
                    Value = Type.getValue(Image);
                    StaticUsage.AddUsage(Type, Root, Usage.ModeEnum.Type);
                }

                if (Value == null)
                {
                    AddError("Cannot evaluate " + ToString() + " as a number", RuleChecksEnum.SemanticAnalysisError);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the INamable which is referenced by this expression, if any
        /// </summary>
        public override INamable Ref
        {
            get { return Value; }
        }

        /// <summary>
        ///     Provides the type of this expression
        /// </summary>
        /// <returns></returns>
        public override Type GetExpressionType()
        {
            return Type;
        }

        /// <summary>
        ///     Provides the value associated to this Expression
        /// </summary>
        /// <param name="context">The context on which the value must be found</param>
        /// <param name="explain">The explanation to fill, if any</param>
        /// <returns></returns>
        protected internal override IValue GetValue(InterpretationContext context, ExplanationPart explain)
        {
            return Value;
        }

        /// <summary>
        ///     Fills the list provided with the element matching the filter provided
        /// </summary>
        /// <param name="retVal">The list to be filled with the element matching the condition expressed in the filter</param>
        /// <param name="filter">The filter to apply</param>
        public override void Fill(List<INamable> retVal, BaseFilter filter)
        {
            if (filter.AcceptableChoice(Value))
            {
                retVal.Add(Value);
            }
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public override void GetExplain(TextualExplanation explanation, bool explainSubElements = true)
        {
            explanation.Write(Image);
        }
    }
}
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

using System;
using System.Collections.Generic;
using DataDictionary.Functions;
using DataDictionary.Functions.PredefinedFunctions;
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.src;
using DataDictionary.Values;
using Function = DataDictionary.Functions.Function;

namespace DataDictionary.Types
{

    /// <summary>
    ///     A type. All types must inherit from this class
    /// </summary>
    public class Type : Generated.Type, IDefaultValueElement, IGraphicalDisplay
    {
        /// <summary>
        ///     Provides the enclosing namespace
        /// </summary>
        public NameSpace NameSpace
        {
            get
            {
                NameSpace retVal = EnclosingNameSpaceFinder.find(this);

                if (retVal == null && Dictionary != null)
                {
                    // This can be the case for artificial types
                    retVal = Dictionary.FindNameSpace("Default");
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Indicates if the type is abstract
        /// </summary>
        /// <returns></returns>
        public virtual bool IsAbstract
        {
            get { return false; }
            set
            {
                Structure aStructure = this as Structure;
                if (aStructure != null)
                {
                    aStructure.IsAbstract = value;
                }
            }
        }

        /// <summary>
        ///     Gets a value based on its image
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public virtual IValue getValue(string image)
        {
            return null;
        }

        /// <summary>
        ///     The default value, as string
        /// </summary>
        public virtual string Default
        {
            get { return getDefault(); }
            set { setDefault(value); }
        }


        public override string ExpressionText
        {
            get
            {
                string retVal = Default;

                if (retVal == null)
                {
                    retVal = "";
                }

                return retVal;
            }
            set
            {
                Default = value;
                __expression = null;
            }
        }

        /// <summary>
        ///     Provides the expression tree associated to this action's expression
        /// </summary>
        private Expression __expression;

        public Expression Expression
        {
            get
            {
                if (__expression == null)
                {
                    __expression = new Parser().Expression(this, ExpressionText);
                }

                return __expression;
            }
            set { __expression = value; }
        }

        public InterpreterTreeNode Tree
        {
            get { return Expression; }
        }

        /// <summary>
        ///     Clears the expression tree to ensure new compilation
        /// </summary>
        public void CleanCompilation()
        {
            Expression = null;
        }

        /// <summary>
        ///     Creates the tree according to the expression text
        /// </summary>
        public InterpreterTreeNode Compile()
        {
            // Side effect, builds the statement if it is not already built
            return Tree;
        }


        /// <summary>
        ///     Indicates that the expression is valid for this IExpressionable
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public bool checkValidExpression(string expression)
        {
            bool retVal = false;

            Expression tree = new Parser().Expression(this, expression, null, false, null, true);
            retVal = tree != null;

            return retVal;
        }

        /// <summary>
        ///     The default value
        /// </summary>
        public virtual IValue DefaultValue
        {
            get
            {
                IValue retVal = null;

                try
                {
                    if (!Utils.Util.isEmpty(Default))
                    {
                        retVal = getValue(Default);

                        if (retVal == null)
                        {
                            if (Expression != null)
                            {
                                retVal = Expression.GetExpressionValue(new InterpretationContext(this), null);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    AddException(e);
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Indicates whether a value can be cast into this type
        /// </summary>
        public virtual bool CanBeCastInto
        {
            get { return false; }
        }

        /// <summary>
        ///     A function which allows to cast a value as a new value of this type
        /// </summary>
        public Function castFunction;

        public Function CastFunction
        {
            get
            {
                if (castFunction == null && CanBeCastInto)
                {
                    Util.DontNotify(() => { castFunction = new Cast(this); });
                }

                return castFunction;
            }
        }

        /// <summary>
        ///     Converts a value in this type
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns></returns>
        public virtual IValue convert(IValue value)
        {
            return null;
        }

        /// <summary>
        ///     Performs the arithmetic operation based on the type of the result
        /// </summary>
        /// <param name="context">The context used to perform this operation</param>
        /// <param name="left"></param>
        /// <param name="operation"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual IValue PerformArithmericOperation(InterpretationContext context, IValue left,
            BinaryExpression.Operator operation, IValue right) // left +/-/*/div/exp right
        {
            IValue retVal = null;

            Function leftFunction = left as Function;
            Function rigthFunction = right as Function;

            if (leftFunction != null)
            {
                if (rigthFunction == null)
                {
                    if (leftFunction.Graph != null)
                    {
                        Graph graph = Graph.createGraph(Function.GetDoubleValue(right));
                        rigthFunction = graph.Function;
                    }
                    else
                    {
                        Surface surface = Surface.createSurface(Function.GetDoubleValue(right),
                            leftFunction.Surface.XParameter, leftFunction.Surface.YParameter);
                        rigthFunction = surface.Function;
                    }
                }

                if (leftFunction.Graph != null)
                {
                    IGraph tmp = null;
                    switch (operation)
                    {
                        case BinaryExpression.Operator.Add:
                            tmp = leftFunction.Graph.AddGraph(rigthFunction.Graph);
                            break;

                        case BinaryExpression.Operator.Sub:
                            tmp = leftFunction.Graph.SubstractGraph(rigthFunction.Graph);
                            break;

                        case BinaryExpression.Operator.Mult:
                            tmp = leftFunction.Graph.MultGraph(rigthFunction.Graph);
                            break;

                        case BinaryExpression.Operator.Div:
                            tmp = leftFunction.Graph.DivGraph(rigthFunction.Graph);
                            break;
                    }
                    Graph tmpGraph = tmp as Graph;
                    if (tmpGraph != null)
                    {
                        retVal = tmpGraph.Function;
                    }
                }
                else
                {
                    Surface rightSurface = rigthFunction.GetSurface(leftFunction.Surface.XParameter,
                        leftFunction.Surface.YParameter);
                    Surface tmp = null;
                    switch (operation)
                    {
                        case BinaryExpression.Operator.Add:
                            tmp = leftFunction.Surface.AddSurface(rightSurface);
                            break;

                        case BinaryExpression.Operator.Sub:
                            tmp = leftFunction.Surface.SubstractSurface(rightSurface);
                            break;

                        case BinaryExpression.Operator.Mult:
                            tmp = leftFunction.Surface.MultiplySurface(rightSurface);
                            break;

                        case BinaryExpression.Operator.Div:
                            tmp = leftFunction.Surface.DivideSurface(rightSurface);
                            break;
                    }
                    retVal = tmp.Function;
                }
            }

            return retVal;
        }

        public virtual bool CompareForEquality(IValue left, IValue right) // left == right
        {
            return left == right;
        }

        public virtual bool Less(IValue left, IValue right) // left < right
        {
            throw new TypeInconsistancyException("Cannot compare " + left.ToString() + " with " + right.ToString());
        }

        public virtual bool Greater(IValue left, IValue right) // left > right
        {
            throw new TypeInconsistancyException("Cannot compare " + left.ToString() + " with " + right.ToString());
        }

        public virtual bool Contains(IValue right, IValue left) // left in right
        {
            throw new TypeInconsistancyException("Variable of type " + GetType() + " cannot contain a variable of type " +
                                                 left.GetType());
        }

        /// <summary>
        ///     Indicates that the other type can be placed in variables of this type
        /// </summary>
        /// <param name="otherType"></param>
        /// <returns></returns>
        public virtual bool Match(Type otherType)
        {
            bool retVal = false;

            if (otherType is AnyType)
            {
                retVal = true;
            }
            else
            {
                ModelElement current = otherType;
                while (!retVal && current != null)
                {
                    retVal = this == current;
                    current = current.Updates;
                }

                current = this;
                while (!retVal && current != null)
                {
                    retVal = current == otherType;
                    current = current.Updates;
                }
            }

            // Check unified structures
            if (!retVal)
            {
                retVal = MatchStructures(this as Structure, otherType as UnifiedStructure) ||
                         MatchStructures(otherType as Structure, this as UnifiedStructure);
            }

            // Check unified state machines
            if (!retVal)
            {
                retVal = MatchStateMachines(this as StateMachine, otherType as UnifiedStateMachine) ||
                         MatchStateMachines(otherType as StateMachine, this as UnifiedStateMachine);
            }

            return retVal;
        }

        /// <summary>
        ///     Checks that the unified structure includes the structure and, in that case,
        ///     indicates that the types match
        /// </summary>
        /// <param name="structureType"></param>
        /// <param name="unifiedStructureType"></param>
        /// <returns></returns>
        private bool MatchStructures(Structure structureType, UnifiedStructure unifiedStructureType)
        {
            bool retVal = false;
            if (structureType != null && unifiedStructureType != null)
            {
                retVal = unifiedStructureType.MergedStructures.Contains(structureType);
            }
            return retVal;
        }

        /// <summary>
        ///     Checks that the unified state machine includes the state machine and, in that case,
        ///     indicates that the types match
        /// </summary>
        /// <param name="stateMachineType"></param>
        /// <param name="unifiedstateMachineType"></param>
        /// <returns></returns>
        private bool MatchStateMachines(StateMachine stateMachineType, UnifiedStateMachine unifiedstateMachineType)
        {
            bool retVal = false;
            if (stateMachineType != null && unifiedstateMachineType != null)
            {
                retVal = unifiedstateMachineType.MergedStateMachines.Contains(stateMachineType);
            }
            return retVal;
        }

        /// <summary>
        ///     Indicates that binary operation is valid for this type and the other type
        /// </summary>
        /// <param name="otherType"></param>
        /// <returns></returns>
        public virtual bool ValidBinaryOperation(BinaryExpression.Operator operation, Type otherType)
        {
            bool retVal;

            if (operation == BinaryExpression.Operator.In || operation == BinaryExpression.Operator.NotIn)
            {
                Collection collectionType = otherType as Collection;
                if (collectionType != null)
                {
                    retVal = Match(collectionType.Type);
                }
                else
                {
                    retVal = Match(otherType);
                }
            }
            else
            {
                retVal = Match(otherType);
            }

            return retVal;
        }

        /// <summary>
        ///     Indicates if the type is double
        /// </summary>
        public bool IsDouble()
        {
            bool retVal = false;

            Range range = this as Range;
            if (range != null)
            {
                retVal = range.getPrecision() == acceptor.PrecisionEnum.aDoublePrecision;
            }
            else
            {
                retVal = this == EFSSystem.DoubleType;
            }

            return retVal;
        }
        
        /// <summary>
        ///     Combines two types to create a new one
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual Type CombineType(Type right, BinaryExpression.Operator Operator)
        {
            return null;
        }

        /// <summary>
        ///     The X position
        /// </summary>
        public int X
        {
            get { return getX(); }
            set { setX(value); }
        }

        /// <summary>
        ///     The Y position
        /// </summary>
        public int Y
        {
            get { return getY(); }
            set { setY(value); }
        }

        /// <summary>
        ///     The width
        /// </summary>
        public int Width
        {
            get { return getWidth(); }
            set { setWidth(value); }
        }

        /// <summary>
        ///     The height
        /// </summary>
        public int Height
        {
            get { return getHeight(); }
            set { setHeight(value); }
        }

        /// <summary>
        ///     The name to be displayed
        /// </summary>
        public virtual string GraphicalName
        {
            get { return Name; }
        }

        /// <summary>
        ///     Indicates whether the namespace is hidden
        /// </summary>
        public bool Hidden
        {
            get { return getHidden(); }
            set { setHidden(value); }
        }

        /// <summary>
        ///     Indicates that the element is pinned
        /// </summary>
        public bool Pinned
        {
            get { return getPinned(); }
            set { setPinned(value); }
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public virtual void GetExplain(TextualExplanation explanation, bool explainSubElements)
        {
            explanation.Comment(this);
        }

        /// <summary>
        /// Indicates that a rule is applicable for this type at the provided priority
        /// </summary>
        /// <returns></returns>
        public virtual bool ApplicableRule(acceptor.RulePriority priority)
        {
            return false;
        }
    }
}
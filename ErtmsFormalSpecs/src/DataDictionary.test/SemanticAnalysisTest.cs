﻿using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.RuleCheck;
using DataDictionary.Tests.Runner;
using DataDictionary.Values;
using NUnit.Framework;
using Utils;
using Collection = DataDictionary.Types.Collection;
using NameSpace = DataDictionary.Types.NameSpace;
using Structure = DataDictionary.Types.Structure;
using StructureElement = DataDictionary.Types.StructureElement;
using StateMachine = DataDictionary.Types.StateMachine;
using State = DataDictionary.Constants.State;
using Variable = DataDictionary.Variables.Variable;
using RuleCondition = DataDictionary.Rules.RuleCondition;

namespace DataDictionary.test
{
    [TestFixture]
    internal class SemanticAnalysisTest : BaseModelTest
    {
        /// <summary>
        ///     Tests that an EFS expression that can refer to either a variable or a type will reference the variable
        /// </summary>
        [Test]
        public void TestVariableAndTypeWithSameName()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "NameSpace");

            Structure structure = CreateStructure(nameSpace, "ModelElement");
            StructureElement structElem = CreateStructureElement(structure, "Value", "Boolean");
            structElem.setDefault("True");

            Variable variable = CreateVariable(nameSpace, "ModelElement", "ModelElement");
            variable.SubVariables["Value"].Value = System.BoolType.False;

            Expression expression = new Parser().Expression(dictionary, "NameSpace.ModelElement.Value");
            IValue value = expression.GetExpressionValue(new InterpretationContext(), null);

            Assert.AreEqual(value, variable.SubVariables["Value"].Value);
        }

        /// <summary>
        ///     Test the concatenation of two collections
        /// </summary>
        [Test]
        public void TestCollectionConcatenation()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "NameSpace");

            Structure structure = CreateStructure(nameSpace, "ModelElement");
            StructureElement structElem = CreateStructureElement(structure, "Value", "Boolean");
            structElem.setDefault("True");

            Collection collection = CreateCollection(nameSpace, "Coll", "ModelElement", 10);
            collection.Type = structure;
            collection.setMaxSize(3);
            collection.Default = "[]"; 

            Variable variable = CreateVariable(nameSpace, "V", "Coll");

            RuleCondition ruleCondition = CreateRuleAndCondition(nameSpace, "Test");
            Action action = CreateAction(ruleCondition, "V <- V + [ModelElement{Value => True}] ");

            RuleCheckerVisitor visitor = new RuleCheckerVisitor(dictionary);
            visitor.visit(nameSpace);

            Util.IsThereAnyError isThereAnyError = new Util.IsThereAnyError();
            Assert.AreEqual(0, isThereAnyError.ErrorsFound.Count);
            Assert.AreEqual("[]", variable.Value.LiteralName);

            Runner runner = new Runner(false);
            runner.Cycle();
            Assert.AreEqual("[" + structure.DefaultValue.LiteralName + "]", variable.Value.LiteralName);

            runner.Cycle();
            Assert.AreEqual("[" + structure.DefaultValue.LiteralName + ", " + structure.DefaultValue.LiteralName + "]", variable.Value.LiteralName);

            runner.Cycle();
            Assert.AreEqual("[" + structure.DefaultValue.LiteralName + ", " + structure.DefaultValue.LiteralName + ", " + structure.DefaultValue.LiteralName + "]", variable.Value.LiteralName);

            // In this case, the new collection cannot be placed in the variable
            runner.Cycle();
            Assert.AreEqual(1, action.Messages.Count);
            Assert.AreEqual(ElementLog.LevelEnum.Error, action.Messages[0].Level);
        }

        /// <summary>
        ///     Tests that two "cousin" states in a state machine that share the same name are not confused
        ///             State Machine
        ///             /            \
        ///         State1          State2
        ///            |              |
        ///        SubState1  !=   SubState1
        /// </summary>
        [Test]
        public void TestDifferentiationOfSubStatesWithSameName()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "NameSpace");

            StateMachine stateMachine = CreateStateMachine(nameSpace, "StateMachine");
            // The left branch
            State state1 = CreateState(stateMachine, "State1");
            State subState1 = CreateState(state1.StateMachine, "SubState");
            // The right branch
            State state2 = CreateState(stateMachine, "State2");
            State subState2 = CreateState(state2.StateMachine, "SubState");

            Variable stateVariable = CreateVariable(nameSpace, "Variable", "StateMachine");
            stateVariable.Default = "StateMachine.State1.SubState";

            Compiler.Compile_Synchronous(true);

            Expression expression = new Parser().Expression(dictionary,
                "NameSpace.Variable in [NameSpace.StateMachine.State2.SubState]");
            IValue value = expression.GetExpressionValue(new InterpretationContext(), null);
            Assert.AreEqual(System.BoolType.False, value);
        }
    }
}
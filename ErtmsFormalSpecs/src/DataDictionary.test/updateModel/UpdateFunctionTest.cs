﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataDictionary.Constants;
using DataDictionary.Functions;
using DataDictionary.Interpreter;
using DataDictionary.Types;
using DataDictionary.Values;
using NUnit.Framework;

namespace DataDictionary.test
{
    [TestFixture]
    public class UpdateFunctionTest : BaseModelTest
    {
        /// <summary>
        /// Tests that a call to an updated function is evaluated correctly
        /// </summary>
        [Test]
        public void TestUpdateFunction()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");
            Function function = CreateFunction(nameSpace, "f", "Bool");
            Case cas1 = CreateCase(function, "Case 1", "q()");

            Function function2 = CreateFunction(nameSpace, "q", "Bool");
            Case cas2 = CreateCase(function2, "Case 1", "True");


            Dictionary dictionary2 = CreateDictionary("TestUpdate");
            dictionary2.setUpdates(dictionary.Guid);

            Function updatedFunction = function.CreateFunctionUpdate(dictionary2);
            Case cas3 = (Case) updatedFunction.Cases[0];
            cas3.ExpressionText = "NOT q()";


            Compiler.Compile_Synchronous(true);

            Expression expression = Parser.Expression(dictionary, "N1.f()");
            IValue value = expression.GetValue(new InterpretationContext(), null);
            Assert.AreEqual(System.BoolType.False, value);
        }

        [Test]
        public void TestUpdateMultipleFunctions()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");

            Function function = CreateFunction(nameSpace, "f", "Bool");
            Case cas1 = CreateCase(function, "Case 1", "q()");

            Function function2 = CreateFunction(nameSpace, "q", "Bool");
            Case cas2 = CreateCase(function2, "Case 1", "True");

            Dictionary dictionary2 = CreateDictionary("TestUpdate");
            dictionary2.setUpdates(dictionary.Guid);
            
            Function updatedFunction = function.CreateFunctionUpdate(dictionary2);
            Case cas3 = (Case)updatedFunction.Cases[0];
            cas3.ExpressionText = "NOT q(param => 3)";
            
            Function updatedFunction2 = function2.CreateFunctionUpdate(dictionary2);
            Parameter intParameter = CreateFunctionParameter(updatedFunction2, "param", "Integer");
            Case cas4 = (Case) updatedFunction2.Cases[0];
            cas4.ExpressionText = "False";
            

            Compiler.Compile_Synchronous(true);


            Expression expression = Parser.Expression(dictionary, "N1.f()");
            IValue value = expression.GetValue(new InterpretationContext(), null);
            Assert.AreEqual(System.BoolType.True, value);
        }


        [Test]
        public void TestUpdateCalledFunction()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");

            Function function = CreateFunction(nameSpace, "f", "Bool");
            Case cas1 = CreateCase(function, "Case 1", "q()");

            Function function2 = CreateFunction(nameSpace, "q", "Bool");
            Case cas2 = CreateCase(function2, "Case 1", "True");


            Dictionary dictionary2 = CreateDictionary("TestUpdate");
            dictionary2.setUpdates(dictionary.Guid);

            Function updatedFunction = function2.CreateFunctionUpdate(dictionary2);
            Case cas3 = (Case)updatedFunction.Cases[0];
            cas3.ExpressionText = "False";


            Compiler.Compile_Synchronous(true);

            Expression expression = Parser.Expression(dictionary, "N1.f()");
            IValue value = expression.GetValue(new InterpretationContext(), null);
            Assert.AreEqual(System.BoolType.False, value);
        }

        [Test]
        public void TestParameterTypeReference()
        {

            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");

            DataDictionary.Types.Enum enumeration = CreateEnum(nameSpace, "Enum");
            DataDictionary.Constants.EnumValue value1 = CreateEnumValue(enumeration, "First");

            Function function = CreateFunction(nameSpace, "f", "Bool");
            
            Parameter param = new Parameter();
            param.setTypeName("N1.Enum");
            param.setName("Value");

            function.appendParameters(param);
            Case cas1 = CreateCase(function, "Case 1", "True", "Value == Enum.First");


            Dictionary dictionary2 = CreateDictionary("TestUpdate");
            dictionary2.setUpdates(dictionary.Guid);

            Function updatedFunction = function.CreateFunctionUpdate(dictionary2);
            Case cas3 = (Case)updatedFunction.Cases[0];
            cas3.ExpressionText = "False";


            Compiler.Compile_Synchronous(true);

            Expression expression = Parser.Expression(dictionary, "N1.f(N1.Enum.First)");
            IValue value = expression.GetValue(new InterpretationContext(), null);
            Assert.AreEqual(System.BoolType.False, value);
        }
    }
}
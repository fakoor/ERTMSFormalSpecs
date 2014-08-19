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
namespace DataDictionary.Functions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Caches the result of a function in a Curry-like fashion 
    /// </summary>
    public class CurryCache
    {
        /// <summary>
        /// The function for which this cache is built
        /// </summary>
        private Function Function { get; set; }

        /// <summary>
        /// A curried cache 
        /// </summary>
        private class FunctionCache : Dictionary<Values.IValue, FunctionCache>
        {
            /// <summary>
            /// The value associated to the last parameter of the function
            /// </summary>
            public Values.IValue Value { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            public FunctionCache() : base()
            {
            }
        }

        /// <summary>
        /// Provides the value 
        /// </summary>
        private FunctionCache Curry { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public CurryCache(Function function)
        {
            Function = function;
            Curry = new FunctionCache();
        }


        /// <summary>
        /// Gets the value of this curry cache according to the parameter association
        /// </summary>
        /// <param name="association"></param>
        /// <returns></returns>
        public Values.IValue GetValue(Dictionary<Variables.Actual, Values.IValue> association)
        {
            Values.IValue retVal = null;

            FunctionCache current = Curry;
            foreach (Values.IValue val in OrderedParameters(association))
            {
                FunctionCache next;
                if (current.TryGetValue(val, out next))
                {
                    current = next;
                }
                else
                {
                    current = null;
                    break;
                }
            }

            if (current != null)
            {
                retVal = current.Value;
            }

            return retVal;
        }

        /// <summary>
        /// Sets the value according to the parameter association
        /// </summary>
        /// <param name="association"></param>
        /// <param name="value"></param>
        public void SetValue(Dictionary<Variables.Actual, Values.IValue> association, Values.IValue value)
        {
            FunctionCache current = Curry;
            foreach (Values.IValue val in OrderedParameters(association))
            {
                FunctionCache next;
                if (!current.TryGetValue(val, out next))
                {
                    next = new FunctionCache();
                    current.Add(val, next);
                }
                current = next;
            }

            current.Value = value;
        }

        /// <summary>
        /// Provides the parameter values ordered by the formal parameter they are associated with
        /// </summary>
        /// <param name="association"></param>
        /// <returns></returns>
        private List<Values.IValue> OrderedParameters(Dictionary<Variables.Actual, Values.IValue> association)
        {
            List<Values.IValue> retVal = new List<Values.IValue>();

            foreach (Parameter p in Function.FormalParameters)
            {
                // Order the actual according to the function parameter 
                foreach (KeyValuePair<Variables.Actual, Values.IValue> pair in association)
                {
                    if (pair.Key.Parameter == p)
                    {
                        retVal.Add(pair.Value);
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Clears the cache
        /// </summary>
        public void Clear()
        {
            Curry.Clear();
        }
    }
}
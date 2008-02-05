using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Specifications
{
    /// <summary>
    /// Class for implementing Case/When/Else statements.
    /// </summary>
    /// <see cref="ISpecification"/>
    public class CaseSpecification : Specification
    {
        private readonly List<WhenThenPair> _whenThens;
        private readonly ISpecification _else;

        internal CaseSpecification(List<WhenThenPair> whenThens, ISpecification elseSpecification)
        {
            _whenThens = whenThens;
            _else = elseSpecification;
        }

        /// <summary>
        /// Perform the test.
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        protected override TestResult InnerTest(object exp, object root)
        {
            // test when-then pairs in order
            // the first "when" that succeeds will determine the result
            foreach (WhenThenPair pair in _whenThens)
            {
                if(pair.When.Test(exp).Success)
                {
                    return ResultOf(pair.Then.Test(exp));
                }
            }

            // otherwise execute the "else" clause
            return ResultOf(_else.Test(exp));
        }

        private TestResult ResultOf(TestResult innerResult)
        {
            return innerResult.Success ? new TestResult(true) : 
                new TestResult(false, new TestResultReason(this.FailureMessage, innerResult.Reasons)); 
        }
    }
}

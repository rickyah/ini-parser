using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace NUnit.Framework
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class ExpectedExceptionAttribute : NUnitAttribute, IWrapTestMethod
    {
        private readonly Type _expectedExceptionType;

        public ExpectedExceptionAttribute(Type type) => _expectedExceptionType = type;

        public TestCommand Wrap(TestCommand command) => new ExpectedExceptionCommand(command, _expectedExceptionType);

        private class ExpectedExceptionCommand : DelegatingTestCommand
        {
            private readonly Type _expectedType;

            public ExpectedExceptionCommand(TestCommand innerCommand, Type expectedType)
                : base(innerCommand)
            {
                _expectedType = expectedType;
            }

            public override TestResult Execute(TestExecutionContext context)
            {
                var exceptionType = RunAndGetExceptionType(context);

                if (exceptionType == _expectedType)
                    context.CurrentResult.SetResult(ResultState.Success);
                else
                    context.CurrentResult.SetResult(ResultState.Failure, $"Expected {_expectedType.Name} but got {exceptionType?.Name ?? "no exception"}.");

                return context.CurrentResult;
            }

            private Type RunAndGetExceptionType(TestExecutionContext context)
            {
                try
                {
                    innerCommand.Execute(context);
                    return null;
                }
                catch (Exception ex)
                {
                    return ex is NUnitException
                        ? ex.InnerException.GetType()
                        : ex.GetType();
                }
            }
        }
    }
}
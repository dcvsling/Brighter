﻿using System;
using FluentAssertions;
using Nito.AsyncEx;
using Xunit;
using Paramore.Brighter.Policies.Handlers;
using Paramore.Brighter.Tests.ExceptionPolicy.TestDoubles;
using Paramore.Brighter.Tests.TestDoubles;
using Polly;
using TinyIoC;

namespace Paramore.Brighter.Tests.ExceptionPolicy
{
    public class CommandProcessorWithRetryPolicyAsyncTests
    {
        private CommandProcessor _commandProcessor;
        private readonly MyCommand _myCommand = new MyCommand();
        private int _retryCount;

        public CommandProcessorWithRetryPolicyAsyncTests()
        {
            var registry = new SubscriberRegistry();
            registry.RegisterAsync<MyCommand, MyFailsWithFallbackDivideByZeroHandlerAsync>();

            var container = new TinyIoCContainer();
            var handlerFactory = new TinyIocHandlerFactoryAsync(container);
            container.Register<IHandleRequestsAsync<MyCommand>, MyFailsWithFallbackDivideByZeroHandlerAsync>().AsSingleton();
            container.Register<IHandleRequestsAsync<MyCommand>, ExceptionPolicyHandlerAsync<MyCommand>>().AsSingleton();

            var policyRegistry = new PolicyRegistry();

            var policy = Policy
                .Handle<DivideByZeroException>()
                .WaitAndRetryAsync(new[]
                {
                    1.Seconds(),
                    2.Seconds(),
                    3.Seconds()
                }, (exception, timeSpan) =>
                {
                    _retryCount++;
                });
            policyRegistry.Add("MyDivideByZeroPolicy", policy);

            MyFailsWithFallbackDivideByZeroHandlerAsync.ReceivedCommand = false;

            _commandProcessor = new CommandProcessor(registry, handlerFactory, new InMemoryRequestContextFactory(), policyRegistry);
        }

        //We have to catch the final exception that bubbles out after retry
        [Fact]
        public void When_Sending_A_Command_That_Should_Retry_Failure_Async()
        {
            Catch.Exception(() => AsyncContext.Run(async () => await _commandProcessor.SendAsync(_myCommand)));

            //_should_send_the_command_to_the_command_handler
            Assert.True(MyFailsWithFallbackDivideByZeroHandlerAsync.ShouldReceive(_myCommand));
            //_should_retry_three_times
            Assert.AreEqual(3, _retryCount);
        }
    }
}

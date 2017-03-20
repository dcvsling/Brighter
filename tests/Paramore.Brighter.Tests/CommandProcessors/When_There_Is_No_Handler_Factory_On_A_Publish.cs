﻿#region Licence
/* The MIT License (MIT)
Copyright © 2015 Ian Cooper <ian_hammond_cooper@yahoo.co.uk>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the “Software”), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE. */

#endregion

using System;
using Xunit;
using Paramore.Brighter.Tests.TestDoubles;

namespace Paramore.Brighter.Tests
{
    public class CommandProcessorPublishMissingHandlerFactoryTests
    {
        private CommandProcessor _commandProcessor;
        private readonly MyEvent _myEvent = new MyEvent();
        private Exception _exception;

        public CommandProcessorPublishMissingHandlerFactoryTests()
        {
            _commandProcessor = new CommandProcessor(new SubscriberRegistry(), (IAmAHandlerFactory) null, new InMemoryRequestContextFactory(), new PolicyRegistry());
        }

        [Fact]
        public void When_There_Is_No_Handler_Factory_On_A_Publish()
        {
            _exception = Catch.Exception(() => _commandProcessor.Publish(_myEvent));

           //_should_throw_an_invalid_operation_exception
            Assert.IsInstanceOf<InvalidOperationException>(_exception);
        }
    }
}

﻿using NUnit.Framework;
using Nimbus.Infrastructure.RequestResponse;
using Nimbus.MessageContracts;
using Shouldly;

namespace Nimbus.Tests
{
    public class WhenExtractingAHandlerMethodForARequest
    {
        [Test]
        public void WeShouldGetTheRightMethod()
        {
            var request = new SomeInternalRequest();
            var handlerMethod = RequestMessagePump.ExtractHandlerMethodInfo(request);

            handlerMethod.ShouldNotBe(null);
        }

        public class SomeInternalRequest : BusRequest<SomeInternalRequest, SomeInternalResponse>
        {
        }

        public class SomeInternalResponse : IBusResponse
        {
        }
    }
}
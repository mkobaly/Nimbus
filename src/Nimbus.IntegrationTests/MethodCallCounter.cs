﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Nimbus.Extensions;

namespace Nimbus.IntegrationTests
{
    public static class MethodCallCounter
    {
        private static readonly ConcurrentDictionary<MethodInfo, ConcurrentBag<object>> _allReceivedCalls = new ConcurrentDictionary<MethodInfo, ConcurrentBag<object>>();

        public static IEnumerable<KeyValuePair<MethodInfo, ConcurrentBag<object>>> AllReceivedCalls
        {
            get { return _allReceivedCalls; }
        }

        public static void RecordCall<T>(Expression<Action<T>> expr)
        {
            var methodCallExpression = (MethodCallExpression) expr.Body;
            var method = methodCallExpression.Method;

            // http://stackoverflow.com/questions/2616638/access-the-value-of-a-member-expression
            var messageExpression = methodCallExpression.Arguments.First();
            var objectMember = Expression.Convert(messageExpression, typeof (object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();
            var message = getter();

            var messageBag = _allReceivedCalls.GetOrAdd(method, new ConcurrentBag<object>());
            messageBag.Add(message);

            var methodName = "{0}.{1}".FormatWith(typeof(T).FullName, method.Name);
            Console.WriteLine("Observed call to {0} with argument of type {1}".FormatWith(methodName, message.GetType()));
        }

        public static IEnumerable<object> AllReceivedMessages
        {
            get
            {
                var messageBags = _allReceivedCalls.Values;
                var messages = messageBags.SelectMany(bag => bag).ToArray();
                return messages;
            }
        }

        public static IEnumerable<object> ReceivedCallsWithAnyArg<T>(Expression<Action<T>> expr)
        {
            var methodCallExpression = (MethodCallExpression) expr.Body;
            var method = methodCallExpression.Method;
            var messageBag = _allReceivedCalls.GetOrAdd(method, new ConcurrentBag<object>());
            return messageBag;
        }

        public static void Clear()
        {
            _allReceivedCalls.Clear();
        }
    }
}
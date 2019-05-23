using System;
using System.Collections.Generic;
using System.Text;

namespace Astron.Network.Tests.Helpers
{
    public static class MockHelpers
    {
        public static void VerifyAll<T>(this IEnumerable<T> mocks, Action<T> callback)
        {
            foreach (var mock in mocks) callback(mock);
        }
    }
}

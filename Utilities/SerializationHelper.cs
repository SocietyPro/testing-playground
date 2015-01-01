using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.JustMock;

namespace BackendTest.Utilities
{
    class SerializationHelper<T>
    {
        public void MockPrivateDeserializer(T result)
        {
            Mock.SetupStatic(typeof(T), Behavior.CallOriginal);
            Mock.NonPublic.Arrange<T>(typeof(T), "Deserialize", ArgExpr.IsAny<byte[]>()).Returns(result);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.JustMock;

namespace Arbitrage.Utilities
{
    class SerializationHelper<T>
    {
        public void MockPrivateDeserializer(T result)
        {
            Mock.SetupStatic(typeof(T), Behavior.CallOriginal);
            Mock.NonPublic.Arrange<T>(typeof(T), "Deserialize", ArgExpr.IsAny<byte[]>()).Returns(result);
        }

        public void MockPrivateSerializer()
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes("result");
            Mock.SetupStatic(typeof(T), Behavior.CallOriginal);
            Mock.NonPublic.Arrange<byte[]>(typeof(T), "Serialized").Returns(bytes);
        }   
       
    }
}

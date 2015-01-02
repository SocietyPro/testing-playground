using System.IO;
using System.Net;
using Arbitrage;
using Telerik.JustMock;

namespace BackendTest.Utilities
{
    public class WebHelper
    {
        

        public void MockRequest(Stream resultStream)
        {
            HttpWebResponse response = Mock.Create<HttpWebResponse>();
            HttpWebRequest request = Mock.Create<HttpWebRequest>();
         
            Mock.SetupStatic<WebRequest>();
            Mock.Arrange(() => WebRequest.Create(Arg.AnyString)).Returns(request);
            Mock.Arrange(() => request.GetResponse()).Returns(response);
            Mock.Arrange(() => response.GetResponseStream()).Returns(resultStream);
            
        }

    }
}
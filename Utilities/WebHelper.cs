using System.IO;
using System.Net;
using Arbitrage;
using Telerik.JustMock;

namespace Arbitrage.Utilities
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

         public void MockRequestError(Stream resultStream)
        {
            HttpWebRequest request = Mock.Create<HttpWebRequest>();
            HttpWebResponse response = Mock.Create<HttpWebResponse>();

            WebException ex = Mock.Create<WebException>();
            Mock.SetupStatic<WebRequest>();
            Mock.Arrange(() => WebRequest.Create(Arg.AnyString)).Returns(request);
            Mock.Arrange(() => request.GetResponse()).Throws(ex);
            Mock.Arrange(() => response.GetResponseStream()).Returns(resultStream);
            Mock.Arrange(() => ex.Response).Returns(response);
        }

    }
}
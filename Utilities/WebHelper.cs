using System.IO;
using System.Net;
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

        public static void MockWebSocketServer()
        {
            Mock.SetupStatic(typeof(WebSocketServer));
            WebSocketServer server = Mock.Create<WebSocketServer>(Behavior.Strict);
            Mock.Arrange(() => WebSocketServer.Current).Returns(server);
            //Mock.Arrange(() => server.Broadcast(Arg.IsAny<TradeWatch>(), Arg.IsAny<Subscription>())).DoNothing();
            Mock.Arrange(() => server.Broadcast(Arg.IsAny<IMessage>())).DoNothing();

            
        }

    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Telerik.JustMock;

namespace BackendTest.Utilities
{
    public class WebRequestHelper
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
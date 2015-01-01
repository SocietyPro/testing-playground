using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Arbitrage;
using Telerik.JustMock;

namespace BackendTest.Utilities
{
    public class SqlHelper
    {
        public void MockConnection()
        {
            SqlConnection conn = Mock.Create<SqlConnection>();
            Mock.Arrange(() => Sql.GetConnection(Arg.AnyString)).Returns(conn);
            Mock.Arrange(() => Sql.DbNullToInt(Arg.AnyObject)).CallOriginal();
        }

        public void MockEmptyDataReader()
        {
            MockDataReader(new List<string>());
        }

        public void MockDataReaderWithOneLine(string result)
        {
            List<string> list = new List<string>();
            list.Add(result);
            MockDataReader(list);
            
        }

        public void MockDataReader(List<string> results)
        {
            MockConnection();
            SqlDataReader dr = Mock.Create<SqlDataReader>();   
            Mock.Arrange(() => new SqlCommand().ExecuteReader()).Returns(dr);
            foreach (string row in results)
            {
               byte[] bytes = System.Text.Encoding.UTF8.GetBytes(row);
               Mock.Arrange(() => dr.Read()).Returns(true).InSequence(); 
               Mock.Arrange(() => dr[0]).Returns(bytes).InSequence();
            }
            Mock.Arrange(() => dr.Read()).Returns(false).InSequence();
        }

        public void MockNonQuery()
        {
            MockConnection();
            Mock.Arrange(() => new SqlCommand().ExecuteNonQuery()).DoNothing();
            //Mock.Assert(() => new SqlCommand().ExecuteNonQuery(), Occurs.Once());
        }
    }
}
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
            SqlDataReader dr = Mock.Create<SqlDataReader>();
            Mock.Arrange(() => dr.Read()).Returns(false).InSequence();
            Mock.Arrange(() => new SqlCommand().ExecuteReader()).Returns(dr);
        }

        public void MockDataReaderWithOneLine(string result)
        {
            SqlDataReader dr = Mock.Create<SqlDataReader>();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes("result");
            Mock.Arrange(() => dr.Read()).Returns(true).InSequence();
            Mock.Arrange(() => dr.Read()).Returns(false).InSequence();
            Mock.Arrange(() => dr[0]).Returns(bytes);
            Mock.Arrange(() => new SqlCommand().ExecuteReader()).Returns(dr);
        }
    }
}
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
        public void SetupSql()
        {
            SqlConnection conn = Mock.Create<SqlConnection>();
            Mock.Arrange(() => Sql.GetConnection(Arg.AnyString)).Returns(conn);
            Mock.Arrange(() => Sql.DbNullToInt(Arg.AnyObject)).CallOriginal();
        }

        
    }
}

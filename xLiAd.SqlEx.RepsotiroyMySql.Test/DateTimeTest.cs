using MySql.Data.MySqlClient;
using System;
using Xunit;

namespace xLiAd.SqlEx.RepsotiroyMySql.Test
{
    public class DateTimeTest
    {
        [Fact]
        public void Test1()
        {
            var connection = new MySqlConnection("server=localhost;user id=root;password=33842901;database=zhanglei;");
            var repo = new RepositoryMysql.RepositoryMysql<member>(connection);
            var list = repo.All();
        }
    }
}

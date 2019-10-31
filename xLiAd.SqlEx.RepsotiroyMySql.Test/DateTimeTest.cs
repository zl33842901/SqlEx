using Dapper;
using MySql.Data.MySqlClient;
using System;
using xLiAd.SqlEx.Core;
using xLiAd.SqlEx.RepositoryMysql;
using Xunit;

namespace xLiAd.SqlEx.RepsotiroyMySql.Test
{
    public class DateTimeTest
    {
        //[Fact]
        //public void Test1()
        //{
        //    var connection = new MySqlConnection("server=localhost;user id=root;password=33842901;database=zhanglei;");
        //    var repo = new RepositoryMysql.RepositoryMysql<member>(connection);
        //    var list = repo.All();
        //}

        public static ITypeMapper TypeMapper = new TypeMapper();
        string conn = "server=172.16.101.40;User Id=root;password=cig@2017;Database=dapperExTest;CharSet=utf8;Convert Zero Datetime=True;Allow Zero Datetime=True";
        RepositoryMysql<DictInfo> RepoDict => new RepositoryMysql<DictInfo>(conn);
        RepositoryMysql<TestStamp> repoStamp => new RepositoryMysql<TestStamp>(conn);
        RepositoryMysql<TestTimeStamp> repoTimeStamp => new RepositoryMysql<TestTimeStamp>(conn);
        RepositoryMysql<TestTimeStamp2> repoTimeStamp2 => new RepositoryMysql<TestTimeStamp2>(conn);
        [Fact]
        public void TestInsert()
        {
            var repository = repoTimeStamp;
            //Add �����������б�ʶ�ֶ�(Identity����)ʱ�����ر�ʶID�����򷵻�Ӱ������
            var rst = repository.Add(new TestTimeStamp()
            {
                Name = "������",
                CreateTime = DateTime.Now
            });
            rst += repository.Add(new TestTimeStamp()
            {
                Name = "�ٺ�",
                CreateTime = DateTime.MinValue
            });
            var repo2 = repoTimeStamp2;
            rst += repo2.Add(new TestTimeStamp2()
            {
                Name = "�ٺ�",
                CreateTime = DateTime.Now
            });
            rst += repo2.Add(new TestTimeStamp2()
            {
                Name = "�ٺ�",
                CreateTime = null
            });
            Assert.True(rst > 0);
        }
        [Fact]
        public void TestQuery()
        {
            var repository = repoTimeStamp;
            var list = repository.All();
            Assert.NotEmpty(list);

            //MySql.Data.Types.MySqlDateTime
        }
        [Fact]
        public void TestQuery2()
        {
            var repository = repoTimeStamp2;
            var list = repository.All();
            Assert.NotEmpty(list);
        }

        [Fact]
        public void TestQuery3()
        {
            var repository = repoTimeStamp;
            var DbCon = new MySqlConnection(conn);
            var Reader = DbCon.ExecuteReader("Select * from `TestTimeStamp`");
            //var Parser = Reader.GetRowParser(typeof(TestTimeStamp));
            var Parser = TypeConvert.GetSerializer<TestTimeStamp>(TypeMapper, Reader);
            while (Reader.Read())
            {
                object rst = Parser(Reader);
                //foreach (var p in ps)
                //{
                //    var col = Reader.GetOrdinal($"{p.Name}{ResolveExpression.JsonColumnNameSuffix}");
                //    var s = Reader.GetString(col);
                //    var pv = Deserializer(s, p.PropertyType);
                //    p.SetValue(rst, pv);
                //}
            }
            Reader.Close();
        }
    }
}

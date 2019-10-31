using System;
using System.Data.SqlClient;
using Xunit;

namespace xLiAd.SqlEx.RepositoryMsSql.Test
{
    public class InsertTest
    {
        private SqlConnection Conn => new SqlConnection("Data Source=127.0.0.1;Initial Catalog=zhanglei;Persist Security Info=True;User ID=sa;Password=zhanglei");
        [Fact]
        public void TestDictInfo()
        {
            var repository = new RepositoryMsSql<DictInfo>(Conn);
            //Add �����������б�ʶ�ֶ�(Identity����)ʱ�����ر�ʶID�����򷵻�Ӱ������
            var rst = repository.Add(new DictInfo()
            {
                DictName = "��������",
                DictType = 99,
                Remark = "���Ա�ע",
                OrderNum = OrderEnum.optionA,
                Deleted = false
            });
            Assert.True(rst > 0);
        }
        [Fact]
        public void TestTimeStamp()
        {
            RepositoryMsSql<TestStamp> repoStamp = new RepositoryMsSql<TestStamp>(Conn);
            var rst = repoStamp.Add(new TestStamp()
            {
                CreateTime = DateTime.Now,
                Name = "��������"
            });
            Assert.True(rst > 0);
        }
        [Fact]
        public void TestDictInfoMulti()
        {
            var repository = new RepositoryMsSql<DictInfo>(Conn);
            //Add �����������б�ʶ�ֶ�(Identity����)ʱ�����ر�ʶID�����򷵻�Ӱ������
            var rst = repository.Add(new DictInfo[] {
                new DictInfo()
                {
                DictName = "��������",
                DictType = 99,
                Remark = "���Ա�ע",
                OrderNum = OrderEnum.optionA,
                Deleted = false
                },
                new DictInfo()
                {
                DictName = "��������",
                DictType = 99,
                Remark = "���Ա�ע",
                OrderNum = OrderEnum.optionA,
                Deleted = false
                }
            });
            Assert.True(rst > 0);
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Xunit;
using xLiAd.SqlEx.Core.Helper;
using xLiAd.SqlEx.Repository;

namespace xLiAd.SqlEx.RepositoryMsSql.Test
{
    /// <summary>
    /// 本 ORM 优势：
    /// 0，使用表达式写条件、排序 等能提高工作效率（避免字段名称写错，简化语法。）
    /// 1，支持 Select 出部分字段匿名类型
    /// 2，支持部分字段更新
    /// 3，支持根据条件更新、删除
    /// 4，支持枚举字段检索
    /// 5，支持事务
    /// </summary>
    public class QueryTest
    {
        private SqlConnection Conn => new SqlConnection("Data Source=127.0.0.1;Initial Catalog=zhanglei;Persist Security Info=True;User ID=sa;Password=zhanglei");
        [Fact]
        public void TestWhere()
        {
            var repository = new RepositoryMsSql<DictInfo>(Conn);
            var ida = new List<int>() { 106071, 106072 };
            var rst = repository.Where(x => ida.Contains(x.DictID));
            Assert.Equal(2, rst.Count);
        }
        [Fact]
        public void TestWhere2()
        {
            var repository = new RepositoryMsSql<DictInfo>(Conn);
            int? id = 106071;
            var rst = repository.Where(x => x.DictID == id);
            Assert.Single(rst);
        }
        [Fact]
        public void TestWhere3()
        {
            var repository = new RepositoryMsSql<DictInfo2>(Conn);
            int? id = 106071;
            var rst = repository.Where(x => x.DictID == id);
            Assert.Single(rst);
        }
        [Fact]
        public void TestWhere4()
        {
            var repository = new RepositoryMsSql<DictInfo2>(Conn);
            int id = 106071;
            var rst = repository.Where(x => x.DictID == id);
            Assert.Single(rst);
        }
        [Fact]
        public void TestWhere5()
        {
            var repository = new RepositoryMsSql<DictInfo>(Conn);
            OrderEnum orderEnum = OrderEnum.optionA;
            var rst = repository.Where(x => x.OrderNum == orderEnum);
            Assert.NotEmpty(rst);
            foreach(var item in rst)
            {
                Assert.Equal(OrderEnum.optionA, item.OrderNum);
            }
        }
        [Fact]
        public void TestWhere6()
        {
            var repository = new RepositoryMsSql<DictInfo>(Conn);
            OrderEnum? orderEnum = OrderEnum.optionA;
            var rst = repository.Where(x => x.OrderNum == orderEnum);
            Assert.NotEmpty(rst);
            foreach (var item in rst)
            {
                Assert.Equal(OrderEnum.optionA, item.OrderNum);
            }
        }
        [Fact]
        public void TestWhere7()
        {
            var repository = new RepositoryMsSql<DictInfo2>(Conn);
            OrderEnum orderEnum = OrderEnum.optionA;
            var rst = repository.Where(x => x.OrderNum == orderEnum);
            Assert.NotEmpty(rst);
            foreach (var item in rst)
            {
                Assert.Equal(OrderEnum.optionA, item.OrderNum);
            }
        }
        [Fact]
        public void TestWhere8()
        {
            var repository = new RepositoryMsSql<DictInfo2>(Conn);
            OrderEnum? orderEnum = OrderEnum.optionA;
            var rst = repository.Where(x => x.OrderNum == orderEnum);
            Assert.NotEmpty(rst);
            foreach (var item in rst)
            {
                Assert.Equal(OrderEnum.optionA, item.OrderNum);
            }
        }
        [Fact]
        public void TestWhere9()
        {
            var repository = new RepositoryMsSql<DictInfo2>(Conn);
            var rst = repository.Where(x => x.OrderNum == null);
            Assert.Empty(rst);
        }
        [Fact]
        public void TestWhere10()
        {
            var repository = new RepositoryMsSql<DictInfo>(Conn);
            var rst = repository.Where(x => x.OrderNum == null);
            Assert.Empty(rst);
        }
        [Fact]
        public void TestWhere11()
        {
            var repository = new RepositoryMsSql<DictInfo>(Conn);
            var rst = repository.Where(x => x.DictID == null);
            Assert.Empty(rst);
        }
        [Fact]
        public void TestWhere12()
        {
            var repository = new RepositoryMsSql<DictInfo2>(Conn);
            var rst = repository.Where(x => x.DictID == null);
            Assert.Empty(rst);
        }
        [Fact]
        public void TestWhereAndFind()
        {
            var repository = new RepositoryMsSql<DictInfo>(Conn);
            var rst = repository.Where(x => x.DictID > 106087);
            var model = repository.Find(106088);//Find 是根据主键获取记录，所以主键上要有 Key标记。
            Assert.Equal(model.Remark, rst.Find(x => x.DictID == 106088)?.Remark);
        }
        [Fact]
        public void TestWhereAndAll()
        {
            var repository = new RepositoryMsSql<DictInfo>(Conn);
            var rst = repository.Where(x => x.DictID > 106091 && !x.Deleted);
            var model = rst.FirstOrDefault();
            Assert.False(null == model);
            var all = repository.All();
            var model2 = all.Find(x => x.DictID == model.DictID);
            Assert.False(null == model2);
            Assert.Equal(model.Deleted, model2.Deleted);
            Assert.Equal(model.CreateTime, model2.CreateTime);
            Assert.Equal(model.Remark, model2.Remark);
            Assert.Equal(model.DictName, model2.DictName);
        }
        [Fact]
        public void TestWhereWithSomeFields()
        {
            var repository = new RepositoryMsSql<DictInfo>(Conn);
            var rst = repository.Where(x => x.CreateTime > new DateTime(2018, 12, 1), x => x.DictID, x => x.DictName);//只查询 DictID DictName 两个字段
            Assert.False(0 == rst.Count);
            Assert.Equal(0, rst.Count(x => !string.IsNullOrEmpty(x.Remark)));
            var first = rst.First();
            var model = repository.Find(first.DictID);
            Assert.Equal(first.DictName, model.DictName);
        }
        [Fact]
        public void TestWhereSelect()
        {
            var repository = new RepositoryMsSql<DictInfo>(Conn);
            var rst = repository.WhereSelect(x => x.DictID >= 106071, x => x.DictName);//投影查询（只查询某些字段）可投影为匿名类型
            var rstfull = repository.Where(x => x.DictID >= 106071);
            Assert.False(0 == rstfull.Count);
            Assert.Equal(rst.Count, rstfull.Count);
            bool r = rst.Contains(rstfull.First().DictName);
            Assert.True(r);

            var ll2 = repository.WhereSelect(x => x.DictName.Contains("哈哈") && x.Deleted, x => new { x.DictName, x.DictID }).ToDictionary(x => x.DictID, x => x.DictName);
        }

        [Fact]
        public void TestWhereSelectDistinct()
        {
            IRepository<DictInfo> repository = new RepositoryMsSql<DictInfo>(Conn);
            var resultNoDistinct = repository.WhereSelect(x => true, x => x.DictName);
            var resultDistinct = repository.WhereSelectDistinct(x => true, x => x.DictName);
            foreach(var item in resultDistinct)
            {
                bool b = resultNoDistinct.Contains(item);
                Assert.True(b);
            }
            Assert.True(resultDistinct.Count <= resultNoDistinct.Count);
        }

        [Fact]
        public void TestWhereDistinct()
        {
            IRepository<DictInfo> repository = new RepositoryMsSql<DictInfo>(Conn);
            var resultDistinct = repository.WhereDistinct(x => true, x => x.DictName, x => x.DictType);
            var resultDistinct2 = repository.WhereDistinct(x => true);
            Assert.True(resultDistinct.Count <= resultDistinct2.Count);
        }

        [Fact]
        public void TestWhereOrderSelect()
        {
            var repository = new RepositoryMsSql<DictInfo>(Conn);
            var id = 106071;
            //WhereOrderSelect 查询排序并投影
            var lrst = repository.WhereOrderSelect(x => x.DictID > id && x.DictName.Contains("老总") && x.CreateTime > new DateTime(2018, 1, 1), x => x.DictID, x => new { i1 = x.DictID, i2 = x.DictName });
            bool b = lrst.Count > 0;
            Assert.True(b);
            b = !lrst.First().i2.Contains("老总");
            Assert.False(b);
        }
        [Fact]
        public void TestExpression()
        {
            var repository = new RepositoryMsSql<DictInfo>(Conn);
            Expression<Func<DictInfo, bool>> expression = x => x.DictID > 106071;
            expression = expression.And(x => x.DictName.Contains("老总"));
            var rst = repository.Where(expression);
            bool b = rst.Count > 0;
            Assert.True(b);
        }
        [Fact]
        public void TestTimeStamp()
        {
            RepositoryMsSql<TestStamp> repoStamp = new RepositoryMsSql<TestStamp>(Conn);
            var rst = repoStamp.Where(x => true);
            bool b = rst.Any(x => x.ROWVERSION < 1);
            Assert.False(b);
        }
        [Fact]
        public void TestPageList()
        {
            var repository = new RepositoryMsSql<DictInfo>(Conn);
            var rst = repository.PageList(x => x.DictID > 5555, x => x.DictID, 1, 10, true);
            bool b = rst.Items.Count == 10;
            Assert.True(b);
            Assert.Equal(rst.Total, repository.Count(x => x.DictID > 5555));
        }
        [Fact]
        public void TestTransaction()
        {
            var repository = new RepositoryMsSql<DictInfo>(Conn);
            var trans = repository.GetTransaction();//只利用了 repository 里的连接对象。
            var trepo = trans.GetRepository<DictInfo>();//带有事务的仓储对象
            var trepo2 = trans.GetRepository<TestStamp>();//带有事务的仓储对象
            bool rd = new System.Random().Next(1, 20000) > 10000;//随机的正负值，用来试验抛错与不抛错的情况。
            try
            {
                trepo.UpdateWhere(x => x.DictID == 100018, x => x.CreateTime, DateTime.Now);
                var i = 0;
                var t = trepo.Where(x => x.DictID == 100018);
                if (rd)
                {
                    var j = 5 / i;
                }
                trepo2.UpdateWhere(x => x.Id == 1, x => x.Name, "修改后的测试名称");
                trans.Commit();
                if (rd)
                    Assert.True(false);
                else
                    Assert.True(true);
            }
            catch (Exception)
            {
                trans.Rollback();
                if (rd)
                    Assert.True(true);
                else
                    Assert.True(false);
            }
        }
        /// <summary>
        /// 字段类型不对应的测试
        /// </summary>
        [Fact]
        public void QueryWhenField()
        {
            var repository = new RepositoryMsSql<Articles>(Conn);
            var result = repository.All();
        }
    }
}

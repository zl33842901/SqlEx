using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace xLiAd.SqlEx.RepositoryMsSql.Test
{
    public class RepositoryMine<T> : RepositoryMsSql<T>
    {
        public RepositoryMine(string conn) : base(conn, null)
        {

        }
        public RepositoryMine() : this("")
        {

        }
    }
    public class AbcRepository: RepositoryMine<int>
    {
        public AbcRepository() : base()
        {

        }
    }
    public class MultiInheritTest
    {
        [Fact]
        public void Test1()
        {
            var abcRepo = new AbcRepository();
        }
    }
}

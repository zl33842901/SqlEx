﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Xunit;

namespace xLiAd.SqlEx.RepositoryMsSql.Test
{
    public class QueryHelperTest
    {
        SqlConnection Con => new SqlConnection("Data Source=127.0.0.1;Initial Catalog=zhanglei;Persist Security Info=True;User ID=sa;Password=feih#rj87");
        //[Fact]
        //public void TestQueryParamProvider()
        //{
        //    RepositoryMsSql<DictInfo> repository = new RepositoryMsSql<DictInfo>(Con, null);
        //    var q = new QueryParamProvider<DictInfo>();
        //    q.AddItem(x => x.DictName, QueryParamProviderOprater.Contains);
        //    q.AddItem(x => x.DictID, QueryParamProviderOprater.LessThanOrEqual, null, null);
        //    q.AddItem(x => x.CreateTime, QueryParamProviderOprater.GreaterThanOrEqual, null, "startTime");
        //    q.AddItem(x => x.CreateTime, QueryParamProviderOprater.LessThan, null, "endTime");
        //    var nv = new System.Collections.Specialized.NameValueCollection();
        //    nv.Add("DictName", "");
        //    nv.Add("DictID", "106071");
        //    nv.Add("startTime", "2018-12-1");
        //    nv.Add("endTime", "2019-2-1");
        //    var ee = q.GetExpression(nv);
        //    var l = repository.Where(ee);
        //}
        //[Fact]
        //public void TestQueryParamJoiner()
        //{
        //    RepositoryMsSql<DictInfo> repository = new RepositoryMsSql<DictInfo>(Con, null);
        //    RepositoryMsSql<Articles> repos = new RepositoryMsSql<Articles>(Con, null);
        //    QueryParamJoiner<Articles> qq = new QueryParamJoiner<Articles>();
        //    qq.AddItem<DictInfo, int>(repository, x => x.DictName, QueryParamJoinerOprater.Contains, x => x.DictID, x => x.DictID);
        //    var nv2 = new System.Collections.Specialized.NameValueCollection();
        //    nv2.Add("DictName", "技术副");
        //    var aa = qq.GetExpression(nv2);
        //    var l2 = repos.Where(aa);
        //}
        //[Fact]
        //public void TestLeftJoin()
        //{
        //    RepositoryMsSql<DictInfo> repository = new RepositoryMsSql<DictInfo>(Con, null);
        //    RepositoryMsSql<Articles> repos = new RepositoryMsSql<Articles>(Con, null);
        //    QueryParamJoiner<Articles> qq = new QueryParamJoiner<Articles>();
        //    qq.AddItem<DictInfo, int>(repository, x => x.DictName, QueryParamJoinerOprater.Contains, x => x.DictID, x => x.DictID);
        //    var nv2 = new System.Collections.Specialized.NameValueCollection();
        //    nv2.Add("DictName", "技术副");
        //    var aa = qq.GetExpression(nv2);
        //    var l2 = repos.Where(aa);

        //    l2.LeftJoin(repository, x => x.DictID, x => x.DictID, (x, y) => { x.DictName = y.DictName; }, out var _, x=>x.DictName);

        //}
        //[Fact]
        //public void TestLeftExpression()
        //{
        //    RepositoryMsSql<DictInfo> repository = new RepositoryMsSql<DictInfo>(Con, null);
        //    RepositoryMsSql<Articles> repos = new RepositoryMsSql<Articles>(Con, null);
        //    RepositoryMsSql<Author> reposAuthor = new RepositoryMsSql<Author>(Con, null);
        //    QueryParamJoiner<Articles> qq = new QueryParamJoiner<Articles>();
        //    qq.AddItem<DictInfo, int>(repository, x => x.DictName, QueryParamJoinerOprater.Contains, x => x.DictID, x => x.DictID);
        //    var nv2 = new System.Collections.Specialized.NameValueCollection();
        //    nv2.Add("DictName", "技术副");
        //    var aa = qq.GetExpression(nv2);

        //    var bb = aa.LeftExpression<Author, Articles, int>(repos, x => x.AId, x => x.Id);
        //    var l3 = reposAuthor.Where(bb);
        //}
    }
}

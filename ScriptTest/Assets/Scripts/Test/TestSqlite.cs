using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSqlite : MonoBehaviour {

	// Use this for initialization
	void Start () {

        // Bak
        //DbAccess dbAccess = new DbAccess("data source=liuhaixia.db");
        string dbFilePath = Application.persistentDataPath + "/liuhaixia.db";
        DbAccess dbAccess;
#if ANDROID
		//注意！！！！！！！这行代码的改动
		dbAccess = new DbAccess("URI=file:" + dbFilePath);
#elif IOS
        
        //注意！！！！！！！这行代码的改动
        dbAccess = new DbAccess(@"Data Source=" + dbFilePath);
#else
        //dbAccess = new DbAccess("data source=liuhaixia.db"); // 可行
        //dbAccess = new DbAccess(@"Data Source=" + dbFilePath); // 可行
        dbAccess = new DbAccess("URI=file:" + dbFilePath); // 可行
#endif

        // 创建表格
        dbAccess.CreateTable("ben", new string[] { "name", "qq", "email", "blog" }, new string[] { "text", "text", "text", "text" });
        // 插入数据
        //dbAccess.InsertInto("gowild", new string[] { "'刘海峡'", "'609043941'", "'609043941@qq.com'", "'http://blog.sina.com.cn/s/blog_e7779a160102wpt1.html'" });
        // 更新数据
        //dbAccess.UpdateInto("gowild", new string[] { "name", "qq" }, new string[] { "'liuhaixia'", "'11111111'"}, "email", "'609043941@qq.com'");
        // 删除数据
        dbAccess.InsertInto("ben", new string[] { "'刘海峡'", "'609043941'", "'609043941@qq.com'", "'http://blog.sina.com.cn/s/blog_e7779a160102wpt1.html'" });
        dbAccess.InsertInto("ben", new string[] { "'刘大峡'", "'000000'", "'000000@qq.com'", "'http://blog.sina.com.cn/s/blog_e7779a160102wpt1.html'" });
        dbAccess.InsertInto("ben", new string[] { "'刘中峡'", "'111111'", "'111111@qq.com'", "'http://blog.sina.com.cn/s/blog_e7779a160102wpt1.html'" });
        dbAccess.InsertInto("ben", new string[] { "'刘小峡'", "'222222'", "'222222@qq.com'", "'http://blog.sina.com.cn/s/blog_e7779a160102wpt1.html'" });
        dbAccess.Delete("ben", new string[] { "email", "email" }, new string[] { "'000000@qq.com'", "'111111@qq.com'" });
        // 查找数据
        SqliteDataReader sqReader = dbAccess.SelectWhere("ben", new string[] { "name", "email" }, new string[] { "qq" }, new string[] { "=" }, new string[] { "609043941" });
        while (sqReader.Read())
        {
            Debug.Log(sqReader.GetString(sqReader.GetOrdinal("name")) + " || " + sqReader.GetString(sqReader.GetOrdinal("email")));
        }

        sqReader.Close();

        dbAccess.CloseSqlConnection();

        // Bak
        //string dbFilePath = Application.dataPath + "/liuhaixia.db";
        //DbAccess dbAccess = new DbAccess("URI=file:" + dbFilePath);
        //dbAccess.CreateTable("gowild", new string[] { "name", "qq", "email", "blog" }, new string[] { "text", "text", "text", "text" });
        //dbAccess.CloseSqlConnection();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

using System;
using Symbol.Data;

namespace Examples.Data.netcore {
    class Program {
        static void Main(string[] args) {
            //{
            //    var c = Symbol.Data.NoSQL.Condition.Parse("{\"createDate\":{\"$range\":{\"min\":\"2022-01-01\",\"minEq\":true,\"max\":\"2022-07-22\",\"maxEq\":true}}}");
            //    Console.WriteLine(c.ToJson(true));
            //}
            {
                //创建数据上下文对象
                //DataContextTest("mssql");
                //DataContextTest("mssql2012");
                //DataContextTest("mysql");
                DataContextTest("postgresql");
                DataContextTest("kingbase");
                //DataContextTest("sqlite");
            }
            Console.ReadKey();
        }

        static void DataContextTest(string type) {
            IDataContext db = CreateDataContext(type);


            //初始化 &  数据
            DatabaseSchema(db);

            //group by parse
 //           {
 //               var sql = @"
 //select
 //   [age],
 //   count(1) as [count],
 //   sum([money]) as [totalMoney],
 //   min([height]) as [min_height],
 //   max([height]) as [max_height],
 //   avg([height]) as [avg_height]
 //from [test]
 //where
 //    ( [age]>=@p1 )
 //group by
 //    [age]
 //having
 //    ( max([height])<=@p2 )
 //order by
 //   [totalMoney] desc
 //limit 3,10";
 //               var builder = db.CreateSelect("test", sql);
 //               builder.AddCommandParameter(18);
 //               builder.AddCommandParameter(190);
 //               Console.WriteLine(builder.CommandText);
 //               var list = builder.ToList();
 //               Console.WriteLine(JSON.ToNiceJSON(list));
 //           }

            //group by build
            {
                var builder = db.CreateSelect("test");
                //在年满18岁，并且身高在190及以下的人群中
                //    各年龄收入情况
                //        按收入，从高到低排序
                //        排除收入最高的前3的年龄
                //        取出10个年龄
                //    最小身高
                //    最大身高
                //    平均身高
                builder.Query(new { age = "{ '$gte' : 18 }" })
                       .GroupBy("age")
                       .Having(new { height ="{ '$max': { '$lte': 190 } }" })
                       .Select("age")
                       .CountAs("count")
                       .SumAs("money","totalMoney")
                       .MinAs("height","min_height")
                       .MaxAs("height", "max_height")
                       .AverageAs("height", "avg_height")
                       .OrderBy("totalMoney", OrderBys.Descing)
                       .Skip(3)
                       .Take(10);
                Console.WriteLine(builder.CommandText);
                var list = builder.ToList();
                Console.WriteLine(JSON.ToNiceJSON(list));
            }
            //增 删 改 查  常规操作
            DatabaseCRUD(db);
            //求值
            {
                var maxValue = db.Max<double>("test", "count");
                Console.WriteLine(maxValue);
            }
            {
                var maxValue = db.Max<object>("test", "count");
                Console.WriteLine(maxValue);
            }
            //泛型
            QueryGeneric(db);

            //性能测试
            QueryPerf(db);
        }

        static IDataContext CreateDataContext(string type) {
            object connectionOptions = null;
            switch (type) {
                case "mssql":
                    connectionOptions = new {
                        host = "mssql-master.hh\\MSSQL2014",    //服务器，端口为默认，所以不用写
                        port = 11433,                           //端口
                        name = "test",                          //数据库名称
                        account = "test",                       //登录账号
                        password = "test",                      //登录密码
                    };
                    break;
                case "mssql2012":
                    connectionOptions = new {
                        host = "mssql-master.hh\\MSSQL2014",    //服务器，端口为默认，所以不用写
                        port = 11433,                           //端口
                        name = "test",                          //数据库名称
                        account = "test",                       //登录账号
                        password = "test",                      //登录密码
                    };
                    break;
                case "mysql":
                    connectionOptions = new {
                        host = "mysql-master.hh",               //服务器
                        port = 3306,                            //端口，可以与服务器写在一起，例如127.0.0.1:3306
                        name = "test",                          //数据库名称
                        account = "test",                       //登录账号
                        password = "test",                      //登录密码
                    };
                    break;
                case "postgresql":
                    connectionOptions = new {
                        host = "pgsql-master.hh",               //服务器
                        port = 5432,                            //端口，可以与服务器写在一起，例如127.0.0.1:5432
                        name = "test",                          //数据库名称
                        account = "test",                       //登录账号
                        password = "test",                      //登录密码
                    };
                    break;
                case "kingbase":
                    connectionOptions = new {
                        host = "kingbase-master.hh",            //服务器
                        port = 54321,                           //端口，可以与服务器写在一起，例如127.0.0.1:5432
                        name = "test",                          //数据库名称
                        account = "test",                       //登录账号
                        password = "test",                      //登录密码
                    };
                    break;
                case "sqlite":
                    connectionOptions = new {
                        //name = "test",                          //数据库名称
                        //memory = true,                          //内存数据库
                        file= "D:\\.system\\cache\\temp\\test.sqlite.db"
                    };
                    break;
            }
            //Provider 自动扫描Symbol.Data.*.dll
            return Symbol.Data.Provider.CreateDataContext(type, connectionOptions);
            //return new Symbol.Data.SqlServer2012Provider().CreateDataContext(connectionOptions);
        }

        static void DatabaseSchema(IDataContext db) {
            if (db.TableExists("test")) {
                db.TableDelete("test");
            }
            var random = new Random();
            switch (db.Provider.Name) {
                case "mssql": {

                        #region 创建表：t_user
                        if (!db.TableExists("t_user")) {
                            db.ExecuteNonQuery(@"
                                create table [dbo].[t_user](
                                    [id]                     int identity(1,1)        not null                        ,
                                    [type]                   smallint                 not null                        ,
                                    [account]                nvarchar(64)             not null                        ,
                                    [password]               varchar(32)              not null                        ,
                                    constraint [PK_t_user] primary key clustered ([id] asc) with (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) 
                                        on [PRIMARY] 
                                ) on [PRIMARY]");
                        }
                        #endregion
                        #region 创建表：test
                        db.ExecuteNonQuery(@"
                                create table [dbo].[test](
                                    [id]                     bigint identity(1,1)     not null                        ,
                                    [name]                   nvarchar(255)                null                        ,
                                    [age]                    int                      not null                        ,
                                    [height]                 int                      not null                        ,
                                    [money]                  decimal(18,2)            not null                        ,
                                    [count]                  bigint                   not null                        ,
                                    [data]                   ntext                        null                        ,
                                    constraint [PK_test] primary key clustered ([id] asc) with (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) 
                                        on [PRIMARY] 
                                ) on [PRIMARY]");

                        #endregion

                    }
                    break;
                case "postgresql": {

                        #region 创建表：t_user
                        if (!db.TableExists("t_user")) {
                            db.ExecuteNonQuery(@"
                                create table t_user(
                                    id bigserial not null,
                                    ""type"" smallint  not null,
                                    account character varying(64) not null,
                                    ""password"" character varying(32) not null,
                                    CONSTRAINT ""pk_t_User_id"" PRIMARY KEY(id)
                                )
                                WITH(
                                    OIDS = FALSE
                                );");
                        }
                        #endregion
                        #region 创建表：test

                        db.ExecuteNonQuery(@"
                                create table test(
                                   id bigserial not null,
                                   name character varying(255),
                                   ""age""      integer         not null,
                                   ""height""   integer         not null,
                                   ""money""    numeric(18,2)   not null,
                                   ""count""    bigint          not null,
                                   ""data""     jsonb               null,
                                   CONSTRAINT ""pk_test_id"" PRIMARY KEY(id)
                                )
                                WITH(
                                  OIDS = FALSE
                                ); ");

                        #endregion

                    }
                    break;
                case "kingbase": {

                        #region 创建表：t_user
                        if (!db.TableExists("t_user")) {
                            db.ExecuteNonQuery(@"
                                create table t_user(
                                    id bigserial not null,
                                    ""type"" smallint  not null,
                                    account character varying(64) not null,
                                    ""password"" character varying(32) not null,
                                    CONSTRAINT ""pk_t_User_id"" PRIMARY KEY(id)
                                )
                                WITH(
                                    OIDS = FALSE
                                );");
                        }
                        #endregion
                        #region 创建表：test

                        db.ExecuteNonQuery(@"
                                create table test(
                                   id bigserial not null,
                                   name character varying(255),
                                   ""age""      integer         not null,
                                   ""height""   integer         not null,
                                   ""money""    numeric(18,2)   not null,
                                   ""count""    bigint          not null,
                                   ""data""     jsonb               null,
                                   CONSTRAINT ""pk_test_id"" PRIMARY KEY(id)
                                )
                                WITH(
                                  OIDS = FALSE
                                ); ");

                        #endregion

                    }
                    break;
                case "sqlite": {
                        #region 创建表
                        db.ExecuteNonQuery(@"
                            create table test(
                                id integer primary key autoincrement not null,
                                name        nvarchar(64)        not null,
                                [age]       int                 not null,
                                [height]        int             not null,
                                [money]         numeric(18,2)   not null,
                                [count]         bigint          not null,
                                [data]          ntext               null
                            )
                        ");
                        if (!db.TableExists("t_User")) {
                            db.ExecuteNonQuery(@"
                                create table t_User(
                                    id integer primary key autoincrement not null,
                                    [type] tinyint not null,
                                    account nvarchar(64) not null,
                                    [password] varchar(32) null,
                                    [data] ntext null
                                )
                            ");
                        }
                        #endregion
                        
                    }
                    break;
                case "mysql": {
                        #region 创建表：t_user
                        if (!db.TableExists("t_user")) {
                            db.ExecuteNonQuery(@"
                                CREATE TABLE `t_user`  (
                                  `id` bigint(0) NOT NULL AUTO_INCREMENT,
                                  `type` smallint(0) NOT NULL,
                                  `account` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
                                  `password` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
                                  PRIMARY KEY (`id`) USING BTREE
                                ) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;");
                        }
                        #endregion
                        #region 创建表：test
                            db.ExecuteNonQuery(@"
                                CREATE TABLE `test`  (
                                  `id`      bigint(0) NOT NULL AUTO_INCREMENT,
                                  `name`        varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
                                  `age`         int(0)          NOT NULL,
                                  `height`      bigint(0)       NOT NULL,
                                  `money`       decimal(18,2)   NOT NULL,
                                  `count`       bigint(0)       NOT NULL,
                                  `data`        json                NULL,
                                  PRIMARY KEY (`id`) USING BTREE
                                ) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;");
                           
                        #endregion
                        break;
                    }
            }
            #region 初始测试数据
            for (var i = 0; i < 100; i++) {
                db.InsertObject<long>("test", new {
                    name = "测试" + (i + 1),
                    count = random.Next(1, 100000),
                    age = random.Next(1, 100),
                    height = random.Next(140, 230),
                    money = Math.Round((decimal)(random.NextDouble() * 100000), 2),
                    data = new {
                        a = random.Next(100000) % 2 == 0,
                        list = new object[] {
                            random.Next(100000),
                            StringRandomizer.Next(8)
                        }
                    }
                });
            }

            #endregion

        }
        static void DatabaseCRUD(IDataContext db) {
            //like 测试
            {
                var builder = db.CreateSelect("t_user");
                builder.Query("{  'type': 1, 'account': { '$like': [ 'abc','zz' ] }  }");
                Console.WriteLine(builder.CommandText);
                builder.Wheres.Clear();
                builder.Query("{ 'account': 134 }");
                Console.WriteLine(builder.CommandText);
                builder.Wheres.Clear();
                builder.Query("{\"publisher\":{\"$like\":\"\\\"fc86a39a2b0a4522bfa203a3d053f286\\\"\"},\"classObjectId\":\"bde0f3f1a4c04b6caf3ecaa73ae32bc5\"}");
                Console.WriteLine(builder.CommandText);
                builder.Wheres.Clear();
                builder.Query("{\"createDate\":{\"$range\":{\"min\":\"2022-01-01\",\"minEq\":true,\"max\":\"2022-07-22\",\"maxEq\":true}}}");
                Console.WriteLine(builder.CommandText);
                builder.Dispose();
            }
            Console.WriteLine("continue ...");
            Console.ReadKey();



            //常规测试
            {
                //删除数据
                db.BeginTransaction();
                Console.WriteLine("delete count={0}", db.Delete("test", new {
                    name = "xxxxxxxxx",         //name为xxxxxxxxx
                    id = "{ '$gt': 200000 }"     //id大于200000，C#语法不支持JSON，但我们支持嵌套JSON语句 :)
                }));
                db.CommitTransaction();

                //插入数据
                db.BeginTransaction();
                var id = db.InsertObject<long>("test", new {
                    name = "xxxxxxxxx",
                    count = 9999,
                    age = 26,
                    height = 185,
                    money = 9999.99, //凉心价
                    data = new {//JSON类型测试
                        url = "https://www.baidu.com/",
                        guid = System.Guid.NewGuid(),
                        datetime = DateTime.Now,
                        values = FastWrapper.As(new {//嵌套复杂对象测试
                            nickName = "昵尔",
                            account = "test"
                        })
                    }
                });
                Console.WriteLine($"insert id={id}");
                db.CommitTransaction();

                //查询数据
                Console.WriteLine("select");
                Console.WriteLine(JSON.ToNiceJSON(db.Find("test", new { name = "xxxxxxxxx" })));
                //更新数据
                db.BeginTransaction();
                var updated = db.Update("test", new { name = "fsafhakjshfksjhf", count = 88 }, new { id }) == 1;
                Console.WriteLine($"update {updated}");
                db.CommitTransaction();

                //验证是否真的修改到
                Console.WriteLine("select new value");
                Console.WriteLine(JSON.ToNiceJSON(db.Find("test", new { id })));
            }
            Console.WriteLine("continue ...");
            Console.ReadKey();
            {
                //枚举测试
                var id = db.InsertObject<long>("t_user", new {
                    account = "admin",
                    type = UserTypes.Manager,
                    password="test"
                });
                Console.WriteLine(JSON.ToNiceJSON(db.Find("t_user", new { id })));
            }
            Console.WriteLine("continue ..."); 
            Console.ReadKey();

        }
        static void QueryPerf(IDataContext db) {
            var q = db.FindAll("test", "{ 'name':['test','test214'] }");

            int max = 0;
            int count = 0;
            int time = 0;
            var results = new System.Collections.Generic.List<string>();
            Action<int> begin = (p) => {
                count = 0;
                max = p;
                time = Environment.TickCount;
            };
            Action<string> end = (p) => {
                time = Environment.TickCount - time;
                string log = $"data read {p} {max} used time {time}ms, avg:{(decimal)time / 1000}ms.";
                results.Add(log);
                Console.WriteLine(log);
            };
            Action print = () => {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(string.Join("\r\n", results.ToArray()));
            };
            {
                begin(10000);
                foreach (var item in q) {
                    count++;
                    //Console.WriteLine(JSON.ToJSON(db.Find("test_copy", new { id = item.Path("id") })));
                    Console.WriteLine(JSON.ToJSON(item));
                    if (count == max) {
                        break;
                    }
                }
                end("普通for");
            }
            {
                begin(10000);
                System.Threading.Tasks.Parallel.ForEach(q, (p, forState) => {
                    int n = System.Threading.Interlocked.Increment(ref count);
                    Console.WriteLine($"【{n}】{System.Threading.Thread.CurrentThread.ManagedThreadId}:{JSON.ToJSON(p)}");
                    if (n == max) {
                        forState.Break();
                        end("并行for");
                    }
                });
            }
            print();
            Console.WriteLine("continue ...");
            Console.ReadKey();
        }


        static void QueryGeneric(IDataContext db) {
            //t_User
            foreach (var item in db.FindAll<t_User>("t_user")) {
                Console.WriteLine($"{JSON.ToJSON(item)}");
            }
            Console.WriteLine("continue ...");
            Console.ReadKey();
        }

    }
}

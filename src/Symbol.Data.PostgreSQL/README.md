### 程序集    [使用说明](https://github.com/symbolspace/Symbol.Data/wiki/Home)
Symbol.Data.PostgreSQL.dll [![Available on NuGet https://www.nuget.org/packages/Symbol.Data.PostgreSQL/](https://img.shields.io/nuget/v/Symbol.Data.PostgreSQL.svg?style=flat)](https://www.nuget.org/packages/Symbol.Data.PostgreSQL/)

运行时支持：
* .net framework 2.0
* .net framework 3.5
* .net framework 4.0
* .net framework 4.5
* .net framework 4.6
* .net framework 4.6.1
* .net framework 4.7
* .net framework 4.8
* .net standard 2.0
* .net core app 3.1
* .net 5.0
* .net 6.0
* .net 7.0
* .net 8.0


### 创建IDataContext [更多使用说明](https://github.com/symbolspace/Symbol.Data/wiki/Home)
```csharp
//连接参数，可以是json字符串、匿名对象、IDictionary<string, object>
//省去一些记不住的连接参数，只需要设置关键的参数即可
var connectionOptions = new {
    host = "localhost",        //服务器
    port = 5432,               //端口为默认时，可以不用写
    name = "test",             //数据库名称
    account = "test",          //登录账号
    password = "test",         //登录密码
};

//db 类型：Symbol.Data.IDataContext
var db = Symbol.Data.Provider.CreateDataContext("pgsql", connectionOptions);

//也可以这样构建
//var db = return new Symbol.Data.PostgreSQLProvider().CreateDataContext(connectionOptions);

```

### 各个运行时引用说明
* .net framework 2.0
> Npgsql *v2.2.7*

* .net framework 3.5
> Npgsql *v2.2.7*

* .net framework 4.0
> Npgsql *v2.2.7*

* .net framework 4.5~4.8
> Npgsql *v4.1.14*

* .net standard 2.0
> Npgsql *v5.0.18*

* .net core app 3.1
> Npgsql *v5.0.18*

* .net 5.0
> Npgsql *v5.0.18*

* .net 6.0
> Npgsql *v6.0.4.816*

* .net 7.0
> Npgsql *v6.0.12*

* .net 8.0
> Npgsql *v8.0.4*

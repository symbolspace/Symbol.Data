### 程序集    [使用说明](https://github.com/symbolspace/Symbol.Data/wiki/Home)
> 运行时支持 .net framework v2.0/3.5/4.0/4.5、.net standard 2.x、.net core app 2.x。

* Symbol.Data.SQLite.dll [![Available on NuGet https://www.nuget.org/packages/Symbol.Data.SQLite/](https://img.shields.io/nuget/v/Symbol.Data.SQLite.svg?style=flat)](https://www.nuget.org/packages/Symbol.Data.SQLite/)


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
//连接参数直接为文件
var connectionOptions = AppHelper.MapPath("~/test.db");

//内存数据库时：可以是json字符串、匿名对象、IDictionary<string, object>
//var connectionOptions = new {
//     name = "test",
//     memory = true
//}

//db 类型：Symbol.Data.IDataContext
var db = Symbol.Data.Provider.CreateDataContext("pgsql", connectionOptions);

//也可以这样构建
//var db = return new Symbol.Data.SQLiteProvider().CreateDataContext(connectionOptions);

//如果是文件模式，请调用一下CreateFile，用于创建空文件。
//Symbol.Data.SQLite.SQLiteHelper.CreateFile(file);

```

### 各个运行时引用说明
* .net framework v2.0/v3.5
> System.Data.SQLite.Core *v1.0.111*

* .net framework v4.0/v4.5
> System.Data.SQLite.Core *v1.0.111*

* .net standard v2.x / .net core app v2.x
> System.Data.SQLite.Core *v1.0.111* **[core]**


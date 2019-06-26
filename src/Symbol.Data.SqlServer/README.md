### 程序集    [使用说明](https://github.com/symbolspace/Symbol.Data/wiki/Home)
> 运行时支持 .net framework v2.0/3.5/4.0/4.5、.net standard 2.x、.net core app 2.x。

* Symbol.Data.SqlServer.dll [![Available on NuGet https://www.nuget.org/packages/Symbol.Data.SqlServer/](https://img.shields.io/nuget/v/Symbol.Data.SqlServer.svg?style=flat)](https://www.nuget.org/packages/Symbol.Data.SqlServer/)


### 创建IDataContext [更多使用说明](https://github.com/symbolspace/Symbol.Data/wiki/Home)
```csharp
//连接参数，可以是json字符串、匿名对象、IDictionary<string, object>
//省去一些记不住的连接参数，只需要设置关键的参数即可
var connectionOptions = new {
    host = "localhost",        //服务器
    port = 1433,               //端口为默认时，可以不用写
    name = "test",             //数据库名称
    account = "test",          //登录账号
    password = "test",         //登录密码
};

//db 类型：Symbol.Data.IDataContext
//2008及以下请传mssql2005
var db = Symbol.Data.Provider.CreateDataContext("mssql2012", connectionOptions);

//也可以这样构建
//2008及以下请使用SqlServer2005Provider
//var db = return new Symbol.Data.SqlServer2012Provider().CreateDataContext(connectionOptions);

```

### 各个运行时引用说明
* .net framework v2.0/v3.5/v4.0/v4.5
> System.Data

* .net standard v2.x / .net core app v2.x
> System.Data.SqlClient *v4.5.0* **[core]**


### 程序集    [使用说明](https://github.com/symbolspace/Symbol.Data/wiki/Home)
Symbol.Data.Kingbase.dll [![Available on NuGet https://www.nuget.org/packages/Symbol.Data.Kingbase/](https://img.shields.io/nuget/v/Symbol.Data.Kingbase.svg?style=flat)](https://www.nuget.org/packages/Symbol.Data.Kingbase/)

运行时运行
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
    port = 54321,              //端口为默认时，可以不用写
    name = "test",             //数据库名称
    account = "test",          //登录账号
    password = "test",         //登录密码
};

//db 类型：Symbol.Data.IDataContext
var db = Symbol.Data.Provider.CreateDataContext("kingbase", connectionOptions);

//也可以这样构建
//var db = return new Symbol.Data.KingbaseProvider().CreateDataContext(connectionOptions);

```

### 各个运行时引用说明
* .net framework 4.0
> Kdbndp_V9 *v4.0.1.10*

* .net framework 4.5~4.8
> Kdbndp_V9 *v4.5.0*

* .net standard 2.0
> Kdbndp_V9 *v2.0.0.10*

* .net core app 3.1
> Kdbndp_V9 *v3.1.2.10*

* .net 5.0
> Kdbndp_V9 *v5.0.0.816*

* .net 6.0
> Kdbndp_V9 *v6.0.4.816*

* .net 7.0
> Kdbndp_V9 *v7.0.0*

* .net 8.0
> Kdbndp_V9 *v8.0.1.10*

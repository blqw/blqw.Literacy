# 使用IL.Emit方式快速访问属性,字段和方法   

## 特色  
### 全,易,快
功能强大  
上手简单  
性能优异  

## 与反射的性能比较  

>MethodInfo 循环 1000000 次  
>运行时间    CPU时钟周期    垃圾回收( 1代      2代      3代 )    
>209ms       413,287,378              0        0        0     
>                                                           
>Literacy 循环 1000000 次                                      
>运行时间    CPU时钟周期    垃圾回收( 1代      2代      3代 )   
>52ms        103,657,391              0        0        0     
>                                                           
>dynamic 循环 1000000 次                                       
>运行时间    CPU时钟周期    垃圾回收( 1代      2代      3代 )   
>46ms        92,546,226               0        0        0     

#### 性能测试代码
```csharp
static void Main(string[] args)
{
    User u = new User();
    CodeTimer.Initialize();
    CodeTimer.Time("MethodInfo", 1000000, () => GetName2(u));
    CodeTimer.Time("Literacy", 1000000, () => GetName(u));
    CodeTimer.Time("dynamic", 1000000, () => GetName3(u));
}

static ObjectProperty prop;

public static object GetName(object obj)
{
    if (obj == null) throw new ArgumentNullException("obj");
    if (prop == null)
    {
        prop = new Literacy(obj.GetType()).Property["Name"];
        if (prop == null) throw new NotSupportedException("对象不包含Name属性");
    }
    return prop.GetValue(obj);
}

static MethodInfo getName;

public static object GetName2(object obj)
{
    if (obj == null) throw new ArgumentNullException("obj");
    if (getName == null)
    {
        getName = obj.GetType().GetProperty("Name").GetGetMethod();
    }
    return getName.Invoke(obj, null); //缓存了反射Name属性
}

public static object GetName3(object obj)
{
    if (obj == null) throw new ArgumentNullException("obj");
    return ((dynamic)obj).Name;
}
```

## 更新说明  
#### 2016.03.16  
* 更新MEF  
  
#### 2016.02.23  
* 修复bug  

#### 2016.02.21  
* 增加导入插件 `CreateGetter`,`CreateSetter`,`CreateCaller`

#### 2016.02.20  
* 优化IoC模块  

#### 2015.11.06
* 将 Convert3 独立  
* 内置MEF模块,用于IOC方式载入Convert3  

#### 2015.06.11  
* 增加对匿名类的支持,支持包括匿名类的属性赋值和new操作  

#### 2015.06.09  
* 签入Convert3项目 代替原有类型转换方案 删除部分不常用的特性  

#### 2014.12.25  
* ObjectProperty中增加 MappingName 属性  
* Convert2中增加对象转对象和对象转DataTable的相关方法  

#### 2014.12.12
* 在 Convert2 中增加全角转半角 半角转全角的方法  

#### 2014.12.04
* 增加接口 `IMemberMappingAttributre` 用于控制对象成员名称的映射关系,对象成员特性如果实现该接口,则`Convert2`中转换实体的方法优先考虑映射名称

#### 2014.10.10
* 日常维护,优化了StringToGuid的逻辑,优化性能

#### 2014.09.22  
* 正式版发布

#### 2014.09.14  
* 修正Convert2枚举类型转换中的bug
* 修正Convert2可空值类型转换中的bug
* 修正Convert2实体转换中的bug
* 修正Convert2转换object类型中的bug
* 修正Literacy在反射式会忽略系统自动生成字段的问题
  * ObjectProperty中增加AutoField属性,用于判断是否为自动生成的字段
  * ObjectPropertyCollection 循环时(foreach)不会出现自动字段
* 修正字段可赋值判断为是否常量(之前是判断是否Readonly,事实证明Readonly的字段也是可以赋值的)
* 修改TypeCodes枚举,支持更多类型

#### 2014.08.31  
* 增加静态类 Convert2, 用于转换对象  
* TypeInfo增加TryParse和Convert方法 用于转换对象   
  
#### 2014.08.19  
* 增加静态类 TypesHelper, 存放用于处理Type对象的静态方法  
* 增加密封类 TypeInfo, 用于拓展系统Type对象的属性和方法  
* 修改Literacy类中的缓存,将操作转移至TypesHelper  
* 修改TypeCodes的计算方法,将操作转移至TypeInfo  
  
#### 2014.07.30  
* 为Literacy和ObjectProperty实体增加ID(自增标识,2个类共享自增序列)和UID(全球唯一标识符GUID)方便在做缓存的时候作为key使用  
* 增加TypeCodes枚举,用于扩展系统的TypeCode 会稍微影响构造Literacy的速度,但是由于Literacy本身是全局缓存,所以影响不大  
  
#### 2014.07.29  
* 增加对特性的支持,现在可以从 Literacy 或 Literacy.Property 或 Literacy.Field 直接使用Attributes属性访问  

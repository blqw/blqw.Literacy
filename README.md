# 使用IL.Emit方式快速访问属性,字段和方法   

## 特色  
### 全,易,快
功能强大  
上手简单  
性能优异  

## 与反射的性能比较  

### 测试1  
<table>
<tr><th>测试内容</th><th>循环次数</th><th>运行时间</th><th>CPU时钟周期</th></tr>
<tr><td> PropertyInfo.GetValue() </td><td> 1000000 </td><td> 204ms </td><td> 467,083,802 </td></tr>
<tr><td> dynamic </td><td> 1000000 </td><td> 41ms </td><td> 92,844,899 </td></tr>
<tr><td> Literacy </td><td> 1000000 </td><td> 28ms </td><td> 65,759,428 </td></tr>
</table>
### 测试2  
<table>
<tr><th>测试内容</th><th>循环次数</th><th>初始化时间</th><th>运行时间</th><th>CPU时钟周期</th></tr>
<tr><td> Lambda.Compile() </td><td> 1000000 </td><td> 1.7693ms </td><td> 33ms </td><td> 77,217,274 </td></tr>
<tr><td> CreateDelegate(GetGetMethod()) </td><td> 1000000 </td><td> 1.8108ms </td><td> 29ms </td><td> 66,729,503 </td></tr>
<tr><td> Literacy </td><td> 1000000 </td><td> 1.6712ms </td><td> 12ms </td><td> 28,517,667 </td></tr>
</table>

#### [性能测试代码](https://github.com/blqw/blqw.Literacy/blob/master/Demo/Program.cs)

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

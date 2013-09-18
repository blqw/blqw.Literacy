
namespace blqw
{
    /// <summary> 获取对象属性或字段值的委托
    /// </summary>
    public delegate object LiteracyGetter(object obj);
    /// <summary> 设置对象属性或字段值的委托,返回是否设置成功
    /// </summary>
    public delegate void LiteracySetter(object obj, object value);
    /// <summary> 执行对象方法的委托,返回object对象
    /// </summary>
    public delegate object LiteracyCaller(object obj, params object[] args);

    /// <summary> 执行对象构造函数的委托,返回object对象
    /// </summary>
    public delegate object LiteracyNewObject(params object[] args);

}

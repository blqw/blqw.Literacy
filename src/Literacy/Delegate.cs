
namespace blqw.Reflection
{
    /// <summary> 
    /// 获取对象属性或字段值的委托
    /// </summary>
    /// <param name="obj">实例对象</param>
    public delegate object MemberGetter(object obj);

    /// <summary>
    /// 设置对象属性或字段值的委托
    /// </summary>
    /// <param name="obj">实例对象</param>
    /// <param name="value">需要设置的值</param>
    public delegate void MamberSetter(object obj, object value);

    /// <summary>
    /// 执行对象方法的委托,返回object对象
    /// </summary>
    /// <param name="obj">实例对象</param>
    /// <param name="args">方法参数</param>
    public delegate object MethodInvoker(object obj, params object[] args);

    /// <summary> 
    /// 执行对象构造函数的委托,返回object对象
    /// </summary>
    /// <param name="args">构造器参数</param>
    public delegate object ObjectConstructor(params object[] args);

}

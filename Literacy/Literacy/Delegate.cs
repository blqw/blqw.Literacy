
namespace blqw
{

    /// <summary> 获取对象属性或字段值的委托
    /// </summary>
    /// <param name="obj">实例对象</param>
    public delegate object LiteracyGetter(object obj);

    /// <summary> 设置对象属性或字段值的委托
    /// </summary>
    /// <param name="obj">实例对象</param>
    /// <param name="value">需要设置的值</param>
    public delegate void LiteracySetter(object obj, object value);

    /// <summary> 执行对象方法的委托,返回object对象
    /// </summary>
    /// <param name="obj">实例对象</param>
    /// <param name="args">方法参数</param>
    public delegate object LiteracyCaller(object obj, params object[] args);

    /// <summary> 执行对象构造函数的委托,返回object对象
    /// </summary>
    /// <param name="args">构造器参数</param>
    public delegate object LiteracyNewObject(params object[] args);

    /// <summary> 转换对象的委托,返回是否转换成功
    /// </summary>
    /// <param name="input">包含要转换的对象</param>
    /// <param name="result">当此方法返回时，如果转换成功，则包含转换后的值</param>
    public delegate bool LiteracyTryParse(object input, out object result);

    /// <summary> 转换对象的委托,返回是否转换成功
    /// </summary>
    /// <param name="input">包含要转换的对象</param>
    /// <param name="result">当此方法返回时，如果转换成功，则包含转换后的值</param>
    public delegate bool LiteracyTryParse<T>(object input, out T result);
}

using System;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 标记一个AI函数的占位方法（定义在CRUDVM或普通类上）。
    /// 方法体固定写一行转发：AiCodeRunner.Invoke&lt;T&gt;(this, "函数名", 固定参数字典)，
    /// 真实逻辑由AI生成后保存在模型上标记了 [AiCode] 的字段中，运行时动态编译执行。
    /// 特性上的描述信息会自动拼入AI生成的提示词。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AiFuncAttribute : Attribute
    {
        /// <summary>
        /// 函数名，与 [AiCode]/[AiArgs] 的 FuncName 及AI生成的方法名对应，同一模型内唯一
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 函数用途的中文描述，告诉AI这个函数要完成什么业务
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 返回值含义说明，如 "true=发送，false=过滤掉"
        /// </summary>
        public string ReturnDes { get; set; }
    }

    /// <summary>
    /// 标记AI函数占位方法的某个参数，提供参数的中文说明，
    /// 反射后自动拼入AI生成的提示词
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class AiParamAttribute : Attribute
    {
        /// <summary>
        /// 参数中文说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="description">参数中文说明</param>
        public AiParamAttribute(string description)
        {
            Description = description;
        }
    }

    /// <summary>
    /// 标记模型上的代码字段，存放AI生成的C#方法代码；
    /// FuncName 与 [AiFunc].Name 对应，一个模型可以有多对不同函数的代码/参数字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AiCodeAttribute : Attribute
    {
        /// <summary>
        /// 对应的AI函数名
        /// </summary>
        public string FuncName { get; set; }
    }

    /// <summary>
    /// 标记模型上的参数定义字段，存放AI生成时设计的附加参数定义
    /// （JSON数组：argName/argType/argDes/argDF 四元组）；
    /// FuncName 与 [AiCode] 的 FuncName 配对
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AiArgsAttribute : Attribute
    {
        /// <summary>
        /// 对应的AI函数名
        /// </summary>
        public string FuncName { get; set; }
    }
}

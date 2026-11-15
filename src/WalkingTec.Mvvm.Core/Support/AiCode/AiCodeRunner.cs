using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core.Support.AiCode
{
    /// <summary>
    /// AI函数运行器：占位函数体中的一行转发落到这里。
    /// 职责：定位模型上的 [AiCode]/[AiArgs] 字段 → 合并参数 → 编译（带缓存）→
    /// 按参数名绑定（固定参数 + AI附加参数，AiContext 特殊注入）→ 带超时执行
    /// </summary>
    public static class AiCodeRunner
    {
        /// <summary>
        /// 默认执行超时（毫秒），可通过 AiCodeOptions.TimeoutSeconds 配置
        /// </summary>
        public static int TimeoutMs { get; set; } = 5000;

        /// <summary>
        /// 从CRUDVM调用（占位函数的标准写法）：
        /// => AiCodeRunner.Invoke&lt;T&gt;(Wtm, Entity, "函数名", new Dictionary&lt;string,object&gt;{ ["参数名"]=值 })
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="vm">CRUDVM实例</param>
        /// <param name="funcName">AI函数名</param>
        /// <param name="fixedParams">固定参数字典（键=脚本方法参数名）</param>
        /// <param name="argOverrides">AI附加参数的覆盖值（如下游业务填值控件产出的值），null则全部用默认值</param>
        /// <param name="logger">运行日志回调（脚本通过ctx.Log记录的内容）</param>
        /// <returns>函数返回值</returns>
        public static T Invoke<T>(BaseVM vm, string funcName,
            Dictionary<string, object> fixedParams = null,
            Dictionary<string, object> argOverrides = null,
            Action<string> logger = null)
        {
            if (vm == null)
                throw new ArgumentNullException(nameof(vm));
            //Entity定义在BaseCRUDVM<T>上，用dynamic取值避免泛型协变问题
            var entity = (vm as dynamic).Entity;
            return Invoke<T>(vm.Wtm, entity, funcName, fixedParams, argOverrides, logger);
        }

        /// <summary>
        /// 通用调用入口：代码与参数定义取自实体上标记 [AiCode(FuncName)]/[AiArgs(FuncName)] 的字段
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="wtm">WTM上下文（可为null，仅供脚本通过ctx访问）</param>
        /// <param name="entity">持有AI代码字段的模型实体</param>
        /// <param name="funcName">AI函数名</param>
        /// <param name="fixedParams">固定参数字典（键=脚本方法参数名）</param>
        /// <param name="argOverrides">AI附加参数覆盖值</param>
        /// <param name="logger">运行日志回调</param>
        /// <returns>函数返回值</returns>
        public static T Invoke<T>(WTMContext wtm, object entity, string funcName,
            Dictionary<string, object> fixedParams = null,
            Dictionary<string, object> argOverrides = null,
            Action<string> logger = null)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (string.IsNullOrWhiteSpace(funcName))
                throw new ArgumentException("函数名不能为空", nameof(funcName));

            var entityType = entity.GetType();
            var (codeProp, argsProp) = FindCodeFields(entityType, funcName);
            if (codeProp == null)
                throw new InvalidOperationException($"模型 {entityType.Name} 上没有标记 [AiCode(FuncName=\"{funcName}\")] 的字段");

            var code = codeProp.GetValue(entity) as string;
            var argsJson = argsProp?.GetValue(entity) as string;

            var result = InvokeCode<T>(wtm, code, funcName, entityType, fixedParams, argsJson, argOverrides, logger);
            return result;
        }

        /// <summary>
        /// 直接执行一段代码（供校验后调用等场景），不走实体字段定位
        /// </summary>
        public static T InvokeCode<T>(WTMContext wtm, string code, string funcName, Type modelType,
            Dictionary<string, object> fixedParams = null,
            string argsJson = null,
            Dictionary<string, object> argOverrides = null,
            Action<string> logger = null)
        {
            //1. 编译（带缓存）
            var compile = AiCodeCompiler.Compile(code, funcName, modelType);
            if (compile.Success == false)
            {
                var detail = compile.BlacklistHit != null
                    ? $"黑名单拦截：{compile.BlacklistHit}"
                    : string.Join("\n", compile.Diagnostics);
                throw new InvalidOperationException($"AI代码校验未通过\n{detail}");
            }

            //2. 合并AI附加参数：定义默认值 + 调用时覆盖值
            var defs = AiArgParser.ParseDefinitions(argsJson);
            var argValues = AiArgParser.BuildValues(defs, argOverrides);

            //3. 按脚本方法参数名绑定
            var method = compile.Method;
            var ctx = new AiContext { Wtm = wtm };
            var parameters = method.GetParameters();
            var bound = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var p = parameters[i];
                if (p.ParameterType == typeof(AiContext))
                {
                    bound[i] = ctx;
                    continue;
                }
                if (fixedParams != null && fixedParams.TryGetValue(p.Name, out var fixedVal))
                {
                    bound[i] = AiArgParser.ConvertToType(fixedVal, p.ParameterType, p.Name);
                    continue;
                }
                if (argValues.TryGetValue(p.Name, out var argVal))
                {
                    bound[i] = AiArgParser.ConvertToType(argVal, p.ParameterType, p.Name);
                    continue;
                }
                if (p.HasDefaultValue)
                {
                    bound[i] = p.DefaultValue;
                    continue;
                }
                throw new InvalidOperationException(
                    $"脚本方法声明了参数 {p.Name} ({p.ParameterType.Name})，但固定参数和AI参数定义中都没有提供该参数的值");
            }

            //4. 带超时执行（脚本内死循环时超时抛异常；线程无法真正回收，见框架文档的安全说明）
            var timeout = Math.Max(1000, TimeoutMs);
            var task = Task.Run(() =>
            {
                try
                {
                    return method.Invoke(null, bound);
                }
                catch (TargetInvocationException tie)
                {
                    throw tie.InnerException ?? tie;
                }
            });
            var finished = Task.WhenAny(task, Task.Delay(timeout)).GetAwaiter().GetResult();
            if (finished != task)
                throw new TimeoutException($"AI代码执行超时（{timeout}ms）");

            //5. 日志回调
            logger?.Invoke(ctx.Log.ToString());

            //6. 返回值转换
            var raw = task.Result;
            if (raw == null)
                return default;
            var targetType = typeof(T);
            var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;
            if (underlying.IsInstanceOfType(raw))
                return (T)raw;
            return (T)AiArgParser.ConvertToType(raw, underlying, "返回值");
        }

        /// <summary>
        /// 定位模型上与函数名配对的代码/参数字段
        /// </summary>
        private static (PropertyInfo codeProp, PropertyInfo argsProp) FindCodeFields(Type entityType, string funcName)
        {
            PropertyInfo codeProp = null;
            PropertyInfo argsProp = null;
            foreach (var prop in entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var codeAttr = prop.GetCustomAttribute<AiCodeAttribute>();
                if (codeAttr != null && string.Equals(codeAttr.FuncName, funcName, StringComparison.OrdinalIgnoreCase))
                {
                    if (codeProp != null)
                        throw new InvalidOperationException($"模型 {entityType.Name} 上有多个 [AiCode(FuncName=\"{funcName}\")] 字段，只能有一个");
                    codeProp = prop;
                }
                var argsAttr = prop.GetCustomAttribute<AiArgsAttribute>();
                if (argsAttr != null && string.Equals(argsAttr.FuncName, funcName, StringComparison.OrdinalIgnoreCase))
                {
                    if (argsProp != null)
                        throw new InvalidOperationException($"模型 {entityType.Name} 上有多个 [AiArgs(FuncName=\"{funcName}\")] 字段，只能有一个");
                    argsProp = prop;
                }
            }
            return (codeProp, argsProp);
        }

        /// <summary>
        /// 获取模型上某函数的代码/参数字段当前值（供外部读取展示）
        /// </summary>
        public static (string code, string argsJson) GetCodeAndArgs(object entity, string funcName)
        {
            var (codeProp, argsProp) = FindCodeFields(entity.GetType(), funcName);
            return (codeProp?.GetValue(entity) as string, argsProp?.GetValue(entity) as string);
        }
    }
}

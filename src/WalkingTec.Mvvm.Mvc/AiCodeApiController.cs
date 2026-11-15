using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Support.AiCode;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// AI代码API：代码生成（生成后自动试编译+错误回传重试+AI安全审查）与校验
    /// （编译+黑名单+AI审查，保存前由前端控件调用）。
    /// 接口本身不执行代码，执行入口在业务代码中通过 AiCodeRunner 调用
    /// </summary>
    [AuthorizeJwtWithCookie]
    [ApiController]
    [Route("api/aicode")]
    [ActionDescription("MenuKey.AiCode")]
    [AllRights]
    public class AiCodeApiController : BaseApiController
    {
        /// <summary>
        /// 按VM类型与函数名生成AI代码
        /// </summary>
        /// <param name="input">vmType（CRUDVM类型全名）、funcName、prompt（自然语言需求）</param>
        /// <returns>代码、参数定义、编译诊断与审查结论</returns>
        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] AiCodeGenerateInput input)
        {
            if (string.IsNullOrWhiteSpace(input?.Prompt))
                return BadRequest("请输入需求描述（提示词）");
            var conf = ConfigInfo?.AiCode;
            if (conf?.Enabled == false)
                return BadRequest("AI代码功能未启用（AiCode:Enabled=false）");

            var vmType = ResolveVmType(input.VmType);
            if (vmType == null)
                return BadRequest($"找不到VM类型：{input.VmType}（需要CRUDVM类的FullName）");

            var llm = Wtm?.ServiceProvider?.GetService(typeof(LlmService)) as LlmService;
            if (llm == null)
                return BadRequest("系统未配置大模型服务（Llm节），无法生成代码");

            var result = await AiCodePromptBuilder.GenerateAsync(vmType, input.FuncName, input.Prompt, Wtm);
            return Ok(result);
        }

        /// <summary>
        /// 校验代码（不执行）：编译诊断 + 黑名单拦截 + AI危险代码审查 + 参数定义解析。
        /// 前端保存前应调用本接口，全部通过再保存。
        /// 编辑器每次保存都会调用，高频校验不记录操作日志（NoLog）
        /// </summary>
        /// <param name="input">vmType、funcName、code、args（参数定义JSON）</param>
        /// <returns>校验结果</returns>
        [NoLog]
        [HttpPost("validate")]
        public async Task<IActionResult> Validate([FromBody] AiCodeValidateInput input)
        {
            if (string.IsNullOrWhiteSpace(input?.Code))
                return BadRequest("代码不能为空");
            var conf = ConfigInfo?.AiCode;
            if (conf?.Enabled == false)
                return BadRequest("AI代码功能未启用（AiCode:Enabled=false）");

            var vmType = ResolveVmType(input.VmType);
            if (vmType == null)
                return BadRequest($"找不到VM类型：{input.VmType}");

            //1. 参数定义解析（容错提示）
            var defs = AiArgParser.ParseDefinitions(input.Args);
            var argsJsonValid = string.IsNullOrWhiteSpace(input.Args) || defs.Count > 0 || input.Args.Trim() == "[]";

            //2. 编译 + 黑名单
            var modelType = GetModelType(vmType);
            var compile = AiCodeCompiler.Compile(input.Code, input.FuncName, modelType);

            //3. AI安全审查（仅编译通过后做，节约调用）
            AiReviewResult review = null;
            if (compile.Success && argsJsonValid)
            {
                var llm = Wtm?.ServiceProvider?.GetService(typeof(LlmService)) as LlmService;
                review = await AiCodePromptBuilder.ReviewAsync(input.Code, llm);
                //审查服务不可用且配置强制审查：返回不可保存
                if (review.ServiceAvailable == false && conf?.ReviewRequired == true)
                {
                    return Ok(new
                    {
                        compileOk = true,
                        blacklistHit = (string)null,
                        diagnostics = compile.Diagnostics,
                        argsOk = argsJsonValid,
                        canSave = false,
                        review
                    });
                }
            }

            var canSave = compile.Success && argsJsonValid && (review?.Dangerous != true);
            return Ok(new
            {
                compileOk = compile.Success,
                blacklistHit = compile.BlacklistHit,
                diagnostics = compile.Diagnostics,
                argsOk = argsJsonValid,
                canSave,
                review
            });
        }

        /// <summary>
        /// 获取VM上某AI函数的元数据（函数描述/参数表/模型字段），供前端编辑器加载时展示。
        /// 纯名称类数据且每次打开编辑器都会拉取：登录即可访问（AllRights），不记录操作日志（NoLog）
        /// </summary>
        /// <param name="vmType">VM类型全名</param>
        /// <param name="funcName">函数名</param>
        /// <returns>函数元数据</returns>
        [AllRights]
        [NoLog]
        [HttpGet("meta")]
        public IActionResult GetMeta(string vmType, string funcName)
        {
            var type = ResolveVmType(vmType);
            if (type == null)
                return BadRequest($"找不到VM类型：{vmType}");
            foreach (var m in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                var attr = m.GetCustomAttribute<AiFuncAttribute>();
                if (attr == null || !string.Equals(attr.Name, funcName, StringComparison.OrdinalIgnoreCase))
                    continue;
                return Ok(new
                {
                    attr.Name,
                    attr.Description,
                    attr.ReturnDes,
                    ReturnType = AiCodePromptBuilder.TypeName(m.ReturnType,
                        m.ReturnParameter?.GetCustomAttribute<System.Runtime.CompilerServices.TupleElementNamesAttribute>()?.TransformNames),
                    ModelType = GetModelType(type)?.Name,
                    Params = m.GetParameters().Select(p => new
                    {
                        p.Name,
                        Type = AiCodePromptBuilder.TypeName(p.ParameterType,
                            p.GetCustomAttribute<System.Runtime.CompilerServices.TupleElementNamesAttribute>()?.TransformNames),
                        Des = p.GetCustomAttribute<AiParamAttribute>()?.Description
                    }).ToList()
                });
            }
            return BadRequest($"类型 {type.Name} 上找不到 [AiFunc(Name=\"{funcName}\")] 方法");
        }

        /// <summary>
        /// 按全名解析VM类型：遍历已加载程序集，必须是BaseVM派生
        /// </summary>
        private static Type ResolveVmType(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return null;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type t = null;
                try { t = asm.GetType(fullName, false); } catch { }
                if (t != null && typeof(BaseVM).IsAssignableFrom(t))
                    return t;
            }
            return null;
        }

        /// <summary>
        /// 从VM继承链取 BaseCRUDVM&lt;T&gt; 的模型类型
        /// </summary>
        private static Type GetModelType(Type vmType)
        {
            var t = vmType.BaseType;
            while (t != null)
            {
                if (t.IsGenericType && t.GetGenericTypeDefinition().Name.StartsWith("BaseCRUDVM"))
                    return t.GetGenericArguments()[0];
                t = t.BaseType;
            }
            return null;
        }
    }

    /// <summary>
    /// 生成接口请求体
    /// </summary>
    public class AiCodeGenerateInput
    {
        /// <summary>CRUDVM类型全名（含命名空间）</summary>
        public string VmType { get; set; }
        /// <summary>AI函数名（[AiFunc].Name）</summary>
        public string FuncName { get; set; }
        /// <summary>自然语言需求描述</summary>
        public string Prompt { get; set; }
    }

    /// <summary>
    /// 校验接口请求体
    /// </summary>
    public class AiCodeValidateInput
    {
        /// <summary>CRUDVM类型全名</summary>
        public string VmType { get; set; }
        /// <summary>AI函数名</summary>
        public string FuncName { get; set; }
        /// <summary>方法代码。加JsonStringConverter防止框架LTGT字符串过滤剥掉代码中的比较运算符&lt;和&gt;</summary>
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringConverter))]
        public string Code { get; set; }
        /// <summary>参数定义JSON</summary>
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringConverter))]
        public string Args { get; set; }
    }
}

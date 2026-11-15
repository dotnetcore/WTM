using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WalkingTec.Mvvm.Core.Support.AiCode;

namespace WalkingTec.Mvvm.Core.Test
{
    /// <summary>
    /// 测试用模型：带 [AiCode]/[AiArgs] 字段对（多函数多对）
    /// </summary>
    public class TestAiModel : TopBasePoco
    {
        /// <summary>
        /// 业务名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 输入的原始数值
        /// </summary>
        public double? InputValue { get; set; }

        [AiCode(FuncName = "Calc")]
        public string Code { get; set; }

        [AiArgs(FuncName = "Calc")]
        public string Args { get; set; }

        [AiCode(FuncName = "Check")]
        public string CheckCode { get; set; }

        [AiArgs(FuncName = "Check")]
        public string CheckArgs { get; set; }

        [AiCode(FuncName = "Process")]
        public string ProcessCode { get; set; }

        [AiArgs(FuncName = "Process")]
        public string ProcessArgs { get; set; }

        //故意不加任何注释与特性：测试名称推断兜底路径
        public string remarks { get; set; }
    }

    /// <summary>
    /// 带完整XML注释的返回值DTO，用于测试提示词自动组装
    /// </summary>
    public class TestAiResult
    {
        /// <summary>
        /// 计算得到的结果数值
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// 处理过程的中文说明
        /// </summary>
        public string Log { get; set; }

        /// <summary>
        /// 结果是否需要人工复核
        /// </summary>
        public bool NeedReview { get; set; }
    }

    /// <summary>
    /// 测试用CRUDVM：带 [AiFunc] 占位函数（元组/DTO返回/推断兜底多种形态）
    /// </summary>
    public class TestAiVM : BaseCRUDVM<TestAiModel>
    {
        [AiFunc(Name = "Calc", Description = "按倍数计算输入值", ReturnDes = "计算结果")]
        public double Calc([AiParam("输入值")] double input)
            => AiCodeRunner.Invoke<double>(Wtm, Entity, "Calc",
                 new Dictionary<string, object> { ["input"] = input });

        /// <summary>
        /// 校验输入值并返回计算结果与告警标记
        /// </summary>
        /// <param name="input">输入的原始数值，业务上允许为负数</param>
        /// <returns>value为计算后的数值，alert为是否触发告警</returns>
        [AiFunc(Name = "Check")]
        public (double value, bool alert) Check([AiParam("输入值")] double input)
            => AiCodeRunner.Invoke<(double, bool)>(Wtm, Entity, "Check",
                 new Dictionary<string, object> { ["input"] = input });

        /// <summary>
        /// 按业务规则处理输入并返回结构化结果
        /// </summary>
        /// <param name="input">输入的原始数值</param>
        /// <returns>包含数值、说明与复核标记的结果对象</returns>
        [AiFunc(Name = "Process")]
        public TestAiResult Process([AiParam("输入值")] double input)
            => AiCodeRunner.Invoke<TestAiResult>(Wtm, Entity, "Process",
                 new Dictionary<string, object> { ["input"] = input });

        /// <summary>
        /// 无特性标注的函数，测试名称推断兜底
        /// </summary>
        /// <param name="maxRetryCount">最大重试次数</param>
        /// <param name="isNeedAlert">告警开关</param>
        /// <returns>处理说明</returns>
        [AiFunc(Name = "Guess")]
        public string GuessTest(int maxRetryCount, bool isNeedAlert)
            => AiCodeRunner.Invoke<string>(Wtm, Entity, "Guess",
                 new Dictionary<string, object> { ["maxRetryCount"] = maxRetryCount, ["isNeedAlert"] = isNeedAlert });

        public TestAiVM() { }
    }

    /// <summary>
    /// AI代码功能单元测试：参数解析/类型转换、动态编译、
    /// 黑名单拦截、缓存、按名绑定、上下文注入与运行器
    /// </summary>
    [TestClass]
    public class AiCodeTest
    {
        private const string CalcCode = @"
public static double Calc(double input, int scale, AiContext ctx)
{
    if (input < 0)
    {
        ctx.WriteLog(""输入为负数，按0处理"");
        input = 0;
    }
    else
    {
        ctx.WriteLog(""输入正常"");
    }
    return input * scale;
}";

        private const string CalcArgs = @"[{""argName"":""scale"",""argType"":""int"",""argDes"":""倍数"",""argDF"":3}]";

        #region AiArgParser

        [TestMethod]
        public void ParseDefinitionsTest()
        {
            var defs = AiArgParser.ParseDefinitions(CalcArgs);
            Assert.AreEqual(1, defs.Count);
            Assert.AreEqual("scale", defs[0].argName);
            Assert.AreEqual("int", defs[0].argType);
            Assert.AreEqual("倍数", defs[0].argDes);
            Assert.AreEqual(3, Convert.ToInt32(AiArgParser.ConvertToType(defs[0].argDF, typeof(int), "scale")));

            //空串与非法JSON容错
            Assert.AreEqual(0, AiArgParser.ParseDefinitions(null).Count);
            Assert.AreEqual(0, AiArgParser.ParseDefinitions("").Count);
            Assert.AreEqual(0, AiArgParser.ParseDefinitions("not json").Count);
            //带markdown围栏容错
            var fenced = AiArgParser.ParseDefinitions("```json\n[{\"argName\":\"a\",\"argType\":\"int\",\"argDes\":\"x\",\"argDF\":1}]\n```");
            Assert.AreEqual(1, fenced.Count);
        }

        [TestMethod]
        public void BuildValuesTest()
        {
            var defs = AiArgParser.ParseDefinitions(CalcArgs);
            var values = AiArgParser.BuildValues(defs, null);
            Assert.AreEqual(3, Convert.ToInt32(AiArgParser.ConvertToType(values["scale"], typeof(int), "scale")));

            //覆盖值优先
            var overrides = new Dictionary<string, object> { ["scale"] = 10 };
            values = AiArgParser.BuildValues(defs, overrides);
            Assert.AreEqual(10, Convert.ToInt32(AiArgParser.ConvertToType(values["scale"], typeof(int), "scale")));
        }

        [TestMethod]
        public void ParseValuesJsonTest()
        {
            var dict = AiArgParser.ParseValuesJson(@"{""minPrecision"":2,""name"":""abc""}");
            Assert.AreEqual(2, Convert.ToInt32(AiArgParser.ConvertToType(dict["minPrecision"], typeof(int), "minPrecision")));
            Assert.AreEqual("abc", dict["name"]);
            Assert.AreEqual(0, AiArgParser.ParseValuesJson(null).Count);
            Assert.AreEqual(0, AiArgParser.ParseValuesJson("bad").Count);
        }

        [TestMethod]
        public void ConvertToTypeTest()
        {
            Assert.AreEqual(5, AiArgParser.ConvertToType("5", typeof(int), "a"));
            Assert.AreEqual(5, AiArgParser.ConvertToType(5.0, typeof(int), "a"));
            Assert.AreEqual(5.5, AiArgParser.ConvertToType("5.5", typeof(double), "a"));
            Assert.AreEqual(true, AiArgParser.ConvertToType("true", typeof(bool), "a"));
            Assert.AreEqual((int?)5, AiArgParser.ConvertToType("5", typeof(int?), "a"));
            var guid = Guid.NewGuid();
            Assert.AreEqual(guid, AiArgParser.ConvertToType(guid.ToString(), typeof(Guid), "a"));
            var now = DateTime.Now;
            Assert.AreEqual(now.Date, ((DateTime)AiArgParser.ConvertToType(now.ToString("yyyy-MM-dd HH:mm:ss"), typeof(DateTime), "a")).Date);

            //枚举
            Assert.AreEqual(DBTypeEnum.MySql, AiArgParser.ConvertToType("MySql", typeof(DBTypeEnum), "a"));
            Assert.AreEqual(DBTypeEnum.MySql, AiArgParser.ConvertToType(1, typeof(DBTypeEnum), "a"));

            //类型兼容直接返回
            var list = new List<int> { 1 };
            Assert.AreSame(list, AiArgParser.ConvertToType(list, typeof(IEnumerable<int>), "a"));

            //转换失败抛带参数名的异常
            var ex = Assert.ThrowsException<InvalidOperationException>(() => AiArgParser.ConvertToType("abc", typeof(int), "myarg"));
            StringAssert.Contains(ex.Message, "myarg");
        }

        #endregion

        #region AiCodeCompiler

        [TestMethod]
        public void CompileSuccessTest()
        {
            var result = AiCodeCompiler.Compile(CalcCode, "Calc", typeof(TestAiModel));
            Assert.IsTrue(result.Success, string.Join("\n", result.Diagnostics));
            Assert.IsNotNull(result.Method);
            Assert.AreEqual("Calc", result.Method.Name);
        }

        [TestMethod]
        public void CompileWrongMethodNameFailsTest()
        {
            var result = AiCodeCompiler.Compile(CalcCode, "WrongName", typeof(TestAiModel));
            Assert.IsFalse(result.Success);
            StringAssert.Contains(result.Diagnostics.First(), "WrongName");
        }

        [TestMethod]
        public void CompileSyntaxErrorFailsTest()
        {
            var result = AiCodeCompiler.Compile("public static double Calc( {", "Calc", typeof(TestAiModel));
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Diagnostics.Count > 0);
        }

        [TestMethod]
        public void BlacklistBlocksFileTest()
        {
            var code = @"public static bool X() { System.IO.File.Delete(""a.txt""); return true; }";
            var result = AiCodeCompiler.Compile(code, "X", typeof(TestAiModel));
            Assert.IsFalse(result.Success);
            Assert.AreEqual("File", result.BlacklistHit);
        }

        [TestMethod]
        public void BlacklistBlocksHttpAndProcessTest()
        {
            var http = AiCodeCompiler.Compile(@"public static bool X() { var c = new HttpClient(); return true; }", "X", typeof(TestAiModel));
            Assert.IsFalse(http.Success);
            Assert.AreEqual("HttpClient", http.BlacklistHit);

            var proc = AiCodeCompiler.Compile(@"public static bool X() { Process.Start(""cmd""); return true; }", "X", typeof(TestAiModel));
            Assert.IsFalse(proc.Success);
            Assert.AreEqual("Process", proc.BlacklistHit);

            var refl = AiCodeCompiler.Compile(@"public static bool X() { var m = typeof(string).GetMethod(""Clone""); return true; }", "X", typeof(TestAiModel));
            Assert.IsFalse(refl.Success);
            Assert.AreEqual("GetMethod", refl.BlacklistHit);
        }

        [TestMethod]
        public void BlacklistNotBlockNormalCodeTest()
        {
            //正常LINQ/字符串操作不应误伤
            var code = @"
public static double Calc(double input, int scale, AiContext ctx)
{
    var list = new List<double> { input, 1.5, 2.5 };
    var max = list.Where(x => x > 1).Max();
    ctx.WriteLog($""max={max}"");
    return max * scale;
}";
            var result = AiCodeCompiler.Compile(code, "Calc", typeof(TestAiModel));
            Assert.IsTrue(result.Success, result.BlacklistHit ?? string.Join("\n", result.Diagnostics));
        }

        /// <summary>
        /// 渗透测试发现的绕过路径必须全部拦截：
        /// Type.GetType(字符串)+GetMethods()+Invoke 组合曾是零密码RCE的入口
        /// </summary>
        [TestMethod]
        public void BlacklistBlocksReflectionBypassTest()
        {
            //曾实际用于RCE的payload：字符串反射还原File后Invoke写文件
            var rce = AiCodeCompiler.Compile(@"
public static double Calc(double input)
{
    var t = Type.GetType(""System.IO.File, System.Runtime"");
    var m = t.GetMethods().First(x => x.Name == ""WriteAllText"" && x.GetParameters().Length == 2);
    m.Invoke(null, new object[] { @""C:\temp\pwn.txt"", ""x"" });
    return input * 2;
}", "Calc", typeof(TestAiModel));
            Assert.IsFalse(rce.Success);
            Assert.IsNotNull(rce.BlacklistHit);

            //Type标识符出现在任何位置都拦（var Type t = ...等变体）
            var typeHit = AiCodeCompiler.Compile(@"public static bool X() { Type t = null; return true; }", "X", typeof(TestAiModel));
            Assert.IsFalse(typeHit.Success);
            Assert.AreEqual("Type", typeHit.BlacklistHit);

            //obj.GetType() 反射入口
            var gettype = AiCodeCompiler.Compile(@"public static bool X(object o) { var t = o.GetType(); return true; }", "X", typeof(TestAiModel));
            Assert.IsFalse(gettype.Success);

            //Assembly.Load(byte[])
            var load = AiCodeCompiler.Compile(@"public static bool X() { System.Reflection.Assembly.Load(new byte[0]); return true; }", "X", typeof(TestAiModel));
            Assert.IsFalse(load.Success);
            Assert.AreEqual("Assembly", load.BlacklistHit);
        }

        /// <summary>
        /// P/Invoke与unsafe代码入口必须拦截：DllImport可声明任意原生函数
        /// </summary>
        [TestMethod]
        public void BlacklistBlocksPinvokeAndUnsafeTest()
        {
            var dllimport = AiCodeCompiler.Compile(@"
[DllImport(""kernel32"")] static extern bool Beep(int f, int d);
public static bool X() { return Beep(1, 1); }", "X", typeof(TestAiModel));
            Assert.IsFalse(dllimport.Success);
            Assert.AreEqual("DllImport", dllimport.BlacklistHit);

            var unsafeBlock = AiCodeCompiler.Compile(@"public static bool X() { unsafe { } return true; }", "X", typeof(TestAiModel));
            Assert.IsFalse(unsafeBlock.Success);

            var pointer = AiCodeCompiler.Compile(@"public static bool X() { return true; } static int* p; ", "X", typeof(TestAiModel));
            Assert.IsFalse(pointer.Success);
        }

        /// <summary>
        /// 表达式树编译绕过必须拦截：Expression.Call+Compile可构造任意调用
        /// </summary>
        [TestMethod]
        public void BlacklistBlocksExpressionTreeTest()
        {
            var expr = AiCodeCompiler.Compile(@"
public static bool X()
{
    var call = System.Linq.Expressions.Expression.Call(null, null);
    var f = System.Linq.Expressions.Expression.Lambda(call).Compile();
    return f != null;
}", "X", typeof(TestAiModel));
            Assert.IsFalse(expr.Success);
            Assert.IsNotNull(expr.BlacklistHit);
        }

        /// <summary>
        /// 保存前校验（DoAdd/DoEdit内部调用）：恶意代码返回false，正常代码返回true
        /// </summary>
        [TestMethod]
        public void ValidateAiCodeFieldsTest()
        {
            var good = new TestAiModel { Name = "n", Code = @"public static double Calc(double input) { return input * 2; }" };
            Assert.IsTrue(AiCodeCompiler.ValidateAiCodeFields(good, null));

            var bad = new TestAiModel { Name = "n", Code = @"public static double Calc(double input) { Type.GetType(""x""); return input; }" };
            Assert.IsFalse(AiCodeCompiler.ValidateAiCodeFields(bad, null));

            //空代码不校验（尚未生成的场景）
            var empty = new TestAiModel { Name = "n" };
            Assert.IsTrue(AiCodeCompiler.ValidateAiCodeFields(empty, null));
        }

        [TestMethod]
        public void CompileCacheTest()
        {
            var r1 = AiCodeCompiler.Compile(CalcCode, "Calc", typeof(TestAiModel));
            var r2 = AiCodeCompiler.Compile(CalcCode, "Calc", typeof(TestAiModel));
            Assert.AreSame(r1, r2);
            Assert.AreSame(r1.Method, r2.Method);

            //代码变化不命中缓存
            var other = AiCodeCompiler.Compile(CalcCode.Replace("* scale", "* scale + 1"), "Calc", typeof(TestAiModel));
            Assert.AreNotSame(r1.Method, other.Method);
        }

        #endregion

        #region AiCodeRunner

        [TestMethod]
        public void InvokeCodeBindArgsTest()
        {
            //args默认值 scale=3
            var logs = new List<string>();
            var rv = AiCodeRunner.InvokeCode<double>(null, CalcCode, "Calc", typeof(TestAiModel),
                new Dictionary<string, object> { ["input"] = 2.5 },
                CalcArgs, null, log => logs.Add(log));
            Assert.AreEqual(7.5, rv);
            Assert.IsTrue(logs.Any(l => l.Contains("输入正常")));

            //覆盖 scale=4，且字符串值应被转换
            var rv2 = AiCodeRunner.InvokeCode<double>(null, CalcCode, "Calc", typeof(TestAiModel),
                new Dictionary<string, object> { ["input"] = 2.5 },
                CalcArgs, new Dictionary<string, object> { ["scale"] = "4" });
            Assert.AreEqual(10.0, rv2);
        }

        [TestMethod]
        public void InvokeCodeCtxLogBranchTest()
        {
            var logs = new List<string>();
            var rv = AiCodeRunner.InvokeCode<double>(null, CalcCode, "Calc", typeof(TestAiModel),
                new Dictionary<string, object> { ["input"] = -1 },
                CalcArgs, null, log => logs.Add(log));
            Assert.AreEqual(0, rv);
            Assert.IsTrue(logs.Any(l => l.Contains("输入为负数")));
        }

        [TestMethod]
        public void InvokeCodeMissingParamFailsTest()
        {
            //不传input也不在args中
            var ex = Assert.ThrowsException<InvalidOperationException>(() =>
                AiCodeRunner.InvokeCode<double>(null, CalcCode, "Calc", typeof(TestAiModel),
                    null, CalcArgs));
            StringAssert.Contains(ex.Message, "input");
        }

        [TestMethod]
        public void InvokeCodeBlacklistFailsTest()
        {
            var ex = Assert.ThrowsException<InvalidOperationException>(() =>
                AiCodeRunner.InvokeCode<bool>(null, @"public static bool X() { File.Delete(""a""); return true; }", "X", typeof(TestAiModel)));
            StringAssert.Contains(ex.Message, "黑名单");
        }

        [TestMethod]
        public void InvokeCodeTimeoutTest()
        {
            //自旋约10秒，把运行器超时改短验证超时抛出
            var old = AiCodeRunner.TimeoutMs;
            AiCodeRunner.TimeoutMs = 500;
            try
            {
                var code = @"
public static int Spin(int input)
{
    var end = System.DateTime.Now.AddSeconds(6);
    while (System.DateTime.Now < end) { }
    return input;
}";
                var ex = Assert.ThrowsException<TimeoutException>(() =>
                    AiCodeRunner.InvokeCode<int>(null, code, "Spin", typeof(TestAiModel),
                        new Dictionary<string, object> { ["input"] = 1 }));
                StringAssert.Contains(ex.Message, "超时");
            }
            finally
            {
                AiCodeRunner.TimeoutMs = old;
            }
        }

        [TestMethod]
        public void InvokeByEntityTest()
        {
            var entity = new TestAiModel { Code = CalcCode, Args = CalcArgs };
            var rv = AiCodeRunner.Invoke<double>(null, entity, "Calc",
                new Dictionary<string, object> { ["input"] = 3 });
            Assert.AreEqual(9.0, rv);
        }

        [TestMethod]
        public void InvokeByVmTest()
        {
            var vm = new TestAiVM { Entity = new TestAiModel { Code = CalcCode, Args = CalcArgs } };
            //占位函数的标准调用方式
            var rv = vm.Calc(2.0);
            Assert.AreEqual(6.0, rv);
        }

        [TestMethod]
        public void GetCodeAndArgsTest()
        {
            var entity = new TestAiModel { Code = CalcCode, Args = CalcArgs };
            var (code, args) = AiCodeRunner.GetCodeAndArgs(entity, "Calc");
            Assert.AreEqual(CalcCode, code);
            Assert.AreEqual(CalcArgs, args);
        }

        #endregion

        #region XML注释与提示词自动组装

        private static MethodInfo GetFunc(string name)
        {
            return typeof(TestAiVM).GetMethods()
                .First(m => m.GetCustomAttribute<AiFuncAttribute>()?.Name == name);
        }

        [TestMethod]
        public void XmlDocDiagTest()
        {
            var asm = typeof(TestAiVM).Assembly;
            var loc = asm.Location;
            var xmlPath = System.IO.Path.ChangeExtension(loc, ".xml");
            Assert.IsTrue(System.IO.File.Exists(xmlPath),
                $"XML不存在 Location={loc} XmlPath={xmlPath}");
            var doc = XmlDocReader.GetMethodDoc(GetFunc("Check"));
            Assert.IsNotNull(doc, $"XML存在但未读到注释 DocId={XmlDocReader.MethodDocId(GetFunc("Check"))}");
        }

        [TestMethod]
        public void XmlDocReaderReadsMethodDocTest()
        {
            //测试工程开启了GenerateDocumentationFile，bin下有同名XML
            var doc = XmlDocReader.GetMethodDoc(GetFunc("Check"));
            Assert.IsNotNull(doc, "XML文档注释未生成（测试工程需开启GenerateDocumentationFile）");
            StringAssert.Contains(doc.Summary, "校验输入值");
            Assert.IsTrue(doc.Params.TryGetValue("input", out var pdoc));
            StringAssert.Contains(pdoc, "允许为负数");
            StringAssert.Contains(doc.Returns, "告警");
        }

        [TestMethod]
        public void XmlDocReaderReadsTypeAndPropertyTest()
        {
            Assert.IsNotNull(XmlDocReader.GetTypeSummary(typeof(TestAiResult)));
            StringAssert.Contains(XmlDocReader.GetTypeSummary(typeof(TestAiResult)), "返回值DTO");
            var prop = typeof(TestAiResult).GetProperty(nameof(TestAiResult.NeedReview));
            StringAssert.Contains(XmlDocReader.GetPropertySummary(prop), "人工复核");
        }

        [TestMethod]
        public void TypeNameTupleTest()
        {
            var rt = GetFunc("Check").ReturnType;
            var names = GetFunc("Check").ReturnParameter
                .GetCustomAttribute<System.Runtime.CompilerServices.TupleElementNamesAttribute>()?.TransformNames;
            var display = AiCodePromptBuilder.TypeName(rt, names);
            StringAssert.Contains(display, "value");
            StringAssert.Contains(display, "alert");
            StringAssert.Contains(display, "double");
            StringAssert.Contains(display, "bool");
            //无元素名时不崩
            var noNames = AiCodePromptBuilder.TypeName(rt);
            StringAssert.Contains(noNames, "(");
        }

        [TestMethod]
        public void GuessDescriptionTest()
        {
            StringAssert.Contains(DescriptionGuesser.Guess("isNeedAlert", typeof(bool)), "是否");
            StringAssert.Contains(DescriptionGuesser.Guess("maxRetryCount", typeof(int)), "最大");
            StringAssert.Contains(DescriptionGuesser.Guess("createTime", typeof(DateTime?)), "时间");
            StringAssert.Contains(DescriptionGuesser.Guess("userName", typeof(string)), "名称");
            StringAssert.Contains(DescriptionGuesser.Guess("minValue", typeof(double)), "最小");
            StringAssert.Contains(DescriptionGuesser.Guess("orderCount", typeof(int)), "数量");
            //无规则命中时返回拆词
            StringAssert.Contains(DescriptionGuesser.Guess("pumpFrequency", typeof(double)), "pump");
        }

        [TestMethod]
        public void BuildSystemPromptMergesAllSourcesTest()
        {
            var method = GetFunc("Check");
            var attr = method.GetCustomAttribute<AiFuncAttribute>();
            var prompt = AiCodePromptBuilder.BuildSystemPrompt(attr, method, typeof(TestAiModel));

            //XML summary进入功能描述
            StringAssert.Contains(prompt, "校验输入值");
            //XML param进入固定参数
            StringAssert.Contains(prompt, "允许为负数");
            //XML returns进入返回值说明
            StringAssert.Contains(prompt, "value为计算后的数值");
            //元组元素展开（元素名来自TupleElementNames）
            StringAssert.Contains(prompt, "value : double");
            StringAssert.Contains(prompt, "alert : bool");
            //模型属性的XML注释进入属性清单
            StringAssert.Contains(prompt, "输入的原始数值");
            //元组类型显示
            StringAssert.Contains(prompt, "(double value, bool alert)");
        }

        [TestMethod]
        public void BuildSystemPromptExpandsDtoTest()
        {
            var method = GetFunc("Process");
            var attr = method.GetCustomAttribute<AiFuncAttribute>();
            var prompt = AiCodePromptBuilder.BuildSystemPrompt(attr, method, typeof(TestAiModel));
            //返回值DTO属性展开，字段注释来自XML
            StringAssert.Contains(prompt, "Value : double");
            StringAssert.Contains(prompt, "结果数值");
            StringAssert.Contains(prompt, "NeedReview : bool");
            StringAssert.Contains(prompt, "人工复核");
        }

        [TestMethod]
        public void BuildSystemPromptFallsBackToGuessTest()
        {
            var method = GetFunc("Guess");
            var attr = method.GetCustomAttribute<AiFuncAttribute>();
            var prompt = AiCodePromptBuilder.BuildSystemPrompt(attr, method, typeof(TestAiModel));
            //XML注释优先（Guess的param有XML注释）
            StringAssert.Contains(prompt, "最大重试次数");
            //无XML无特性的模型字段按名称推断并标注
            StringAssert.Contains(prompt, "（按名称推断）");
        }

        [TestMethod]
        public void TupleReturnEndToEndTest()
        {
            //元组返回的脚本经编译与Runner跑通
            var code = @"
public static (double value, bool alert) Check(double input)
{
    if (input > 100)
    {
        return (input, true);
    }
    else
    {
        return (input * 2, false);
    }
}";
            var entity = new TestAiModel { CheckCode = code, CheckArgs = "[]" };
            var rv = AiCodeRunner.Invoke<(double, bool)>(null, entity, "Check",
                new Dictionary<string, object> { ["input"] = 150 });
            Assert.AreEqual(150, rv.Item1);
            Assert.AreEqual(true, rv.Item2);

            var rv2 = AiCodeRunner.Invoke<(double, bool)>(null, entity, "Check",
                new Dictionary<string, object> { ["input"] = 10 });
            Assert.AreEqual(20, rv2.Item1);
            Assert.AreEqual(false, rv2.Item2);
        }

        [TestMethod]
        public void DtoReturnEndToEndTest()
        {
            //自定义类返回的脚本经编译与Runner跑通（DTO与测试同程序集，脚本可new）
            var code = @"
public static TestAiResult Process(double input)
{
    var rv = new TestAiResult();
    rv.Value = input * 3;
    rv.Log = ""processed "" + input;
    rv.NeedReview = input < 0;
    return rv;
}";
            var entity = new TestAiModel { ProcessCode = code, ProcessArgs = "[]" };
            var rv = AiCodeRunner.Invoke<TestAiResult>(null, entity, "Process",
                new Dictionary<string, object> { ["input"] = 5 });
            Assert.AreEqual(15, rv.Value);
            Assert.AreEqual(false, rv.NeedReview);
            StringAssert.Contains(rv.Log, "processed 5");
        }

        #endregion
    }
}

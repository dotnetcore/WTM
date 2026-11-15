using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WalkingTec.Mvvm.Core.Support.AiCode;

namespace WalkingTec.Mvvm.Core.Test
{
    // 模拟AiDemo的VM：命名空间与真实AiDemoVM一致，用于触发编译器的using注入
    namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.AiDemoVms
    {
        public class DummyModel { }
    }

    /// <summary>
    /// 复现AI生成的多分支代码编译失败问题
    /// </summary>
    [TestClass]
    public class AiCodeGenerateCompileTest
    {
        private const string GeneratedCode = @"public static double ProcessValue(double input, double multiplier = 3, double offset = 10, double upperLimit = 1000, int decimalPlaces = 2)
{
    // 第一步：业务上允许输入负数，负数统一按 0 处理
    double safeInput;
    if (input < 0)
    {
        safeInput = 0;
    }
    else
    {
        safeInput = input;
    }

    // 第二步：乘以倍数后加上偏移量（默认乘 3 倍后加 10）
    double result = safeInput * multiplier + offset;

    // 第三步：与上限比较，超过上限则截断为上限值
    if (result > upperLimit)
    {
        result = upperLimit;
    }
    else
    {
        // 未超过上限，保持计算后的原值
    }

    // 第四步：小数位数合法性校验，限定在 0 ~ 15 之间
    int precision;
    if (decimalPlaces < 0)
    {
        precision = 0;
    }
    else if (decimalPlaces > 15)
    {
        precision = 15;
    }
    else
    {
        precision = decimalPlaces;
    }

    // 第五步：按指定小数位四舍五入（远离零方式，避免银行家舍入的歧义）
    double rounded = Math.Round(result, precision, MidpointRounding.AwayFromZero);

    // 返回最终结果
    return rounded;
}";

        [TestMethod]
        public void GeneratedMultiBranchCode_ShouldCompile()
        {
            // 与AiDemo的validate端点一致：携带模型类型（编译器会注入其命名空间的using）
            var modelType = typeof(WalkingTec.Mvvm.Mvc.Admin.ViewModels.AiDemoVms.DummyModel);
            var compile = AiCodeCompiler.Compile(GeneratedCode, "ProcessValue", modelType);
            Assert.IsTrue(compile.Success, "编译失败：" + string.Join("；", compile.Diagnostics));
        }
    }
}

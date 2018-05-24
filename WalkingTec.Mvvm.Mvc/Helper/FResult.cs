using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WalkingTec.Mvvm.Mvc
{
    public class FResult : ContentResult
    {
        public StringBuilder ContentBuilder { get; set; }
        public BaseController Controller { get; set; }

        public FResult() : this(string.Empty) { }

        public FResult(string content)
        {
            ContentBuilder = new StringBuilder(content);
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            Content = ContentBuilder.ToString();
            return base.ExecuteResultAsync(context);
        }
    }
}

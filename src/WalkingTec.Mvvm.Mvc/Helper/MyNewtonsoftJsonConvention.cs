using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Linq;
using System.Reflection;
using WalkingTec.Mvvm.Mvc.Binders;

namespace WalkingTec.Mvvm.Mvc.Helper
{
    public class MyNewtonsoftJsonConvention : IControllerModelConvention
    {
        private readonly Assembly _sharedAssembly;

        public MyNewtonsoftJsonConvention(Assembly sharedAssembly)
        {
            _sharedAssembly = sharedAssembly;
        }

        public void Apply(ControllerModel controller)
        {
            if (ShouldApplyConvention(controller))
            {
                var formatterAttribute = new NewtonsoftJsonFormatterAttribute();

                // The attribute itself also implements IControllerModelConvention so we have to call that one as well.
                // This way, the NewtonsoftJsonBodyModelBinder will be properly connected to the controller actions.
                formatterAttribute.Apply(controller);

                controller.Filters.Add(formatterAttribute);
            }
        }

        private bool ShouldApplyConvention(ControllerModel controller)
        {
            return controller.ControllerType.FullName.StartsWith("Elsa.Server.Api") &&
                !controller.Attributes.Any(x => x.GetType() == typeof(NewtonsoftJsonFormatterAttribute));
        }
    }
}

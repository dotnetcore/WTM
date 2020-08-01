using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WalkingTec.Mvvm.Demo.Filters {
    public class SwaggerDocumentFilter: IDocumentFilter {
		public void Apply (OpenApiDocument swaggerDoc, DocumentFilterContext context) {
			var _removes = (from p in swaggerDoc.Paths where p.Key.StartsWith ("/api/_") select p.Key).ToList ();
			foreach (var _remove in _removes)
				swaggerDoc.Paths.Remove (_remove);
			//
			context.SchemaRepository.Schemas.Clear ();
		}
	}
}

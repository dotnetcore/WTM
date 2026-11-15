# WalkingTec.Mvvm (WTM)

.NET rapid-development MVVM framework (v10.0.0-alpha) targeting `net10.0`. Supports LayUI, React, VUE, VUE3, and Blazor UIs. NuGet packages: `WalkingTec.Mvvm.Core`, `WalkingTec.Mvvm.Mvc`, `WalkingTec.Mvvm.TagHelpers.LayUI`.

## Project

- **Solution**: `WalkingTec.Mvvm.sln`
- **Stack**: .NET 10.0 / ASP.NET Core / EF Core / MSTest + Moq
- **Entry points**: library, not an executable. Demos in `demo/` (ConsoleDemo, Vue3Demo, BlazorDemo, ReactDemo, VueDemo).
- **Version**: `version.props` → 10.0.0 (debug suffix: alpha); shared build props also in `common.props`
- **Test projects**: `test/WalkingTec.Mvvm.Core.Test`, `test/WalkingTec.Mvvm.Mvc.Test`, `test/WalkingTec.Mvvm.Admin.Test`, `test/WalkingTec.Mvvm.Test.Mock` (shared mocks/fixtures)
- **Not a git repo by default tooling**: verify with `git status` before assuming branch/commit workflows

## Commands

```powershell
# Build entire solution
dotnet build WalkingTec.Mvvm.sln

# Run all tests
dotnet test WalkingTec.Mvvm.sln

# Run Core tests only
dotnet test test/WalkingTec.Mvvm.Core.Test/WalkingTec.Mvvm.Core.Test.csproj

# Build a single project
dotnet build src/WalkingTec.Mvvm.Core/WalkingTec.Mvvm.Core.csproj
```

## Architecture

```
src/
├── WalkingTec.Mvvm.Core/         ← Core framework (VM base classes, Models, DataContext, Extensions)
│   ├── Models/                   ← BasePoco, TopBasePoco, PersistPoco, TreePoco + built-in entities
│   ├── BaseCRUDVM.cs             ← Single-table CRUD (DoAdd/DoEdit/DoDelete/DoRealDelete)
│   ├── BasePagedListVM.cs        ← Paged-list with search & export
│   ├── BaseImportVM.cs           ← Excel import via template
│   ├── BaseBatchVM.cs            ← Batch operations (delete, edit group)
│   ├── BaseVM.cs                 ← Root VM base (Wtm context, DC, Session, InitVM/ReInitVM)
│   ├── DataContext.cs            ← EF Core DbContext base (FrameworkContext)
│   └── WTMContext.cs             ← Per-request context bag
├── WalkingTec.Mvvm.Mvc/          ← MVC/API layer
│   ├── BaseController.cs         ← MVC controller base
│   ├── BaseApiController.cs      ← REST API controller base
│   ├── CodeGenVM.cs              ← Code generator (scaffolds Models/VMs/Controllers/Views)
│   ├── _FrameworkController.cs   ← Built-in admin pages (user, role, menu, log…)
│   └── GeneratorFiles/           ← T4-like templates for code-gen output
└── WalkingTec.Mvvm.TagHelpers.LayUI/  ← LayUI Tag Helpers (Form, Grid, Panel, Dialog, Tree…)

Other `src/` items: `WalkingTec.Mvvm.Demo` (shared demo models/VMs used by demos and tests), `WTMAI` (AI-assist helpers).
```

**Model hierarchy**: `TopBasePoco` (Guid ID) → `BasePoco` (+CreateTime/CreateBy/UpdateTime/UpdateBy) → `PersistPoco` (+IsValid, soft-delete) / `TreePoco` (+ParentId, hierarchy).

**VM hierarchy**: `BaseVM` → `BaseCRUDVM<T>` / `BasePagedListVM<T, S>` / `BaseImportVM<T, M>` / `BaseBatchVM<M, E>`.

## Conventions

- **Formatting**: spaces (4 for .cs, 2 for .json/.cshtml/.xml/props), UTF-8, LF-final, trailing whitespace trimmed. `dotnet_sort_system_directives_first = true`. See `.editorconfig`.
- **Naming**: PascalCase for public, `_camelCase` for private fields. VM classes: `{Entity}VM`, `{Entity}ListVM`, `{Entity}Searcher`, `{Entity}BatchVM`, `{Entity}ImportVM`.
- **Models**: inherit `TopBasePoco` / `BasePoco`. Use `[Display(Name="…")]`, `[Required]`, `[StringLength]` DataAnnotations. Navigation properties with `[NotMapped]` where needed.
- **Tests**: MSTest (`[TestClass]`, `[TestMethod]`, `[DataTestMethod]`/`[DataRow]`). Use `MockWtmContext.CreateWtmContext` + in-memory EF Core (`DBTypeEnum.Memory`). Assert via fresh `DataContext` instances. Tests in `WalkingTec.Mvvm.Core.Test` namespace.
- **Doc comments**: XML doc comments in Chinese (中文) on public APIs. Interface methods paired with XML `<summary>`.
- **Async**: sync/async pairs provided (e.g., `DoAdd()` + `DoAddAsync()`).
- **WTM context**: VM gets `Wtm` (WTMContext) injected via Controller; it exposes DC, Session, Cache, LoginUserInfo, ConfigInfo, UIService. Call `CopyContext(BaseVM)` to propagate across sub-VMs.

## Notes

- Full coding guide memory: `wtm-coding-guide` (code-generation patterns for Models/VMs/Controllers/front-end).
- WTM docs: http://wtmdoc.walkingtec.cn

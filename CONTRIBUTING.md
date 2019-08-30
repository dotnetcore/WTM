# Contributing to WTM

We would love for you to contribute to WTM and help make it even better than it is
today! As a contributor, here are the guidelines we would like you to follow:

- [Git Branch Management Rules](#coc)
- [Git Commit Message Rules](#coc1)

## <a name="coc"></a> [Git Branch Management Rules](https://alienwow.cc/zh-cn/2017/06/understand-a-successful-git-branching-model/index.html)

### 分支类型

1. 主分支：
    1. master: 发布分支，随时可以将分支上的代码部署到生产环境上；
    1. develop: 开发分支，所有最新的功能都将在该分支下进行开发，develop 也将是所有分支中功能最全，代码最新的一个分支；
1. 临时分支：
    1. feature/* ：功能分支，命名规则 `feature/功能名称`，作为新功能的开发分支，该分支从 develop 创建，开发完毕之后需要重新合并到 develop；
    1. release/* ：预发布分支，命名规则 `release/v+发布的版本号`，作为预发布分支，release/* 只能从 develop 创建，且在 git flow 中同一个时间点，只能存在一个预发布分支。只有当上一个版本发布成功之后删除该分支，之后才能进行下一个版本的发布。如果在预发布过程中发现了问题，只能在 release/* 分支上进行修改；
    1. bugfix/*  ：Bug 修复分支，命名规则 `bugfix/v+bug修复的版本号`，作为 Bug 修复分支，只能从 master 分支分离出来。主要是用来修复在生产环境上发现的 bug，修复完成并测试通过后需要将该分支合并回 develop 及 master 上，并删除该分支；

## <a name="coc1"></a>[Git Commit Message Rules](http://alienwow.cc/zh-cn/2017/06/git-commit-standard/index.html)

每次提交，Commit message 都包括三个部分：Header，Body 和 Footer。

```html
<type>(<scope>): <subject>
<BLANK LINE>
<body>
<BLANK LINE>
<footer>
```

其中，Header 是必需的，Body 和 Footer 可以省略。

为了避免自动换行，任何一行都不得超过70个字符。

### Header

Header 部分只有一行，包括三个字段：type（必需）、scope（可选）和subject（必需）。

#### type（必需）

`type` 用于描述 `commit` 的类型，只允许如下 10 种类型

- feature ( 功能的增删改 )
- fix ( 修复 bug [PS: 修复 bug 的描述中包含 Issue 的 Id,如：Issue #269] )
- refactor ( 重构，即不是新增功能，也不是修改 bug 的代码变动 )
- ci ( 针对 CI 配置文件和脚本的更改 )
- style ( 不影响代码运行的变动，如格式，修改 using 排序 )
- perf ( performance 缩写，提升性能、优化 )
- lib ( 升级依赖项，如 .net 平台的 nuget 包，node 的 node_modules 等 )
- docs ( 修改文档，如README、CHANGELOG等)
- test ( 增加测试 )
- release ( 发布新版本 )

`type` 为 `feature` 及 `fix` 的 `commit` 均应该添加到 `CHANGELOG.md` 中，其他 `commit` 不建议添加。

#### scope（可选）

`scope` 用于说明 `commit` 影响的范围，如 LayUI、React、Vue、Admin、View、Controller、Model 等，视项目不同而不同。

#### subject（必需）

`subject` 是 `commit` 目的的简短描述。

1. 需保证 Header 整行不超过 70 个字符；
1. 以动词开头，使用第一人称现在时，比如 `change`，而不是 `changed` 或 `changes`；
1. 标题首字母小写；
1. 标题不要使用句号结尾。

### Body

Body 部分是对本次 commit 的详细描述，可以分成多行。下面是一个范例。

```txt
More detailed explanatory text, if necessary.  Wrap it to
about 72 characters or so.

Further paragraphs come after blank lines.

- Bullet points are okay, too
- Use a hanging indent
```

1. 需保证 Body 的每一行都不超过 70 个字符；
1. 使用第一人称现在时，比如 `change`，而不是 `changed` 或 `changes`；
1. 需要解释代码更改的动机，即说明 why & what，而不是 how；
1. 删除 Changes to be committed: 及其下面的 #。

### Footer

Footer 部分只用于两种情况。

#### 不兼容变动

如果当前代码与上一个版本不兼容，则 Footer 部分以BREAKING CHANGE开头，后面是对变动的描述、以及变动理由和迁移方法。

#### 关闭 Issue

如果当前 `commit` 针对某个 `issue` ，那么可以在 Footer 部分关闭这个 `issue`。

```txt
Closes #123, #234, #235
```

### Revert

这是一种特殊情况，如果当前 `commit` 是为了撤销之前的 `commit`，则必须以 `revert:` 开头，后面紧跟着被撤销的 `commit` 的 `Header`。

```txt
revert: feat(pencil): add 'graphiteWidth' option

This reverts commit 667ecc1654a317a13331b17617d973392f415f02.
```

Body 部分的格式是固定的，必须写成 `This reverts commit <hash>.`，其中的 `hash` 是被撤销 `commit` 的 SHA 标识符。

如果当前 `commit` 与被撤销的 `commit`，在同一个发布 `release` 里面，那么它们都不会出现在 `CHANGELOG` 里面。如果两者在不同的发布，那么当前 `commit`，会出现在 `CHANGELOG` 的 `Reverts` 小标题下面。

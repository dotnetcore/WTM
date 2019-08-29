# 版本号说明

## 正式版本号

规则：MAJOR.MINOR.PATCH

例：WalkingTec.Mvvm.Core.2.2.52

- MAJOR: 不兼容的 API 修改，或者大规模的框架重构或模块新增；
- MINOR: 向下兼容的功能性变化；
- PATCH: 向下兼容的 BUG 修复；

## 开发版本号

规则：MAJOR.MINOR.PATCH-type-days.totalSeconds

例：WalkingTec.Mvvm.Core.2.2.52-alpha-1.25021

- type: 发布类型
    - alpha: Alpha 版本，通常用于开发过程和试验。
    - beta: Beta 版本，通常指可用于下一计划版本的功能完整的版本，但可能包含已知 bug。
    - rc: 候选发布，通常可能为最终（稳定）版本，除非出现重大 bug。
- days: 上一次版本发布以来的天数
- totalSeconds: 当天已过时间的秒数

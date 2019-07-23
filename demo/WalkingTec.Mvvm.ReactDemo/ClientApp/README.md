## 注意事项

>1：tsconfig.json 为 react 脚手架生成文件，部分配置无法更改，实际编译 使用的 是 tsconfig.compile.json <br/>
>2: src/subMenu.json 只在 dev 环境中 使用的菜单 目录，正式环境请使用接口返回 数据<br/>
>3: src/setupProxy.js 代理配置文件，更改这个文件需要重新 npm start 才能生效<br/>

## 清空 生产环境中的 console 日志
> congig/webpack.config.prod.js 26行 true 为清空
> config.optimization.minimizer[0].options.terserOptions.compress.drop_console = true;
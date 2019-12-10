const { override, fixBabelImports, addBundleVisualizer, addLessLoader, disableEsLint, babelInclude, addWebpackResolve } = require('customize-cra');
const path = require('path');
module.exports = override(
    // 模块 解析 路径
    addWebpackResolve({
        modules: [
            path.resolve(path.dirname(path.dirname(process.cwd())), 'node_modules'),
            path.resolve(process.cwd(), 'node_modules'),
            path.resolve(process.cwd(), 'src'),
        ]
    }),
    // 添加 需要 编译的目录
    babelInclude([
        // 当前项目
        path.resolve(process.cwd(), 'src'),
        // public 目录
        path.resolve(path.dirname(process.cwd()), 'public', 'src')
    ]),
    // 按需加载
    fixBabelImports('import', {
        libraryName: 'antd',
        libraryDirectory: 'es',
        style: true,
    }),
    addLessLoader({
        javascriptEnabled: true,
        //    modifyVars: { '@primary-color': '#1DA57A' },
        localIdentName: "leng-[local]-[hash:base64:5]"
    }),
    // 依赖分布地图 --analyze
    addBundleVisualizer({
        "analyzerMode": "static",
        "reportFilename": "report.html"
    }, true),
    // 禁用 EsLint 
    disableEsLint()
);
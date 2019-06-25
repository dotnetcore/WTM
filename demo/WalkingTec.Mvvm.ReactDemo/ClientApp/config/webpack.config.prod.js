/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-10-30 15:16:20
 * @modify date 2018-10-30 15:16:20
 * @desc [description]
*/
const paths = require("./paths");
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const ForkTsCheckerWebpackPlugin = require('react-dev-utils/ForkTsCheckerWebpackPlugin');
const lodash = require('lodash');
const BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin;
const postcssNormalize = require('postcss-normalize');

/**
 * 重写 react-scripts 默认配置
 */
module.exports = (config, env) => {
    // config.mode = 'development';
    config.devtool = false;
    // 查看 文件 大小 分布地图
    // config.plugins.push(
    //     new BundleAnalyzerPlugin()
    // );
    // 清空 console
    config.optimization.minimizer[0].options.terserOptions.compress.drop_console = true;
    const cssloader = [
        {
            loader: 'css-loader',
            options: {
                importLoaders: 1,
            },
        },
        {
            loader: 'postcss-loader',
            options: {
                // https://github.com/facebookincubator/create-react-app/issues/2677
                ident: 'postcss',
                plugins: () => [
                    require('postcss-flexbugs-fixes'),
                    require('postcss-preset-env')({
                        autoprefixer: {
                            flexbox: 'no-2009',
                        },
                        stage: 3,
                    }),
                    // Adds PostCSS Normalize as the reset css with default options,
                    // so that it honors browserslist config in package.json
                    // which in turn let's users customize the target behavior as per their needs.
                    postcssNormalize(),
                ],
            },
        },
        {
            loader: 'less-loader',
            options: {
                sourceMap: true,
                javascriptEnabled: true,
            },
        }
    ]
    // 添加 less 编译
    lodash.update(config, 'module.rules[1].oneOf', value => {
        lodash.remove(value, data => String(data.test) === String(/\.css$/) && data.sideEffects === true || String(data.test) === String(/\.module\.css$/));
        return [
            {
                test: /\.(less|css)$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    ...cssloader
                ],
            },
            ...value,
        ]
    });
    return config
}
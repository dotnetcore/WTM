/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-10-30 15:16:20
 * @modify date 2018-10-30 15:16:20
 * @desc [description]
*/
const paths = require("./paths");
const TsconfigPathsPlugin = require('tsconfig-paths-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
/**
 * 重写 react-scripts 默认配置
 */
module.exports = (config, env) => {
    // config.entry.pop();
    // config.entry.push(paths.appIndexJs);
    // config.mode = 'development';
    config.devtool = false;
    config.resolve.extensions = ['.ts', '.tsx', '.js', '.json', '.jsx'];
    config.resolve.plugins.push(new TsconfigPathsPlugin({ configFile: paths.appTsConfig }));
    // 清空 console
    config.optimization.minimizer[0].options.terserOptions.compress.drop_console = true;
    config.module.rules = [
        {
            oneOf: [
                {
                    test: [/\.bmp$/, /\.gif$/, /\.jpe?g$/, /\.png$/],
                    loader: 'url-loader',
                    options: {
                        limit: 10000,
                        name: 'static/media/[name].[hash:8].[ext]',
                    },
                },
                {
                    test: /\.js$/,
                    include: paths.appNodeModules,
                    exclude: paths.jsExclude,
                    use: [
                        'cache-loader',
                        {
                            loader: "babel-loader",
                            options: {
                                inputSourceMap: false,
                                sourceMap: false,
                                // compact: true,
                                presets: ['@babel/preset-env']
                            }
                        }
                    ],

                },
                {
                    test: /\.(tsx|ts|js|jsx)$/,
                    include: paths.appSrc,
                    loader: 'awesome-typescript-loader',
                    options: {
                        useCache: true,
                        transpileOnly: true,
                        errorsAsWarnings: true,
                        // usePrecompiledFiles: true,
                        sourceMap: false,
                    }
                },
                {
                    test: /\.(less|css)$/,
                    use: [
                        MiniCssExtractPlugin.loader,
                        {
                            loader: 'css-loader',
                            options: {
                                importLoaders: 1,
                                sourceMap: false,
                            },
                        },
                        {
                            loader: 'postcss-loader',
                            options: {
                                // https://github.com/facebookincubator/create-react-app/issues/2677
                                ident: 'postcss',
                                sourceMap: false,
                                plugins: () => [
                                    require('postcss-flexbugs-fixes'),
                                    require('autoprefixer')({
                                        browsers: [
                                            '>1%',
                                            'last 4 versions',
                                            'Firefox ESR',
                                            'not ie < 9', // React doesn't support IE8 anyway
                                        ],
                                        flexbox: 'no-2009',
                                    }),
                                ],
                            },
                        },
                        {
                            loader: 'less-loader',
                            options: {
                                sourceMap: false,
                                javascriptEnabled: true,
                            },
                        }
                    ],
                },
                {
                    exclude: [/\.(js|jsx|mjs)$/, /\.html$/, /\.json$/],
                    loader: 'file-loader',
                    options: {
                        name: 'static/media/[name].[hash:8].[ext]',
                    },
                },
            ],
        },
    ]
    return config
}
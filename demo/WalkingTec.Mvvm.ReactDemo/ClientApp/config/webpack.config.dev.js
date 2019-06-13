/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-10-30 15:16:20
 * @modify date 2018-10-30 15:16:20
 * @desc [description]
*/
const paths = require("./paths");
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const lodash = require('lodash');
const ForkTsCheckerWebpackPlugin = require('react-dev-utils/ForkTsCheckerWebpackPlugin');
const postcssNormalize = require('postcss-normalize');

/**
 * 重写 react-scripts 默认配置
 */
module.exports = (config, env) => {
    config.resolve.extensions = ['.ts', '.tsx', '.js', '.json', '.jsx'];
    lodash.remove(config.plugins, data => data instanceof ForkTsCheckerWebpackPlugin);
    config.plugins.push(new MiniCssExtractPlugin({
        filename: 'static/css/[name].css',
        chunkFilename: 'static/css/[name].chunk.css',
    }));
    cssloader = [
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
    config.module.rules = [
        { parser: { requireEnsure: false } },
        {
            oneOf: [
                {
                    test: [/\.bmp$/, /\.gif$/, /\.jpe?g$/, /\.png$/],
                    loader: 'url-loader',
                    options: {
                        limit: 10000,
                        name: 'static/media/[name].[ext]',
                    },
                },
                // {
                //     test: /\.js$/,
                //     include: paths.appNodeModules,
                //     exclude: paths.jsExclude,
                //     use: [
                //         'cache-loader',
                //         {
                //             loader: "babel-loader",
                //             options: {
                //                 // compact: true,
                //                 cacheDirectory: true,
                //                 cacheCompression: true,
                //                 presets: ['@babel/preset-env']
                //             }
                //         }
                //     ],
                // },
                // {
                //     test: /\.(js|mjs)$/,
                //     exclude: /@babel(?:\/|\\{1,2})runtime/,
                //     use: [
                //         'cache-loader',
                //         {
                //             loader: "babel-loader",
                //             options: {
                //                 babelrc: false,
                //                 configFile: false,
                //                 compact: false,
                //                 //   presets: [
                //                 //     [
                //                 //       require.resolve('babel-preset-react-app/dependencies'),
                //                 //       { helpers: true },
                //                 //     ],
                //                 //   ],
                //                 cacheDirectory: true,
                //                 cacheCompression: true,
                //                 // If an error happens in a package, it's possible to be
                //                 // because it was compiled. Thus, we don't want the browser
                //                 // debugger to show the original code. Instead, the code
                //                 // being evaluated would be much more helpful.
                //                 sourceMaps: false,
                //             }
                //         }
                //     ],
                // },
                {
                    test: /\.(tsx|ts|js|jsx)$/,
                    include: paths.appSrc,
                    use: [
                        'cache-loader',
                        {
                            loader: 'awesome-typescript-loader',
                            options: {
                                useCache: true,
                                configFileName: "tsconfig.compile.json",
                                cacheDirectory: "node_modules/.cache/awcache",
                                // transpileOnly: true,
                                errorsAsWarnings: true,
                                // usePrecompiledFiles: true,
                            }
                        }
                    ]

                },
                {
                    test: /\.(less|css)$/,
                    include: paths.appSrc,
                    use: [
                        'style-loader',
                        ...cssloader
                    ],
                },
                {
                    test: /\.(less|css)$/,
                    include: paths.appNodeModules,
                    exclude: paths.appSrc,
                    use: [
                        MiniCssExtractPlugin.loader,
                        ...cssloader
                    ],
                },
                {
                    exclude: [/\.(js|jsx|mjs)$/, /\.html$/, /\.json$/],
                    loader: 'file-loader',
                    options: {
                        name: 'static/media/[name].[ext]',
                    },
                },
            ],
        },
    ]
    return config
}
/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-10-30 15:16:20
 * @modify date 2018-10-30 15:16:20
 * @desc [description]
*/
const paths = require("./paths");
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
/**
 * 重写 react-scripts 默认配置
 */
module.exports = (config, env) => {
    config.resolve.extensions = ['.ts', '.tsx', '.js', '.json', '.jsx'];
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
                sourceMap: true,
                javascriptEnabled: true,
            },
        }
    ]
    config.module.rules = [
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
                {
                    test: /\.js$/,
                    include: paths.appNodeModules,
                    exclude: paths.jsExclude,
                    use: [
                        'cache-loader',
                        {
                            loader: "babel-loader",
                            options: {
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
                        configFileName: "tsconfig.new.json",
                        cacheDirectory: "node_modules/.cache/awcache",
                        // transpileOnly: true,
                        errorsAsWarnings: true,
                        usePrecompiledFiles: true,
                    }
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
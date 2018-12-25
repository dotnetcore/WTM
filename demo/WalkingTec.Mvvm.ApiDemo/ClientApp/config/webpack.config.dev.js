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
    config.resolve.extensions = ['.mjs', '.web.ts', '.ts', '.web.tsx', '.tsx', '.web.js', '.js', '.json', '.web.jsx', '.jsx'];
    config.resolve.plugins.push(new TsconfigPathsPlugin({ configFile: paths.appTsConfig }));
    config.plugins.push(new MiniCssExtractPlugin({
        filename: 'static/css/[name].[contenthash:8].css',
        chunkFilename: 'static/css/[name].[contenthash:8].chunk.css',
    }));
    cssloader = [
        {
            loader: require.resolve('css-loader'),
            options: {
                importLoaders: 1,
            },
        },
        {
            loader: require.resolve('postcss-loader'),
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
            loader: require.resolve('less-loader'),
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
                    loader: require.resolve('url-loader'),
                    options: {
                        limit: 10000,
                        name: 'static/media/[name].[hash:8].[ext]',
                    },
                },
                {
                    test: /\.(tsx|ts|js|jsx)$/,
                    include: paths.appSrc,
                    use: [
                        {
                            loader: require.resolve('awesome-typescript-loader'),
                            options: {

                            },
                        },
                    ],
                },
                {
                    test: /\.(less|css)$/,
                    include: paths.appSrc,
                    use: [
                        require.resolve('style-loader'),
                        ...cssloader
                    ],
                },
                {
                    test: /\.(less|css)$/,
                    exclude: paths.appSrc,
                    use: [
                        MiniCssExtractPlugin.loader,
                        ...cssloader
                    ],
                },
                {
                    exclude: [/\.(js|jsx|mjs)$/, /\.html$/, /\.json$/],
                    loader: require.resolve('file-loader'),
                    options: {
                        name: 'static/media/[name].[hash:8].[ext]',
                    },
                },
            ],
        },
    ]
    return config
}
const webpack = require("webpack");
const path = require("path");
const ExtractTextPlugin = require("extract-text-webpack-plugin");
const HtmlWebpackPlugin = require("html-webpack-plugin");
const CleanWebpackPlugin = require("clean-webpack-plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

const env = "development"; // 'production'; // mode需要
const devMode = process.env.NODE_ENV !== "production";

function resolve(dir) {
  return path.join(__dirname, "..", dir);
}

// 雪碧图路
const spritesConfig = {
  spritePath: "./dist/static"
};

const extractTextPlugin = new ExtractTextPlugin({
  filename: "[name].min.css",
  allChunks: false
});
module.exports = {
  // 多页面应用
  entry: {
    index: "../src/index.js"
  },
  output: {
    publicPath: __dirname + "/dist/",
    path: path.resolve(__dirname, "dist"),
    filename: "[name]-[hash:5].bundle.js",
    chunkFilename: "[name]-[hash:5].chunk.js"
  },
  resolve: {
    extensions: [".js", ".vue", ".json"],
    alias: {
      "@": resolve("src")
    }
  },
  mode: env,
  devtool: "source-map",
  devServer: {
    contentBase: path.join(__dirname, "dist"),
    port: 8000,
    hot: true,
    overlay: true,
    proxy: {
      "/api": {
        target: "https://m.weibo.cn",
        changeOrigin: true,
        logLevel: "debug"
        // headers: {
        //     Cookie: ''
        // }
      }
    },
    historyApiFallback: {
      rewrites: [{ from: /.*/, to: "/index.html" }]
    }
  },

  optimization: {
    splitChunks: {
      cacheGroups: {
        // 注意: priority属性
        // 其次: 打包业务中公共代码
        common: {
          name: "common",
          chunks: "all",
          minSize: 1,
          priority: 0
        },
        // 首先: 打包node_modules中的文件
        vendor: {
          name: "vendor",
          test: /[\\/]node_modules[\\/]/,
          chunks: "all",
          priority: 10
          // enforce: true
        }
      }
    }
  },
  module: {
    rules: [
      {
        test: /\.(js|vue)$/,
        exclude: resolve("node_modules"),
        // exclude: /node_modules/,
        loader: "eslint-loader",
        options: {
          formatter: require("eslint-friendly-formatter")
        }
      },
      {
        test: /\.js$/,
        exclude: resolve("node_modules"),
        //exclude: /(node_modules)/,
        use: {
          loader: "babel-loader" // 转化需要的loader 配置在: .babelrc
        }
      },

      {
        test: /\.css$/,
        use: [MiniCssExtractPlugin.loader, "css-loader"]
      },
      // // Compile SCSS files
      // {
      //     test: /\.scss$/,
      //     // This compiles styles specific to this app
      //     include: resolve(__dirname, './src/app/styles'),
      //     use: [
      //         devMode ? 'style-loader' : MiniCssExtractPlugin.loader,
      //         {
      //             loader: 'css-loader',
      //             options: { minimize: true, sourceMap: true }
      //         },
      //         {
      //             loader: 'sass-loader',
      //             options: {
      //                 sourceMap: true,
      //                 includePaths: [resolve(__dirname, 'css/app.css')]
      //             }
      //         }
      //     ]
      // },
      {
        test: /\.(sa|sc|c)ss$/,
        use: [
          devMode ? "style-loader" : MiniCssExtractPlugin.loader,
          "css-loader",
          "postcss-loader",
          "sass-loader"
        ]
      },

      // {
      //     test: /\.css$/,
      //     use: ExtractTextPlugin.extract({
      //         fallback: {
      //             loader: 'style-loader'
      //         },
      //         use: [
      //             {
      //                 loader: 'css-loader'
      //             },
      //             // 雪碧图
      //             {
      //                 loader: 'postcss-loader',
      //                 options: {
      //                     ident: 'postcss',
      //                     plugins: [
      //                         require('postcss-sprites')(spritesConfig)
      //                     ]
      //                 }
      //             }
      //         ]
      //     })
      // },
      // {
      //     test: /\.scss$/,
      //     // 注意 loader 顺序
      //     use: [
      //         {
      //             loader: 'style-loader' // 将 JS 字符串生成为 style 节点
      //         },
      //         {
      //             loader: 'css-loader' // 将 CSS 转化成 CommonJS 模块
      //         },
      //         {
      //             loader: 'sass-loader' // 将 Sass 编译成 CSS
      //         }
      //     ]
      // },
      {
        test: /\.(png|jpg|jpeg|gif)$/,
        use: [
          {
            loader: "url-loader",
            options: {
              name: "[name]-[hash:5].min.[ext]",
              limit: 10000, // size <= 20KB
              publicPath: "static/",
              outputPath: "static/"
            }
          },
          {
            loader: "img-loader",
            options: {
              plugins: [
                require("imagemin-pngquant")({
                  quality: "80"
                })
              ]
            }
          }
        ]
      },
      {
        test: /\.(eot|woff2?|ttf|svg)$/,
        use: [
          {
            loader: "url-loader",
            options: {
              name: "[name]-[hash:5].min.[ext]",
              limit: 5000, // fonts file size <= 5KB, use 'base64'; else, output svg file
              publicPath: "fonts/",
              outputPath: "fonts/"
            }
          }
        ]
      }
    ]
  },
  plugins: [
    new webpack.HotModuleReplacementPlugin(), // 热更新
    new webpack.NamedModulesPlugin(), // 热更新
    new CleanWebpackPlugin(["dist"]), // 清理
    // extractTextPlugin, // 提取css
    new MiniCssExtractPlugin({
      // 类似 webpackOptions.output里面的配置 可以忽略
      filename: devMode ? "[name].css" : "[name].[hash].css",
      chunkFilename: devMode ? "[id].css" : "[id].[hash].css"
    }),
    new HtmlWebpackPlugin({
      filename: "index.html",
      template: "./index.html",
      chunks: ["common", "vendor", "index"], // entry中的app入口才会被打包
      minify: {
        // 压缩选项
        collapseWhitespace: true
      }
    })
  ]
};

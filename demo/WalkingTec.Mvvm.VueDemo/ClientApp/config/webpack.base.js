const HtmlWebpackPlugin = require("html-webpack-plugin");
const CopyWebpackPlugin = require("copy-webpack-plugin");
const { VueLoaderPlugin } = require("vue-loader");
const webpack = require("webpack");
const { utils } = require("./webpack-util");

// 个性参数
let options = {
  // cdn配置 第三方
  externals: {
    vue: "Vue",
    "element-ui": "ELEMENT"
  }
};
// if (process.env.NODE_ENV === "development") {
options.externals = {};
// }
module.exports = {
  entry: {
    index: utils.resolve("src/index.ts"),
    login: utils.resolve("src/login.ts")
  },
  resolve: {
    extensions: [".ts", ".tsx", ".js", ".vue", ".json"],
    alias: {
      "@": utils.resolve("src"),
      views: utils.resolve("src/views"),
      static: utils.resolve("static"),
      components: utils.resolve("src/components")
    }
  },
  externals: {
    ...options.externals
  },
  module: {
    rules: [
      // ts
      {
        test: /\.tsx?$/,
        exclude: /node_modules/,
        use: [
          {
            loader: "babel-loader"
          },
          {
            loader: "ts-loader",
            options: {
              transpileOnly: true,
              appendTsSuffixTo: ["\\.vue$"],
              happyPackMode: false
            }
          }
          // {
          //     loader: "tslint-loader"
          // }
        ]
      },
      {
        test: /\.js$/,
        exclude: /node_modules/,
        loader: "babel-loader",
        include: [utils.resolve("src")]
      },
      // vue
      {
        test: /\.vue$/,
        exclude: /node_modules/,
        use: ["vue-loader"]
      },
      // img
      {
        test: /\.(png|jpe?g|gif|svg)(\?.*)?$/,
        use: {
          loader: "url-loader",
          options: {
            limit: 10000,
            name: utils.assetsPath("img/[name].[hash:7].[ext]"),
            publicPath: "/"
          }
        }
      },
      // media
      {
        test: /\.(mp4|webm|ogg|mp3|wav|flac|aac)(\?.*)?$/,
        use: {
          loader: "url-loader",
          options: {
            limit: 10000,
            name: utils.assetsPath("media/[name].[hash:7].[ext]"),
            publicPath: "/"
          }
        }
      },
      // fonts
      {
        test: /\.(woff2?|eot|ttf|otf)(\?.*)?$/,
        use: {
          loader: "url-loader",
          options: {
            limit: 10000,
            name: utils.assetsPath("fonts/[name].[hash:7].[ext]"),
            publicPath: "/"
          }
        }
      }
    ]
  },

  optimization: {
    moduleIds: "hashed", // keep module.id stable when vender modules does not change
    splitChunks: {
      cacheGroups: {
        // 注意: priority属性
        // 其次: 打包业务中公共代码
        common: {
          name: "common",
          chunks: "all",
          minChunks: 2,
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
  plugins: [
    new HtmlWebpackPlugin({
      filename: "index.html",
      template: "view/index.html",
      chunks: ["index", "vendor", "common", "runtime"]
      // minify: {
      //     removeComments: true,
      //     collapseWhitespace: true,
      //     removeAttributeQuotes: true
      //     // more options:
      //     // https://github.com/kangax/html-minifier#options-quick-reference
      // }
    }),
    new HtmlWebpackPlugin({
      filename: "login.html",
      template: "view/index.html",
      chunks: ["login", "vendor", "common", "runtime"]
      // minify: {
      //     removeComments: true,
      //     collapseWhitespace: true,
      //     removeAttributeQuotes: true
      //     // more options:
      //     // https://github.com/kangax/html-minifier#options-quick-reference
      // }
    }),
    // VueLoaderPlugin在vue-loaderv15的版本中,这个插件是必须启用的.
    new VueLoaderPlugin(),
    new CopyWebpackPlugin([
      {
        from: utils.resolve("static/img"),
        to: utils.resolve("dist/static/img"),
        toType: "dir"
      }
    ]),
    new webpack.ProvidePlugin({
      _: "lodash"
    })
  ]
};

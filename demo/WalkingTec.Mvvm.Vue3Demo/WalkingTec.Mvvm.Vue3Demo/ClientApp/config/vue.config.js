const MomentLocalesPlugin = require('moment-locales-webpack-plugin');
    // const PrerenderSPAPlugin = require('prerender-spa-plugin')    "prerender-spa-plugin": "^3.4.0",
const CopyPlugin = require('copy-webpack-plugin');
const webpack = require('webpack');
const lodash = require('lodash');
const path = require('path');
const fs = require('fs');
const routerAuto = require('./router-auto');
module.exports = (port, env) => {
  console.log("LENG: env", env.config)
  const deployDev = env.config.DEPLOY_ENV === 'dev';
  const deployPro = env.config.DEPLOY_ENV === 'pro';
  const __dirname = process.cwd();
  process.env.VUE_APP_VERSION = `${env.config.version} ${env.config.timestamp} ${env.config.DEPLOY_ENV}`;
  process.env.VUE_APP_TIME = Date.now();
  const config = {
    outputDir: env.config.dir,
    publicPath: env.config.base,
    runtimeCompiler: true,
    filenameHashing: true,
    devServer: {
      port,
      public: '0.0.0.0',
      hot: true,
      disableHostCheck: true,
      overlay: {
        warnings: false,
        errors: true,
      },
      headers: {
        'Access-Control-Allow-Origin': '*',
      },
      proxy: {
        "/devTarget": {
          target: env.config.target, // 代理地址
          changeOrigin: true,
          pathRewrite: {
            "^/devTarget": ""
          }
        }
      }
    },
    css: {
      // modules: true,
      sourceMap: true,
      loaderOptions: {
        less: {
          lessOptions: {
            // modifyVars: modifyVars,
            javascriptEnabled: true
          }
        }
      }
    },
    pluginOptions: {
      'style-resources-loader': {
        preProcessor: 'less',
        patterns: [
          path.join(__dirname, 'src', 'assets', 'themes', 'index.less'),
          path.join(__dirname, 'src', 'assets', 'themes', 'colors.less'),
          path.join(__dirname, 'src', 'assets', 'themes', 'modifyVars.less')
        ]
      }
    },
    // 自定义webpack配置
    configureWebpack: {
      devtool: deployDev ? 'cheap-module-source-map' : false,
      resolve: {
        alias: {
          "@ant-design/icons/lib/dist$": path.join(process.cwd(), 'src/icon.ts'),
        }
      },
      plugins: [
        new envPlugins(env),
        new CopyPlugin([
          { from: require.resolve('@xt/client').replace('index.ts', 'static') },
        ]),
        // new PrerenderSPAPlugin({ staticDir: path.join(__dirname, env.config.dir), routes: routerAuto('./src/pages', true) }),
        new webpack.DefinePlugin(env.process),
        new MomentLocalesPlugin({ localesToKeep: ['es-us', 'zh-cn'] }),
        new webpack.BannerPlugin({ banner: `@author 冷 (https://github.com/LengYXin)\n@email lengyingxin8966@gmail.com` })
      ],
      output: {
        // 把子应用打包成 umd 库格式
        library: env.qiankunLibrary,
        libraryTarget: 'umd',
        jsonpFunction: `webpackJsonp_${env.qiankunLibrary}`,
      },
      optimization: {
        minimize: !deployDev,
        namedModules: deployDev,
        splitChunks: {
          chunks: 'async',
          cacheGroups: {
            min: {
              name: 'min',
              test: /[\\/]node_modules[\\/](vue.*|mobx.*|core.*|rxjs.*|nuxt.*|.*nuxt.*)[\\/]/,
              chunks: 'all',
            },
            lib: {
              name: 'lib',
              test: /[\\/]node_modules[\\/](ant-.*|@ant-.*|lodash.*|swiper.*|moment.*|viewerjs.*|bn.*|elliptic.*)[\\/]/,
              chunks: 'all',
            }
          }
        }
      },
    },
    chainWebpack: config => {
      config.module
        .rule("i18n")
        .resourceQuery(/blockType=i18n/)
        .type('javascript/auto')
        .use("i18n")
        .loader("@kazupon/vue-i18n-loader")
        .end();
    }
  }
  // 路由
  if (process.env.NODE_ENV === 'production') {
    // config.configureWebpack.plugins.push(new PrerenderSPAPlugin({ staticDir: path.join(__dirname, env.config.dir), routes: routerAuto('./src/pages', true) }))
  }
  return config

}
class envPlugins {
  constructor (env) {
    this.env = env
  }
  apply(compiler) {
    const { options } = compiler;
    compiler.plugin('done', compilation => {
      // console.log("LENG: apply -> compilation", compilation)
      try {
        const envPath = path.join(options.output.path, 'env.config.js');
        if (fs.existsSync(envPath)) {
          const envjs = fs.readFileSync(envPath).toString();
          const newEnvjs = lodash.template(envjs, { interpolate: /\({([\s\S]+?)}\)/g })({
            env: JSON.stringify(this.env.config, null, 4),
            global: JSON.stringify({}, null, 4),
          });
          fs.writeFileSync(envPath, newEnvjs);
        }
      } catch (error) {
        console.log("env failed", error);
      }
    })
  }
}
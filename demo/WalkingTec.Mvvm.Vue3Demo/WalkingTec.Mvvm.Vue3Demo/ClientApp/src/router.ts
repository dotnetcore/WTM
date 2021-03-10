import { map, includes, filter, endsWith, forEach } from 'lodash-es';
const lodash = { map, includes, filter, endsWith, forEach }
class Router {
  lazy = false;
  readonly PageFiles = require.context('./pages', true, /\.vue$/, 'sync') // 根据目录结构去搜索文件
  readonly PagePath = this.PageFiles.keys().filter(file => !lodash.includes(file, 'views'))
  get Routers() {
    const map = this.PagePath.reduce((map, cur) => {
      let dislodge = cur.match(/\/(.+?)\.vue$/)[1] // 只匹配纯文件名的字符串
      let key = dislodge.split('/')[0]; // 拿到一级文件的名称
      (map[key] || (map[key] = [])).push(cur)
      return map
    }, {})
    return this.getRoutes(map)
  }
  getRoutes(map) {
    // console.log("LENG: getRoutes -> map", map)
    const routes: Array<any> = [];
    lodash.map(map, (value, key) => {
      // 所有 页面 只取 index.tsx 结尾页面
      value = lodash.filter(value, item => lodash.endsWith(item, 'index.vue'))
      // 子页面
      const childrenPage = lodash.filter(value, rem => lodash.includes(rem, `/children/`));
      // 根页面
      const page = lodash.filter(value, rem => !lodash.includes(rem, `/children/`));
      lodash.forEach(page, item => {
        routes.push(this.getRoute(item, childrenPage))
      })
    })
    // console.log("LENG: getRoutes -> routes", routes)
    return routes
  }
  getRoute(item, childrenPage) {
    const name = this.getRouteItemName(item);
    const path = this.getRouteItemPath(item);
    // const children = lodash.get(component, 'children', lodash.get(component, 'options.children', lodash.filter(childrenPage, x => lodash.includes(x, `${path}/children`)).map(x => getRoute(x, childrenPage))))
    const route = {
      name,
      path,
      component: this.getComponent(item),
      // 组件 静态属性 meta 或者 options.meta
      // meta: lodash.get(component, 'meta', lodash.get(component, 'options.meta')),
      exact: true,
      // children
    }
    // 子路由
    // if (children && children.length > 0) {
    //   lodash.unset(route, 'name')
    // }
    // 守卫
    // if (beforeEnter) {
    //   route.beforeEnter = beforeEnter;
    // } else if (validate) {
    //   // 添加校验参数
    //   route.beforeEnter = async (to, from, next) => {
    //     if (await validate(to)) {
    //       next()
    //     } else {
    //       // next('/404')
    //       // message.warning({ content: `缺少参数 ${store.$global.dev && to.name}` });
    //       console.warn("LENG:", to)
    //       next('/')
    //     }
    //   };
    // }

    return route
  }
  getComponent(item) {
    const component = this.PageFiles(item);
    return component.default
  }
  /**
   * 获取路由name
   * @param {*} file type：string （文件完整的目录）
   */
  getRouteItemName(file) {
    let match = file.match(/\/(.+?)\.vue$/)[1] // 去除相对路径与.tsx
    let res = match.replace(/_/ig, '').replace(/\//ig, '-') // 把下划线去除， 改变/为-拼接
    return res.replace('-index', '').replace('-children', '')
  }

  /**
  * 获取路由path
  * @param {*} file String （目录，一级路由则为完整目录，多级为自身目录名称）
  */
  getRouteItemPath(file) {
    return file.replace('/index.vue', '').replace(/_/g, ':').replace(/\./g, '') || '/'
  }
}
export default new Router()
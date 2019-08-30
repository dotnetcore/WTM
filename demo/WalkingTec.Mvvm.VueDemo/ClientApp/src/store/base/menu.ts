// class Menu {
//     constructor() {
//     }
//     /**
//      * 获取菜单
//      */
//     async onInitMenu(menu: any[]) {
//         const development = true;
//         if (development) {
//             menu = await import("../../subMenu.json").then(x => x.default);
//         }
//         menu = _.map(menu, data => {
//             // 跨域页面
//             if (Regular.url.test(data.Url)) {
//                 data.Url = "/external/" + encodeURIComponent(data.Url);
//             } else
//                 // public 下的 pages 页面
//                 if (_.startsWith(data.Url, globalConfig.staticPage)) {
//                     data.Url = "/external/" + encodeURIComponent(_.replace(data.Url, globalConfig.staticPage, `${window.location.origin}`));
//                 }
//             return data;
//         })
//         this.setSubMenu(menu);
//     }
//     /**
//      * 递归 格式化 树
//      * @param datalist
//      * @param ParentId
//      * @param children
//      */
//     recursionTree(datalist, ParentId, children = []) {
//         _.filter(datalist, ['ParentId', ParentId]).map(data => {
//             children.push(data);
//             data.Children = this.recursionTree(datalist, data.Id, data.Children || [])
//         });
//         return children;
//     }
//     /**  设置菜单 */
//     setSubMenu(subMenu) {
//         this.ParallelMenu = subMenu;
//         const menu = this.recursionTree(subMenu, null, []);
//         this.subMenu = menu
//     }

// }
// export default new Menu();

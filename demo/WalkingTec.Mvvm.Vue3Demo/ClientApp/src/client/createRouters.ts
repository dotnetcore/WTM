

interface Config {
    rc: any
    redirect?: string
    rootFile?: string
    /** 过滤文件 */
    filter?: (file: any) => boolean
    /** 定义组件返回 */
    component: (file: any) => any
}
/**
 * https://github.com/MrHzq/vue-router-auto
 * rc：require.context 传入的文件
 * redirect：需要将根路由(/)重定向到的路由
 * rootFile：页面级别的.vue存放的文件名称
 * filter：过滤文件
 * component：定义组件返回
 */
export function createRouters(config: Config) {
    // 所生成的所有路由数组
    const Routers: any = []
    const defaultConfig: Config = {
        rc: null,
        redirect: '',
        rootFile: 'pages',
        filter: () => true,
        component: () => ({})
    }
    const { rc, redirect, filter, rootFile, component } = (<any>Object).assign(
        {},
        defaultConfig,
        config
    )
    if (rc === null) return Routers

    // allRouters[object]：存储所有路由的变量：先将所有的路由生成，然后放在该变量里面
    const allRouters: any = {
        len1: []
    }
    // 通过循环RC(传入的文件)
    const routerFileAndLen = rc
        .keys()
        .filter(filter)
        .map((fileName: any) => {
            // 因为得到的filename格式是: './baseButton.vue', 所以这里我们去掉头和尾，只保留真正的文件名
            const realFileName = fileName
                .replace(/^\.\//, '')
                .replace(/\.\w+$/, '')
            return {
                file: fileName,
                fileName: realFileName,
                // routerName(路由名称)：将 / 转为 - 并去掉 _
                routerName: realFileName.replace(/\//g, '-').replace(/_/g, '').replace(/-index/g, ''),
                // routerComponent(路由异步component的文件路径)：将 ./baseButton.vue 从 . 之后截取
                routerComponent: fileName.substr(1),
                // fileLen(文件的层级深度)：通过 / 的数量来判断当前文件的深度
                fileLen: fileName.match(/\//g).length
            }
        })
        .sort((i: any, j: any) => i.fileLen - j.fileLen) // 通过文件深度 升序排序

    // 传入文件中最大深度
    let maxLen = 0
    routerFileAndLen.map((r: any) => {
        const name = r.routerName
        // 生成一块路由对象，包含：name、fileName(用于后续处理真正path的依据)、path、needDelectName(用于后续处理，判断是否删除name的依据)、component
        const obj = {
            name,
            exact: true,
            fileName: r.fileName,
            // path：只是以name命名的path，还不是真正的路由path
            path: '/' + (name === 'index' ? '' : name),
            // needDelectName: name === 'index',
            needDelectName: false,
            component: component(rc(r.file))//() => import(`~/${rootFile}${r.routerComponent}`)
        }
        maxLen = r.fileLen
        // allRouters的key：以 'len'加当前的文件深度 作为key
        const key = 'len' + maxLen
        if (Array.isArray(allRouters[key])) allRouters[key].push(obj)
        else allRouters[key] = [obj]
    })

    // 将根目录层的路由放入Routers中
    // @ts-ignore
    Routers.push(...allRouters.len1)

    // 截取名称方法：从开始到最后一个'-'之间的字符串
    const substrName = (name: any) => name.substr(0, name.lastIndexOf('-'))

    /**
     * 正式生成路由：1、将相应的路由放在对应的路由下，形成子路由；2、生成同级路由
     * index：当前文件深度，为maxlen的倒序循环
     * nofindnum：未找到路由的次数
     * newcurr：当前新的深度下的路由数据
     */
    const ceateRouter = (index: any, nofindnum = 0, newcurr = null) => {
        // 当前深度下的路由数据：优先使用传入的newcurr，其次获取当前深度对应的路由数据
        const curr = newcurr || allRouters['len' + index]
        // 当前深度上一层的路由数据
        const pre = allRouters['len' + (index - 1)]
        // 若 没有上一层的数据了
        if (!pre) {
            // 则表明是属于顶层的路由
            curr.map((c: any) => {
                let path = '/' + c.fileName.replace('/index', '')
                // if (path.match('_')) path = path.replace('/_', '/:')
                // 将真正的路由path赋值给当前路由
                c.path = path
                // 将当前路由放到Routers里面
                Routers.push(c)
            })
            return
        }

        // 在上一层中 未找到的 当前深度路由数据
        let noFind: any = []

        // 循环当前深度路由数据
        curr?.map((c: any) => {
            // 在 上一层深度 的路由数据里面查找
            const fobj = pre.find((p: any) => {
                // 生成 当前深度 当前项 路由的name
                let name = substrName(c.name)
                // 循环nofindnum，当nofindnum>0，则表示已经出现：在上一层中未找到对应的父路由，则需要将 当前深度 当前项 路由的name 再次生成
                for (let i = 0; i < nofindnum; i++) {
                    name = substrName(name)
                }

                return name === p.name
            })
            // 如果 找到了 对应的 父路由数据(fobj)
            if (fobj) {
                // 生成 当前路由的path：1、去掉当前路由中与父路由重复的；2、去掉/；3、将 _ 转为 :；

                let path = c.fileName
                    .replace(fobj.fileName, '')
                    .substr(1)
                    .replace('_', ':')
                if (path.match('/') && !path.match('/:')) {
                    path = path.replace('/index', '')
                }
                if (path === undefined) {
                    throw new Error(
                        `找到了对应的父路由，但是生成子路由的path为【undefined】了`
                    )
                }

                // 将真正的路由path赋值给当前路由
                c.path = path

                // 若：当前路由为 index
                if (path === 'index') {
                    // 1、转为 '' path，''：表明是默认子路由，那父路由就不能存在name属性
                    c.path = ''
                    // 2、将父路由的needDelectName标记为true，表明需要删除它的name
                    fobj.needDelectName = fobj.needDelectName || true
                }
                // 将当前路由放到父路由的children里面
                if (Array.isArray(fobj.children)) fobj.children.push(c)
                else fobj.children = [c]
            } else noFind.push(c) // 表明未找到父路由，则先将当前路由的数据放入noFind中存储起来
        })

        // 若存在：未找到的路由数据，则再次向上一个层级寻找
        if (noFind.length) ceateRouter(index - 1, ++nofindnum, noFind)
    }
    // 倒序循环 最大深度，然后调用生成路由方法
    for (let i = maxLen; i > 1; i--) ceateRouter(i)

    // 路由生成完毕了，应该删除 有默认子路由的父路由的name属性
    const deleteNameFun = (arr: any) => {
        arr.map((r: any) => {
            // 删除多余的fileName属性
            delete r.fileName
            // 判断是否需要删除name属性
            if (r.needDelectName) delete r.name
            // 判断完毕了，则要删除needDelectName属性
            delete r.needDelectName
            // 若 存在子路由 则继续调用deleteNameFun，删除name
            if (Array.isArray(r.children)) deleteNameFun(r.children)
        })
    }
    // 调用deleteNameFun，先删除Routers的一级路由的name
    deleteNameFun(Routers)

    // 若存在重定向的路由，则加入重定向
    if (redirect) Routers.unshift({ path: '/', redirect })
    // 返回正儿八经的的路由数据
    return Routers
}
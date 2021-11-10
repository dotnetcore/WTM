
const path = require('path')
const lodash = require('lodash')
const fs = require('fs')
const getPathInfo = p => path.parse(p)

/**
 * @description // 递归读取文件，类似于webpack的require.context()
 * @param {String} directory 文件目录
 * @param {Boolean} useSubdirectories 是否查询子目录，默认false
 * @param {array} extList 查询文件后缀，默认 ['.js']
 */
module.exports = function (directory, useSubdirectories = false, extList = ['.vue']) {
    const filesList = []
    // 递归读取文件
    function readFileList(directory, useSubdirectories, extList) {
        const files = fs.readdirSync(directory)
        files.forEach(item => {
            const fullPath = path.join(directory, item)
            const stat = fs.statSync(fullPath)
            if (stat.isDirectory() && useSubdirectories) {
                readFileList(path.join(directory, item), useSubdirectories, extList)
            } else {
                const info = getPathInfo(fullPath)
                extList.includes(info.ext) && filesList.push(fullPath)
            }
        })
    }
    readFileList(directory, useSubdirectories, extList)
    // 生成需要的对象
    const res = filesList.filter(item => {
        return lodash.endsWith(item, 'index.vue')&&!lodash.includes(item, `/children/`)
    }).map(item => ({
        path: getRouteItemPath(item).split('\\').join('/').replace('src/pages',''),
        file: item,
        // data: require(item),
        ...getPathInfo(item),
    }))
    return lodash.map(res, 'path')
}

/**
 * 获取路由name
 * @param {*} file type：string （文件完整的目录）
 */
function getRouteItemName(file) {
    let match = file.match(/\/(.+?)\.vue$/)[1] // 去除相对路径与.vue
    let res = match.replace(/_/ig, '').replace(/\//ig, '-') // 把下划线去除， 改变/为-拼接
    return res.replace('-index', '').replace('-children', '')
}

/**
* 获取路由path
* @param {*} file String （目录，一级路由则为完整目录，多级为自身目录名称）
*/
function getRouteItemPath(file) {
    return file.replace('src/pages', '').replace('/index.vue', '').replace('index.vue', '').replace('vue', '').replace(/_/g, ':').replace(/\./g, '')
}
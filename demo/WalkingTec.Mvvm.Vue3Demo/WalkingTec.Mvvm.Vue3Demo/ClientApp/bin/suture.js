/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2020-09-28 16:58:43
 * @modify date 2020-09-28 16:58:43
 * @desc 缝合 脚本
 */
const program = require('commander');
const lodash = require('lodash');
const path = require('path');
const fs = require('fs-extra');
const package = require('../package.json');
const { workspaces } = require('../../../package.json');
try {
    program.version(package.version, '-v, --version')
        .parse(process.argv);
    const env = lodash.head(program.args) || 'dev';
    const root = process.cwd();
    const dir = `build_${lodash.snakeCase(package.version)}/${env}`;
    const rootDir = path.join(root, dir)
    const packages = lodash.map(workspaces, item => {
        return {
            name: lodash.replace(item, 'packages/', ''),
            path: path.join(root, item, dir)
        }
    }).sort(item => lodash.eq(item.name, 'www') ? -1 : 1);
    // 创建目录
    fs.emptyDirSync(rootDir)
    packages.forEach(item => {
        try {
            if (lodash.eq(item.name, 'client')) {
                return
            }
            if (fs.existsSync(item.path)) {
                if (lodash.eq(item.name, 'www')) {
                    return
                    // fs.copySync(item.path, rootDir)
                } else {
                    fs.copySync(item.path, path.join(rootDir, 'childrens', item.name))
                }
                console.log("LENG: suture " + item.name)
            }

        } catch (error) {
            console.log("LENG: error", error)
        }
    })
    // fs.copySync(rootDir, lodash.find(packages, ['name', 'www']).path)
} catch (error) {
    console.log("LENG: error", error)
}
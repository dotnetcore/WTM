const path = require('path');
const fs = require('fs');
import { Plugin } from 'vite'

export function wtmBuildPlugin(): Plugin {
    return {
        name: 'vite:wtmBuild',

        // 该插件在 plugin-vue 插件之前执行，这样就可以直接解析到原模板文件
        enforce: 'pre',

        // 代码转译，这个函数的功能类似于 `webpack` 的 `loader`
        buildStart(option) {
            var files = readDir(__dirname + "\\src\\views")
            var views = {}
            files.forEach((val) => {
                const content = fs.readFileSync(val).toString();
                const reg = /\<script.*?name=\"(.*)\"\>/
                if (reg.test(content) == true) {
                    var m = content.match(reg);
                    if (m?.length) {
                        const path = val.replace(/\\/g, "/").replace(__dirname.replace(/\\/g, "/") + "/src/views", "").replace("/index.vue", "").replace(".vue", "");
                        const name = m![1];
                        const narray = name.split(';');
                        var title = narray[0] ? narray[0] : path.replace(/\//, '_');
                        var ishide = (narray.length > 1 && narray[1] == "false") ? true : false;
                        var className = narray.length > 2 ? narray[2] : '';
                        if (narray.length > 3) {
                            className += "," + narray[3];
                        }

                        views[path] = {
                            title: title,
                            ishide: ishide,
                            className: className
                        }
                    }
                }
            })
            fs.writeFile(__dirname + "\\public\\menu.json", JSON.stringify(views), function err() { })
        }
    }
}

function readDir(dir) {
    return fs.readdirSync(dir).reduce((files, file) => {
        const filePath = path.join(dir, file);
        const stats = fs.statSync(filePath);
        // 判断是文件还是目录
        if (stats.isDirectory()) {
            // 递归地读取目录下所有文件和目录
            files = files.concat(readDir(filePath));
        } else if (stats.isFile()) {
            files.push(filePath);
        }
        return files;
    }, []);
}

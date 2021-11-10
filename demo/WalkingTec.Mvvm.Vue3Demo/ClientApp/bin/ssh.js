#!/usr/bin/env node
// https://www.npmjs.com/package/ssh2
// const Client = require('ssh2').Client;
const program = require('commander');
const lodash = require('lodash');
const path = require('path');
const fs = require('fs-extra');
const child_process = require('child_process');
const package = require('../package.json');
const { workspaces } = require('../../../package.json');
try {
    console.log('--------------------------------- 执行 ssh 发布 ---------------------------------')
    program.version(package.version, '-v, --version')
        .parse(process.argv);
    const env = lodash.head(program.args) || 'dev';
    const root = process.cwd();
    const version = lodash.snakeCase(package.version)
    const dir = `build_${version}`;
    const rootDir = path.join(root, dir);
    const tar = `${env}_${version}.tar.gz`;
    const IP = getIP(env);
    const envStr = getEnv(env);
    const ssh = `#!/bin/bash
    # author: lengyxin, vito
    
    rm -rf ./${tar}
    
    # 打包
    cd ./${env}/childrens && tar -zcf ../../${tar} * && cd ../../
   
    # 发布
    scp -P 22 -r ./${tar} root@${IP}:/data/websites/${envStr}_xt_bgframe/wwwroot/childrens
    
    # 解压
    ssh -p 22 root@${IP} "tar -xzf /data/websites/${envStr}_xt_bgframe/wwwroot/childrens/${tar} -C /data/websites/${envStr}_xt_bgframe/wwwroot/childrens"
    
    # rm -rf ./${tar}
    `
    fs.writeFileSync(path.join(rootDir, env + '.sh'), ssh);
    child_process.execSync(`chmod +x ./${env}.sh`, { cwd: rootDir });
    child_process.exec(`"./${env}.sh"`, { cwd: rootDir }, (error, stdout, stderr) => {
        console.log("");
        console.log('--------------------------------- sh ---------------------------------')
        console.log("");
        if (error) {
            console.error(`                        发布失败: ${error}`);
            console.log(`                          请手动执行脚本`);
            return;
        }
        console.log(`                              成功发布: ${env}`);
        console.log("");
        console.log('--------------------------------- sh ---------------------------------')
        console.log("");
    });
    function getIP(env) {
        const config = {
            // pro: "",
            test: '16.xuantong.cn',
            dev: '14.xuantong.cn'
        }
        return lodash.get(config, env, config.dev)
    }
    function getEnv(env) {
        const config = {
            pro: "prod",
            test: 'testing',
            dev: 'dev'
        }
        return lodash.get(config, env, config.dev)
    }
} catch (error) {
    console.log("LENG: error", error)
}

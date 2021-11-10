import cryptoMd5 from 'crypto-js/md5';
import lodash from 'lodash';
/**
 * 加密算法
 */
export const Encryption = {
    /** 随机 guid  */
    uniqueId,
    /**
     * 递归 格式化 树
     * @param datalist 
     * @param ParentId 
     * @param children 
     */
    recursionTree,
    /**
     * 转换 tree
     */
    toTree,
    /**
     * MD5
     * @param obj 
     * @returns 
     */
    MD5: (obj) => cryptoMd5(JSON.stringify(obj)).toString()
}
function uniqueId() {
    function GUID() {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
            let r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }
    return cryptoMd5(JSON.stringify(GUID())).toString()
}
/**
 * 递归 格式化 树
 * @param datalist 
 * @param ParentId 
 * @param children 
 */
function recursionTree(datalist, ParentId, children = []) {
    lodash.filter(datalist, ['ParentId', ParentId]).map(data => {
        data.children = recursionTree(datalist, data.Id, data.children || []);
        children.push(data);
    });
    return children;
}
function toTree(datalist: Array<any>, children = []) {
    lodash.map(datalist).map(data => {
        data.children = toTree(data.Children, [])
        children.push(lodash.assign({
            title: data.Text,
            key: data.Id,
        }, data));
    });
    return children;
}
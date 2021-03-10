import cryptoMd5 from 'crypto-js/md5';
/**
 * 加密算法
 */
export const Encryption = {
    /** 随机 guid  */
    uniqueId: uniqueId,
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

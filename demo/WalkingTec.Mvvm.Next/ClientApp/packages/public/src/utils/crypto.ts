import cryptoBase64 from 'crypto-js/enc-base64';
import cryptoUtf8 from 'crypto-js/enc-utf8';
/**
 *  base64 加密解密
 */
export const base64 = {
    /**
     * 加密
     */
    encryption: (str: string) => {
        return cryptoBase64.stringify(cryptoUtf8.parse(str));
    },
    /**
     * 解密
     */
    decrypt: (str: string) => {
        return cryptoBase64.parse(str).toString(cryptoUtf8);
    }
}
/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:53:01
 * @modify date 2018-09-12 18:53:01
 * @desc [description]
*/
import lodash from 'lodash';
export class Help {
    static GUID() {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
            let r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }
    /**
     * 比较Form 字段的 值 ，都转换为 字符串 比较
     * @param props 
     * @param Field 
     * @param eqValue 
     */
    static FormValueEqual(props: WTM.FormProps, Field, eqValue) {
        return lodash.eq(lodash.toString(props.form.getFieldValue(Field) || lodash.get(props.defaultValues, Field)), lodash.toString(eqValue))
    }

    static GetFormValue(props: WTM.FormProps, Field) {
        return lodash.toString(props.form.getFieldValue(Field) || lodash.get(props.defaultValues, Field))
    }
}
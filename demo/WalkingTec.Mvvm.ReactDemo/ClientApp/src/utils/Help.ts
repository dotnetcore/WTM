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
     * @param props 表单 props
     * @param Field 表单 id
     * @param eqValue 比较的字段
     * @param nullValues 表单属性 值 为空情况下 返回默认 布尔值
     */
    static FormValueEqual(props: WTM.FormProps, Field, eqValue, nullValues = false) {
        const FieldValue = Help.GetFormValue(props, Field);
        if (FieldValue === '' || FieldValue === undefined || FieldValue === null) {
            return nullValues
        }
        return lodash.eq(FieldValue, lodash.toString(eqValue))
    }

    static GetFormValue(props: WTM.FormProps, Field) {
        const FieldValue = lodash.toString(props.form.getFieldValue(Field));
        if (FieldValue === '' ||FieldValue === undefined || FieldValue === null) {
            return lodash.toString(lodash.get(props.defaultValues, Field))
        }
        return FieldValue
    }
}
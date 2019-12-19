/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-02-24 17:06:55
 * @modify date 2019-02-24 17:06:55
 * @desc [description]
 */
import * as React from 'react';
import lodash from "lodash";
import { Observable } from 'rxjs';
import { Spin, Icon, Tag } from 'antd';
interface IValueFormatterOptions {
    mapKey?: string;
    /**
     * 值转换
     * @memberof IValueFormatterOptions
     */
    valueFormatter?: (value) => any;
    /**
     * 值解析
     * @memberof IValueFormatterOptions
     */
    valueAnalysis?: (value) => any;
}
/**
 * 级联数据 装饰器
 * @param options 
 */
export function DesValueFormatter(options: IValueFormatterOptions = {}) {
    return function (node: JSX.Element) {
        console.log("TCL: DesValueFormatter -> node", node)
        return React.cloneElement(node, {

        })
    }
}
import { defineMetadata } from "./validate";

// 1.定义常量名
const EQUAL = Symbol("equal");
const NOT_EMPTY = Symbol("NOT_EMPTY");
const NUMBER = Symbol("NUMBER");
// 2.定义装饰器
/**
 * 检测这个数是否为数字
 * @param label
 */
export function isNum(label: string = "") {
    return function(target: any, propertyKey: string, index: number) {
        defineMetadata(NUMBER, { index, label }, propertyKey, target);
    };
}
/**
 * 检测数组或字符串不为空
 * @param label
 */
export function notEmpty(label: string) {
    return function(target: any, propertyKey: string, index: number) {
        defineMetadata(NOT_EMPTY, { index, label }, propertyKey, target);
    };
}
/**
 * 检测两个数是否相等，传一个string[]
 * @param label
 */
export function equal(label: string) {
    return function(target: any, propertyKey: string, index: number) {
        defineMetadata(EQUAL, { index, label }, propertyKey, target);
    };
}

// 3.定义检验方法
export const checkNum = (val: any) => {
    if (val instanceof Object) {
        const key = Object.keys(val)[0];
        const value = val[key];
        if (value === "") return true;
        return /^[0-9]+\.?[0-9]+?$/.test(val[key]);
    }
    return /^[0-9]+\.?[0-9]+?$/.test(val);
};
export const checkEqual = (val: string[]) => val[0] === val[1];
export const checkNotEmpty = (val: string | Array<any>) =>
    Array.isArray(val) ? !!val.length : !!val;

// 4.定义规则
export const rules = [
    {
        type: EQUAL,
        checkValue: checkEqual,
        message: "不相同"
    },
    {
        type: NOT_EMPTY,
        checkValue: checkNotEmpty,
        message: "不能为空"
    },
    {
        type: NUMBER,
        checkValue: checkNum,
        message: "不能输入非数字"
    }
];

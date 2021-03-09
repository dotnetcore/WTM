import { defineMetadata } from "./validate";
// 1.定义常量名
var EQUAL = Symbol("equal");
var NOT_EMPTY = Symbol("NOT_EMPTY");
var NUMBER = Symbol("NUMBER");
// 2.定义装饰器
/**
 * 检测这个数是否为数字
 * @param label
 */
export function isNum(label) {
    if (label === void 0) { label = ""; }
    return function (target, propertyKey, index) {
        defineMetadata(NUMBER, { index: index, label: label }, propertyKey, target);
    };
}
/**
 * 检测数组或字符串不为空
 * @param label
 */
export function notEmpty(label) {
    return function (target, propertyKey, index) {
        defineMetadata(NOT_EMPTY, { index: index, label: label }, propertyKey, target);
    };
}
/**
 * 检测两个数是否相等，传一个string[]
 * @param label
 */
export function equal(label) {
    return function (target, propertyKey, index) {
        defineMetadata(EQUAL, { index: index, label: label }, propertyKey, target);
    };
}
// 3.定义检验方法
export var checkNum = function (val) {
    if (val instanceof Object) {
        var key = Object.keys(val)[0];
        var value = val[key];
        if (value === "")
            return true;
        return /^[0-9]+\.?[0-9]+?$/.test(val[key]);
    }
    return /^[0-9]+\.?[0-9]+?$/.test(val);
};
export var checkEqual = function (val) { return val[0] === val[1]; };
export var checkNotEmpty = function (val) {
    return Array.isArray(val) ? !!val.length : !!val;
};
// 4.定义规则
export var rules = [
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
//# sourceMappingURL=rules.js.map
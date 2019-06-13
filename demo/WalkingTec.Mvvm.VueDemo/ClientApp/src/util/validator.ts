export default {
    validatePassword(rule, value, callback) {
        if (value.length < 8) {
            callback(new Error("密码长度不够8位"));
        } else if (!/\d/.test(value) || !/[a-zA-Z]/.test(value)) {
            callback(new Error("密码必须含有数字和字母"));
        } else {
            callback();
        }
    },
    validateConfirmPassword(rule, value, callback) {
        if (value.length < 8) {
            callback(new Error("密码长度不够8位"));
        } else if (!/\d/.test(value) || !/[a-zA-Z]/.test(value)) {
            callback(new Error("密码必须含有数字和字母"));
        } else if (this.form.password !== value) {
            callback(new Error("两次输入的密码不一致,请重新输入"));
        } else {
            callback();
        }
    },
    // 正整数
    validatePositiveIntegers(rule, val, callback) {
        if (!/^[1-9]\d*$/.test(val)) {
            callback(new Error("请输入正整数"));
        } else {
            callback();
        }
    },
    // 正整数(可以为空)
    validatePositiveIntegersOrEmpty(rule, val, callback) {
        if (val === "") {
            callback();
        } else if (!/^[1-9]\d*$/.test(val)) {
            callback(new Error("请输入正整数"));
        } else {
            callback();
        }
    },
    // 正整数
    validatePositiveS(rule, val, callback) {
        if (!/^[0-9]+\.?[0-9]+?$/.test(val)) {
            callback(new Error("请输入小数"));
        } else {
            callback();
        }
    },
    //数字范围
    validateValueRange(rule, value, callback, condition) {
        var min = condition.min;
        var max = condition.max;
        if (value < min || value > max) {
            callback(new Error("数字范围在" + min + "~" + max + "之间"));
        } else {
            callback();
        }
    },
    // 手机号
    validateMobile(value) {
        if (
            value === "" ||
            !/^1(3|4|5|6|7|8|9)[0-9]\d{8}$/.test(value.trim())
        ) {
            return false;
        } else {
            return true;
        }
    },
    // 不能包含中文
    validateNotChinese(rule, value, callback) {
        if (/^[\u4e00-\u9fa5]*$/.test(value)) {
            callback(new Error("不能输入中文"));
        } else {
            callback();
        }
    },
    // 是否微信
    isWeixin() {
        // eslint-disable-next-line
        const ua = navigator.userAgent.toLowerCase();
        const obj = ua.indexOf("micromessenger") !== -1;
        if (obj) {
            return true;
        } else {
            return false;
        }
    },
    // 是否安卓
    isAndroid() {
        // eslint-disable-next-line
        const ua = navigator.userAgent.toLowerCase();
        const obj = ua.indexOf("android") !== -1 || ua.indexOf("linux") > -1;
        if (obj) {
            return true;
        } else {
            return false;
        }
    },
    // 数
    validatePositiveDecimal(val) {
        if (!/^[0-9]+\.?[0-9]+?$/.test(val)) {
            return false;
        } else {
            return true;
        }
    }
};

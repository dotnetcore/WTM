module.exports = {
    root: true,
    parserOptions: {
        parser: "babel-eslint",
        ecmaFeatures: {
            legacyDecorators: true
        }
    },
    env: {
        browser: true,
        node: true,
        mocha: true
    },
    globals: {
        expect: true
    },
    extends: ["plugin:vue/recommended", "standard"],
    plugins: ["vue"],
    rules: {
        semi: 0,
        "no-spaced-func": 0, // 函数调用时 函数名与()之间不能有空格
        "space-before-function-paren": [0, "always"], //函数定义时括号前面要不要有空格
        "no-unneeded-ternary": 0, // 禁止不必要的嵌套 var isYes = answer === 1 ? true : false;
        "no-trailing-spaces": 1, // 一行结束后面不要有空格
        "spaced-comment": 0, // 注释风格不要有空格什么的
        "no-multiple-empty-lines": [1, { max: 2 }], //空行最多不能超过2行
        indent: [2, 4], // 缩进风格
        quotes: ["off", "double"],
        "func-style": 0,
        "generator-star-spacing": "off",
        "no-debugger": process.env.NODE_ENV === "production" ? "error" : "off",
        "vue/max-attributes-per-line": [
            2,
            {
                singleline: 10, //当开始标记在一行中时每行的最大属性数，这里设置为10
                multiline: {
                    max: 1, //当开始标记位于多行时每行的最大属性数，这里设置为1
                    allowFirstLine: false //多行时禁止与标记名称在同一行写属性
                }
            }
        ]
    }
};

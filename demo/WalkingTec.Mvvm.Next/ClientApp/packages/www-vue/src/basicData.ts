import code from "./assets/img/code.png";
import logo from "./assets/img/logo.png";
const BasicData = {
    imgs: {
        code,
        logo
    },
    links: [
        { name: "GitHub", url: "https://github.com/dotnetcore/WTM" },
        { name: "Vue", url: "https://cn.vuejs.org/" },
        { name: "Ant Design of Vue", url: "https://www.antdv.com/docs/vue/introduce-cn/" },
        { name: "Rxjs", url: "https://rxjs.dev/" },
        { name: "Mobx", url: "https://mobx.js.org/" },
        { name: "Lodash", url: " https://lodash.com/" }
    ]
}
export default BasicData
declare module 'vue/types/vue' {
    interface Vue {
        $BasicData: typeof BasicData;
    }
}
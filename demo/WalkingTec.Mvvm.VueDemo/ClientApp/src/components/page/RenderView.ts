import vuemin from "vue/dist/vue";
import { paramTemplate } from "@/util/string";
/**
 * 函数式组件
 * string 转 jsx方案，
 * 待优化
 * 没有data规则如：https://cn.vuejs.org/v2/guide/render-function.html#%E5%87%BD%E6%95%B0%E5%BC%8F%E7%BB%84%E4%BB%B6
 */
const RenderView = {
  functional: true,
  render(h, context) {
    let strHtml = `<div style="${context.props.wrapStyle}"><template>${context.props.hml}</template></div>`;
    strHtml = paramTemplate(strHtml, context.props.params);
    return vuemin.compile(strHtml).render.apply(context, arguments);
  },
  props: {
    hml: {
      type: String,
      required: true
    },
    wrapStyle: {
      type: String,
      default: "display: inline-block;"
    },
    params: {
      type: Object,
      default: () => {}
    }
  }
};

export default RenderView;

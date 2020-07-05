/**
 * el-table 内容自适应
 * el-height-full-auto-table
 */
import { DirectiveOptions } from "vue";
import { debounce } from "@/util/throttle-debounce.ts";
const PAUSE = 300;
const BOTTOM_OFFSET = 68;
const doResize = (el, binding, vnode) => {
  const { componentInstance: $table } = vnode;
  // if (!$table.height) {
  //   throw new Error(`el-$table must set the height. Such as height='100px'`);
  // }
  if (!$table) return;
  if (el.offsetTop !== 0) {
    const height =
      window.innerHeight - el.offsetTop - BOTTOM_OFFSET;
    $table.layout.setHeight(height);
    $table.doLayout();
  }
};
const debounceFn = debounce(doResize, PAUSE);
const elHeightAdaptiveTable: DirectiveOptions = {
  bind(el, binding, vnode) {
    if (binding.value && binding.value !== 0) {
      el.resizeListener = () => {
        debounceFn(el, binding, vnode);
      };
      window.addEventListener("resize", el.resizeListener);
    }
  },
  update(el, binding, vnode) {
    if (binding.value && binding.value !== 0) {
      debounceFn(el, binding, vnode);
    }
  },
  unbind(el) {
    window.removeEventListener("resize", el.resizeListener);
  }
};
export default elHeightAdaptiveTable;

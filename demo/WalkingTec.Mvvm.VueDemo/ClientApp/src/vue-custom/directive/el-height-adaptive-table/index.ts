/**
 * el-table 内容自适应
 */
import { DirectiveOptions } from "vue";
import { debounce } from "@/util/throttle-debounce.ts";
const PAUSE = 300;
const BOTTOM_OFFSET = 68;
const doResize = (el, binding, vnode) => {
  const { componentInstance: $table } = vnode;
  if (!$table.height) {
    throw new Error(`el-$table must set the height. Such as height='100px'`);
  }
  const { value } = binding;
  const bottomOffset = value || BOTTOM_OFFSET;
  if (!$table) return;
  const height =
    window.innerHeight - el.getBoundingClientRect().top - bottomOffset;
  const scrollHeight = $table.bodyWrapper.scrollHeight;
  if (scrollHeight > height) {
    $table.layout.setHeight(height);
  } else {
    $table.layout.setHeight(scrollHeight + 2 + $table.layout.headerHeight);
  }
  $table.doLayout();
};
const debounceFn = debounce(doResize, PAUSE);
const elHeightAdaptiveTable: DirectiveOptions = {
  bind(el, binding, vnode) {
    el.resizeListener = () => {
      debounceFn(el, binding, vnode);
    };
    //addResizeListener(el, el.resizeListener)
    window.addEventListener("resize", el.resizeListener);
  },
  update(el, binding, vnode) {
    debounceFn(el, binding, vnode);
  },
  unbind(el) {
    //removeResizeListener(el, el.resizeListener)
    window.removeEventListener("resize", el.resizeListener);
  }
};
export default elHeightAdaptiveTable;

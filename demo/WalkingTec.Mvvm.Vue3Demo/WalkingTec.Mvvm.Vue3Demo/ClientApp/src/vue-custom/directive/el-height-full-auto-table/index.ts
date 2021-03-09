/**
 * el-table 内容自适应
 * el-height-full-auto-table
 */
import { DirectiveOptions } from "vue";
import { debounce } from "@/util/throttle-debounce.ts";
const PAUSE = 300;
const BOTTOM_OFFSET = 68;

// todo update : 截流
const getElementTop = (element) => {
  let actualTop = element.offsetTop;
  let current = element.offsetParent;
  while (current !== null) {
    actualTop += current.offsetTop;
    current = current.offsetParent;
  }
  return actualTop;
}


const doResize = (el, vnode) => {
  const { componentInstance: $table } = vnode;
  if (!$table) return;
  $table.$nextTick(() => {
    const offsetTop = getElementTop(el);
    const height =
        window.innerHeight - offsetTop - BOTTOM_OFFSET;
    $table.layout.setHeight(height - 2);
    $table.doLayout();
  })
};

const debounceFn = debounce(doResize, PAUSE);

const elHeightAdaptiveTable: DirectiveOptions = {
  // inserted: (el, { value }, vnode) => {
  //   if (value && value !== 0) {
  //       const { componentInstance: $table } = vnode;
  //       if ($table) {
  //         $table.layout.setHeight(value);
  //       }
  //   }
  // },
  bind(el, { value }, vnode) {
    if (!value || value === 0) {
      el.resizeListener = () => {
        debounceFn(el, vnode);
      };
      window.addEventListener("resize", el.resizeListener);
    }
  },
  update(el, { value }, vnode) {
    if (!value || value === 0) {
      debounceFn(el, vnode);
    }
  },
  unbind(el) {
    window.removeEventListener("resize", el.resizeListener);
  }
};
export default elHeightAdaptiveTable;

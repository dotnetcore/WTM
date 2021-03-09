import { debounce } from "@/util/throttle-debounce.ts";
var PAUSE = 300;
var BOTTOM_OFFSET = 68;
// todo update : 截流
var getElementTop = function (element) {
    var actualTop = element.offsetTop;
    var current = element.offsetParent;
    while (current !== null) {
        actualTop += current.offsetTop;
        current = current.offsetParent;
    }
    return actualTop;
};
var doResize = function (el, vnode) {
    var $table = vnode.componentInstance;
    if (!$table)
        return;
    $table.$nextTick(function () {
        var offsetTop = getElementTop(el);
        var height = window.innerHeight - offsetTop - BOTTOM_OFFSET;
        $table.layout.setHeight(height - 2);
        $table.doLayout();
    });
};
var debounceFn = debounce(doResize, PAUSE);
var elHeightAdaptiveTable = {
    // inserted: (el, { value }, vnode) => {
    //   if (value && value !== 0) {
    //       const { componentInstance: $table } = vnode;
    //       if ($table) {
    //         $table.layout.setHeight(value);
    //       }
    //   }
    // },
    bind: function (el, _a, vnode) {
        var value = _a.value;
        if (!value || value === 0) {
            el.resizeListener = function () {
                debounceFn(el, vnode);
            };
            window.addEventListener("resize", el.resizeListener);
        }
    },
    update: function (el, _a, vnode) {
        var value = _a.value;
        if (!value || value === 0) {
            debounceFn(el, vnode);
        }
    },
    unbind: function (el) {
        window.removeEventListener("resize", el.resizeListener);
    }
};
export default elHeightAdaptiveTable;
//# sourceMappingURL=index.js.map
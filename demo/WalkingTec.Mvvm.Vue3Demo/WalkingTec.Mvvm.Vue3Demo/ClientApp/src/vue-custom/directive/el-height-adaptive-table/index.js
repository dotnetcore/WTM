import { debounce } from "@/util/throttle-debounce.ts";
var PAUSE = 300;
var BOTTOM_OFFSET = 68;
var doResize = function (el, binding, vnode) {
    var $table = vnode.componentInstance;
    if (!$table.height) {
        throw new Error("el-$table must set the height. Such as height='100px'");
    }
    var value = binding.value;
    var bottomOffset = _.isNumber(value) ? value : BOTTOM_OFFSET;
    if (!$table)
        return;
    var height = window.innerHeight - el.getBoundingClientRect().top - bottomOffset;
    var scrollHeight = $table.bodyWrapper.scrollHeight;
    if (scrollHeight === 0) {
        return;
    }
    if (scrollHeight > height) {
        $table.layout.setHeight(height);
    }
    else {
        $table.layout.setHeight(scrollHeight + 2 + $table.layout.headerHeight);
    }
    $table.doLayout();
};
var debounceFn = debounce(doResize, PAUSE);
var elHeightAdaptiveTable = {
    inserted: function (el, _a, vnode) {
        var value = _a.value;
        if (value) {
            var $table = vnode.componentInstance;
            if ($table) {
                $table.layout.setHeight("123", "min-height");
            }
        }
    },
    bind: function (el, binding, vnode) {
        if (binding.value) {
            el.resizeListener = function () {
                debounceFn(el, binding, vnode);
            };
            //addResizeListener(el, el.resizeListener)
            window.addEventListener("resize", el.resizeListener);
        }
    },
    update: function (el, binding, vnode) {
        if (binding.value) {
            debounceFn(el, binding, vnode);
        }
    },
    unbind: function (el) {
        //removeResizeListener(el, el.resizeListener)
        window.removeEventListener("resize", el.resizeListener);
    }
};
export default elHeightAdaptiveTable;
//# sourceMappingURL=index.js.map
/**
 * dialog 拖拽
 */
var elDraggableDialog = {
    bind: function (el, _, vnode) {
        var dragDom = el.querySelector(".el-dialog");
        var dialogHeaderEl = el.querySelector(".el-dialog__header");
        dragDom.style.cssText += ";top:0px;";
        dialogHeaderEl.style.cssText += ";cursor:move;";
        dialogHeaderEl.onmousedown = function (e) {
            var disX = e.clientX - dialogHeaderEl.offsetLeft;
            var disY = e.clientY - dialogHeaderEl.offsetTop;
            var dragDomWidth = dragDom.offsetWidth;
            var dragDomHeight = dragDom.offsetHeight;
            var screenWidth = document.body.clientWidth;
            var screenHeight = document.body.clientHeight;
            var minDragDomLeft = dragDom.offsetLeft;
            var maxDragDomLeft = screenWidth - dragDom.offsetLeft - dragDomWidth;
            var minDragDomTop = dragDom.offsetTop;
            var maxDragDomTop = screenHeight - dragDom.offsetTop - dragDomHeight;
            var styleLeftStr = getComputedStyle(dragDom)["left"];
            var styleTopStr = getComputedStyle(dragDom)["top"];
            if (!styleLeftStr || !styleTopStr)
                return;
            var styleLeft;
            var styleTop;
            // Format may be "##%" or "##px"
            if (styleLeftStr.includes("%")) {
                styleLeft =
                    +document.body.clientWidth * (+styleLeftStr.replace(/%/g, "") / 100);
                styleTop =
                    +document.body.clientHeight * (+styleTopStr.replace(/%/g, "") / 100);
            }
            else {
                styleLeft = +styleLeftStr.replace(/px/g, "");
                styleTop = +styleTopStr.replace(/px/g, "");
            }
            document.onmousemove = function (e) {
                var left = e.clientX - disX;
                var top = e.clientY - disY;
                // Handle edge cases
                if (-left > minDragDomLeft) {
                    left = -minDragDomLeft;
                }
                else if (left > maxDragDomLeft) {
                    left = maxDragDomLeft;
                }
                if (-top > minDragDomTop) {
                    top = -minDragDomTop;
                }
                else if (top > maxDragDomTop) {
                    top = maxDragDomTop;
                }
                // Move current element
                dragDom.style.cssText += ";left:" + (left + styleLeft) + "px;top:" + (top +
                    styleTop) + "px;";
                // Emit onDialogDrag event
                // See https://stackoverflow.com/questions/49264426/vuejs-custom-directive-emit-event
                if (vnode.componentInstance) {
                    vnode.componentInstance.$emit("onDialogDrag");
                }
                else if (vnode.elm) {
                    vnode.elm.dispatchEvent(new CustomEvent("onDialogDrag"));
                }
            };
            document.onmouseup = function () {
                document.onmousemove = null;
                document.onmouseup = null;
            };
        };
    }
};
export default elDraggableDialog;
//# sourceMappingURL=index.js.map
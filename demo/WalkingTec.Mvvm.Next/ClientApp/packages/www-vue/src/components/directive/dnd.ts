
import Hammer from 'hammerjs';
import lodash from 'lodash';
import Vue from 'vue';
export default (vue: typeof Vue) => {
    vue.directive('w-swipe', {
        // 当被绑定的元素插入到 DOM 中时……
        bind(el, binding) {

        },
        // componentUpdated(el){
        //     console.error("TCL: bind -> el", el)

        // },
        inserted(el: any, binding) {
            if (lodash.eq(binding.value, "page-action-modal")) {
                const header = el.offsetParent.querySelector('.ant-modal-header');
                el = header || el;
            }
            const hammer = new Hammer(el)
            el.hammer = hammer;
            el.style.cursor = 'move';
            // el.deltaX = 0;
            // el.deltaY = 0;
            // hammer.on('panstart', (event) => {
            // });
            hammer.on('panmove', (event) => {
                onUpdate(lodash.eq(binding.value, "page-action-modal") ? el.offsetParent : el, event.deltaX, event.deltaY);
            });
            hammer.on('panend', (event) => {
                // this.panstart = false
            });
            function onUpdate(element, left, top) {
                element.style.left = left + 'px';
                element.style.top = top + 'px';
            }
        },
        // update(el){
        //     console.error("TCL: bind -> el", el)

        // },
        unbind(el: any) {
            el.hammer && el.hammer.destroy()
        }
    });
}
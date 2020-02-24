
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
            const isModal = lodash.eq(binding.value, "page-action-modal");
            let HammerElement = el;
            if (isModal) {
                const header = el.offsetParent.querySelector('.ant-modal-header');
                HammerElement = header || el;
            }
            const hammer = new Hammer(HammerElement);
            el.hammer = hammer;
            el.style.cursor = 'move';
            el.deltaX = 0;
            el.deltaY = 0;
            hammer.on('panstart', (event) => {
                // el.deltaX = 0;
                // el.deltaY = 0;
            });
            hammer.on('panmove', (event) => {
                const x = event.deltaX + el.deltaX;
                let y = event.deltaY + el.deltaY;
                if (isModal && y <= -50) {
                    y = -50;
                }
                onUpdate(isModal ? el.offsetParent : el, x, y);
            });
            hammer.on('panend', (event) => {
                el.deltaX = lodash.get(el, 'UpdateStyle.left');
                el.deltaY = lodash.get(el, 'UpdateStyle.top');
            });
            function onUpdate(element, left, top) {
                element.style.left = left + 'px';
                element.style.top = top + 'px';
                el.UpdateStyle = {
                    left,
                    top
                }
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
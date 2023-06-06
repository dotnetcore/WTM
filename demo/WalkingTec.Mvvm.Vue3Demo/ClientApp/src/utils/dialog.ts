import { ElDialog } from 'element-plus';
import { createApp,h } from 'vue';
import { i18n } from '/@/i18n/index';
import ElementPlus from 'element-plus';
export const Dialog={
    open:(title:string,component:any)=>{
        const div = document.createElement('div');
        document.body.appendChild(div);
        const close = () => {
            //@ts-ignore
            app.unmount(div);
            div.remove();
          };
        const app = createApp({
            render() {
              return h(
                ElDialog,
                {                  
                  modelValue: true,                
                  onClosed:()=>{close();}  
                },
                ()=>h(component)
              );
            }
          });
          app.use(ElementPlus).use(i18n).mount(div);
    }
}
<template>
    <div>
        <template v-if="_readonly">
            <div class="ql-editor" v-html="value"></div>
        </template>
        <!-- <template v-else> -->
        <div class="w-quill-container" v-show="!_readonly">
            <div ref="container"></div>
        </div>
        <!-- </template> -->
    </div>
</template>
  <script lang="ts">
import { Watch, Options, Ref, mixins, Inject } from "vue-property-decorator";
import Quill from "quill";
import "quill/dist/quill.snow.css";
import ImageResize from 'quill-image-resize-module';
Quill.register('modules/imageResize',ImageResize);
import { FieldBasics } from "../script";
@Options({ components: {} })
export default class extends mixins(FieldBasics) {
    // 表单状态值
    @Inject() readonly formState;
    // 自定义校验状态
    @Inject() readonly formValidate;
    // 实体
    @Inject() readonly PageEntity;
    // 表单类型
    @Inject({ default: '' }) readonly formType;
    @Ref('container') container: HTMLDivElement
    Quill: Quill;
    async mounted() {
        this.createQuill()
        console.log(this._name);
        //this.onValueChange(this.value, undefined);
        // this.onRequest();
        if (this.debug) {
            console.log("");
            console.group(`Field ~ ${this.entityKey} ${this._name} `);
            console.log(this);
            console.groupEnd();
        }
    }
    createQuill() {
        this.lodash.set(Quill.prototype, 'getHTML', () => {
            return this.container.querySelector(".ql-editor").innerHTML;
        })
        this.lodash.set(Quill.prototype, 'getWordCount', () => {
            return this.container.querySelector<HTMLDivElement>(".ql-editor").innerText.length;
        })
        this.Quill = new Quill(this.container, {
            debug: this.debug,
            modules: {
                // 工具栏配置
              toolbar:{
                container:[
                  ["bold", "italic", "underline", "strike"],       // 加粗 斜体 下划线 删除线
                  ["blockquote", "code-block"],                    // 引用  代码块
                  [{ list: "ordered" }, { list: "bullet" }],       // 有序、无序列表
                  [{ indent: "-1" }, { indent: "+1" }],            // 缩进
                  [{ size: ["small", false, "large", "huge"] }],   // 字体大小
                  [{ header: [1, 2, 3, 4, 5, 6, false] }],         // 标题
                  [{ color: [] }, { background: [] }],             // 字体颜色、字体背景颜色
                  [{ align: [] }],                                 // 对齐方式
                  ["link", "image", "video"], // 链接、图片、视频
                ]
              },
              imageResize: {
                    displayStyles: {
                      backgroundColor: "black",
                      border: "none",
                      color: "white",
                    },
                    modules: ["Resize", "DisplaySize", "Toolbar"],
             },
            },
            placeholder: 'Compose an epic...',
            //readOnly: true,
            theme: 'snow'
        })
        let that = this
        setTimeout(()=>{
          if (that.value) {
              that.Quill.root.innerHTML = that.value
          }
          that.Quill.on('text-change', (delta, oldContents, source) => {
              that.value = that.lodash.invoke(that.Quill, 'getHTML')
          })
        },2000)
        
        // this.Quill.on('editor-change', (delta, oldContents, source) => {
        //     console.log("LENG ~ editor-change", delta, oldContents, source)
        // })
    }

    /*@Watch("value")
    onValueChange(val, old) {
      if (this.value) {
          this.Quill.root.innerHTML = val
      }
      this.Quill.on('text-change', (delta, oldContents, source) => {
          this.value = this.lodash.invoke(this.Quill, 'getHTML')
      })
    }*/

}
</script>
  <style lang="less">
</style>
  
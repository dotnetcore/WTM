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
                // toolbar: '#toolbar'
            },
            placeholder: 'Compose an epic...',
            // readOnly: true,
            theme: 'snow'
        })
        if (this.value) {
            this.Quill.root.innerHTML = this.value
        }
        this.Quill.on('text-change', (delta, oldContents, source) => {
            this.value = this.lodash.invoke(this.Quill, 'getHTML')
        })
        // this.Quill.on('editor-change', (delta, oldContents, source) => {
        //     console.log("LENG ~ editor-change", delta, oldContents, source)
        // })
    }
    @Watch('disabled')
    onWatchDisabled() {
        this.Quill.enable(!this.disabled)
    }

}
</script>
  <style lang="less">
</style>
  
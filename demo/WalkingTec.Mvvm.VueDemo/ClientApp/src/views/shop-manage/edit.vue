<template>
    <div class="edit">
        <template v-if="isEdit">
            <slot></slot>
            <span v-show="error" class="val-error">
                {{error}}
            </span>
        </template>
        <template v-else>
            <img v-if="isImg" :src="val">
            <span v-else class="edit-value">{{val}}</span>
        </template>
    </div>
</template>

<script lang='ts'>
import { Component, Vue, Prop } from "vue-property-decorator";
import { Action, State } from "vuex-class";

@Component
export default class Edit extends Vue {
    @Prop({ type: String })
    val;
    @Prop({ type: String })
    error;
    @Prop({ type: Boolean })
    isEdit;
    @Prop({ type: String })
    isImg;
    created() {}
}
</script>
<style lang='less'>
@import "~@/assets/css/variable.less";
.edit {
    display: inline-block;
    img {
        border-radius: 6px;
        width: 360vw * @vwUnit;
        height: 180vw * @vwUnit;
    }
    .edit-value {
        font-size: 14px;
        font-weight: 400;
        color: #303133;
    }
    .val-error {
        font-size: 12px;
        color: #fc633d;
        margin-left: 16vw * @vwUnit;
    }
}
</style>

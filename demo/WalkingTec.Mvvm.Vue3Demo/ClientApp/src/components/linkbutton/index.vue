<template>
    <span class="linkbutton">
        <template v-if="props.confirm">
            <el-popconfirm :title="props.confirm" @confirm="doClick">
                <template #reference>
                    <ElCard v-if="props.isShortCut">
                        <a @click="doClick">
                            <div><span :class="props.icon" :style="{ color: ic }"></span></div>
                            {{ props.buttonText }}
                        </a>
                    </ElCard>
                    <el-button :class="props.class" :style="props.style" v-else :type="props.type" :text="props.isText">
                        <i v-if="props.icon&&props.isText==false" :class="props.icon"></i>
                        {{ props.buttonText }}
                    </el-button>
                </template>
            </el-popconfirm>
        </template>
        <template v-else>
            <ElCard v-if="props.isShortCut">
                <a @click="doClick">
                    <div><span :class="props.icon" :style="{ color: ic }"></span></div>
                    {{ props.buttonText }}
                </a>
            </ElCard>
            <el-button v-else :type="props.type" :class="props.class" :style="props.style" :text="props.isText"
                @click="doClick">
                <i v-if="props.icon&&props.isText==false" :class="props.icon"></i>
                {{ props.buttonText }}
            </el-button>

        </template>
    </span>
</template>

<script setup lang="ts">
import { defineAsyncComponent, computed, nextTick, ref, getCurrentInstance } from 'vue';
import other from '/@/utils/other';
import { storeToRefs } from 'pinia';
import { useThemeConfig } from '/@/stores/themeConfig';

const viewsModules: any = import.meta.glob('../../views/**/*.{vue,tsx}');
const storesThemeConfig = useThemeConfig();
const { themeConfig } = storeToRefs(storesThemeConfig);
const emit = defineEmits(['click']);
const props = defineProps({
    type: {
        type: String,
        default: 'primary',
    },
    confirm: {
        type: String,
        default: null,
    },
    url: {
        type: String,
        default: null,
    },
    target: {
        type: String,
        default: 'self', //self,newwindow,dialog
    },
    buttonText: {
        type: String,
        default: '',
    },
    icon: {
        type: String,
        default: null,
    },
    isShortCut: {
        type: Boolean,
        default: false,
    },
    isText: {
        type: Boolean,
        default: false
    },
    dialogTitle: {
        type: String,
        default: '',
    },
    iconColor: {
        type: String,
        default: null,
    },
    class: null,
    style: null
});

const ic = computed(() => {
    if (!props.iconColor) {
        return themeConfig.value.primary
    }
    else {
        return props.iconColor
    }
})


const doClick = async () => {
    if (props.url) {
        if (props.target == "self") {
            window.location.href = "/#" + props.url.replace(/\/index$/, '');
        }
        else if (props.target == "dialog") {
            const createDialog = defineAsyncComponent(viewsModules[`../../views${props.url}.vue`]);
            other.openDialog(props.dialogTitle, createDialog)
        }
        else {
            window.open("/#" + props.url.replace(/\/index$/, ''));
        }
    }
    emit("click");
}
</script>
<style scoped lang="scss">
a {
    color: #777;
    text-align: center;
    display: block;
    cursor: pointer;

    div {
        color: #333;
        padding: 0.6rem 1rem;
        font-size: 40px;
        margin-bottom: 0.8rem
    }
}
</style>

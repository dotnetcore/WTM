import headerIcon from './headerIcon.vue';
import noRowsOverlay from './noRowsOverlay.vue';
import loadingOverlay from './loadingOverlay.vue';
import image from './image.vue';
import icons from './icons.vue';
import renSwitch from './switch.vue';

export default {
    headerIcon,
    noRowsOverlay,
    loadingOverlay,
    image,
    icons,
    switch: renSwitch
}
/**
 * 枚举
 */
export enum frameworkComponents {
    /** 标题图标 */
    headerIcon = 'headerIcon',
    /** 空数据 */
    noRowsOverlay = 'noRowsOverlay',
    loadingOverlay = 'loadingOverlay',
    image = 'image',
    icons = 'icons',
    switch = 'switch',
}
declare module '@vue/runtime-core' {
    interface ComponentCustomProperties {
        $FrameworkComponents: typeof frameworkComponents
    }
}
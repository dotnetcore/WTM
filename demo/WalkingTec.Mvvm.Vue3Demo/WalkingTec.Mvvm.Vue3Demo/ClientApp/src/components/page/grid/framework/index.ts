import headerIcon from './headerIcon.vue';
import noRowsOverlay from './noRowsOverlay.vue';
import loadingOverlay from './loadingOverlay.vue';
import image from './image.vue';
import renWwitch from './switch.vue';

export default {
    headerIcon,
    noRowsOverlay,
    loadingOverlay,
    image,
    switch: renWwitch
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
    switch = 'switch',
}
declare module '@vue/runtime-core' {
    interface ComponentCustomProperties {
        $FrameworkComponents: typeof frameworkComponents
    }
}
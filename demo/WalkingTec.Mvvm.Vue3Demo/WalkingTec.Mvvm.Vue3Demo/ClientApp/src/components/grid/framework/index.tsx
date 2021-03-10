import headerIcon from './headerIcon.vue';
import noRowsOverlay from './noRowsOverlay.vue';
import loadingOverlayComponent from './loadingOverlayComponent.vue';

export default {
    headerIcon,
    noRowsOverlay,
    loadingOverlayComponent
}
/**
 * 枚举
 */
export enum frameworkComponents {
    /** 标题图标 */
    headerIcon = 'headerIcon',
    /** 空数据 */
    noRowsOverlay = 'noRowsOverlay',
    loadingOverlayComponent = 'loadingOverlayComponent',
}
declare module '*.svg'
declare module '*.png'
declare module '*.jpg'
declare module '*.jpeg'
declare module '*.gif'
declare module '*.bmp'
declare module '*.tiff'
declare module '*.json'
declare namespace WTM {
    /**
     * 动作
     */
    interface IActions {
        /** 添加按钮 */
        insert: boolean;
        /** 添加修改 */
        update: boolean;
        /** 删除按钮 */
        delete: boolean;
        /** 导入按钮 */
        import: boolean;
        /** 导出按钮 */
        export: boolean;
        [key: string]: boolean
    }
    interface IUrl {
        src: string;
        method: string;
        [key: string]: any;
    }
    interface IUrls {
        search: IUrl;
        details: IUrl;
        insert: IUrl;
        update: IUrl;
        delete: IUrl;
        import: IUrl;
        export: IUrl;
        exportIds: IUrl;
        template: IUrl;
        [key: string]: IUrl;
    }
}



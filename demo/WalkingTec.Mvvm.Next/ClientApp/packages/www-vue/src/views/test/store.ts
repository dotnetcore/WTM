import { EntitiesPageStore } from '@leng/public/src';
export class PageStore extends EntitiesPageStore {
    constructor() {
        super({
            target: '/api',
            Search: { url: '/_actionlog/Search', },
            Details: { url: '/_FrameworkUserBase/{id}' },
            Insert: { url: '/_FrameworkGroup/Add' },
            Update: { url: '/_FrameworkGroup/Edit' },
            Delete: { url: '/_FrameworkGroup/BatchDelete' },
            Export: { url: '/_FrameworkGroup/ExportExcel', body: {} },
        });
    }
}
export default PageStore
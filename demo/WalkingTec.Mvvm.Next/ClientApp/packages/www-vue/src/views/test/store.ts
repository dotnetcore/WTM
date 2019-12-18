import { EntitiesPageStore } from '@leng/public/src';
export class PageStore extends EntitiesPageStore {
    constructor() {
        super({
            Search: { url: '' },
            Insert: { url: '' },
            Update: { url: '' },
            Delete: { url: '' },
            Import: { url: '' },
            Export: { url: '' },
            ExportByIds: { url: '' },
            Template: { url: '' },
        })
    }
}
export default PageStore
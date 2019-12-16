import { EntitiesPageStore } from './index'
import { EntitiesUserStore } from '../user'
import { Request } from '../../utils/request'
import { toJS } from 'mobx'
Request.target = 'http://localhost:5555'
jest.useFakeTimers()
it('Page：', async () => {
    // 模拟登录  jwt 模式
    await new EntitiesUserStore().onLogin('admin', '000000', { body: { cookie: false } });

    const Store = new EntitiesPageStore({
        target: '/api',
        Search: { url: '/_FrameworkUserBase/Search', },
        Details: { url: '/_FrameworkUserBase/{id}' },
        Insert: { url: '/_FrameworkGroup/Add' },
        Update: { url: '/_FrameworkGroup/Edit' },
        Delete: { url: '/_FrameworkGroup/BatchDelete' },
        Export: { url: '/_FrameworkGroup/ExportExcel' },
    });
    // await Store.onSearch()
    // await Store.onDetails({ body: { id: '72b2b90d-a8ae-41a0-9f6b-171945c7dd3' } });
    // await Store.onInsert({
    //     body: {
    //         "Entity": {
    //             // "ID": "9c435300-b17b-4122-98dd-4297d3e4889c",
    //             "GroupCode": "123123",
    //             "GroupName": "bbacd",
    //             "GroupRemark": "bbbb"
    //         }
    //     }
    // });
    // await Store.onDelete({ body: ['9c435300-b17b-4122-98dd-4297d3e4889c'] })
    await Store.onExport({})
    // console.log("TCL: res", toJS(Store.Details))
});

import { EntitiesUserStore } from './index'
import { Request } from '../../utils/request'
Request.target = 'http://localhost:5555'
jest.useFakeTimers()
it('用户登录：', async () => {
    const Store = new EntitiesUserStore();
    await Store.onLogin('admin', '000000');
    console.log(JSON.stringify(Store, null, 4));
});

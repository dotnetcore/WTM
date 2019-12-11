import { EntitiesPageStore } from './index'
import { Request } from '../../utils/request'
Request.target = 'http://localhost:5555'
jest.useFakeTimers()
it('Page：', async () => {
    const Store = new EntitiesPageStore();
});

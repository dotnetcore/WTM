import { EntitiesUserStore } from './index'
jest.useFakeTimers()
it('test', async () => {
    const Store = new EntitiesUserStore();
    Store.onLogin('LENG', '000000');
    console.log(`'状态：${Store.OnlineState} 年龄：${Store.Age} 姓名：${Store.Name}`)
});

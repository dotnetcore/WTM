import { EntitiesUserStore } from './index'
jest.useFakeTimers()
it('用户登录：', async () => {
    const Store = new EntitiesUserStore();
    Store.Request.target = 'http://localhost:5555';
    await Store.onLogin('admin', '000000');
    console.log(`'状态：${Store.OnlineState} 年龄：${Store.Age} 姓名：${Store.Name}`)
});

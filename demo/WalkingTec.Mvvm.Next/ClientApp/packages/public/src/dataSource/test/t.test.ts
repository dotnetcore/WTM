import { testDataSource } from './index'
it('test', async () => {
    const test = new testDataSource();
    await test.onGetMock()
});

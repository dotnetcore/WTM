import request_new from './request'
it('test', async () => {
    const res = await request_new.cache({ url: "https://www.easy-mock.com/mock/5a9130e5a2f38c18c96bce97/example/mock" });
    const res2 = await request_new.cache({ url: "https://www.easy-mock.com/mock/5a9130e5a2f38c18c96bce97/example/mock" });
    console.log("TCL: res2", res2)
    console.log("TCL: res", res)
})
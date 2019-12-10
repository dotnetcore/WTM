import { Request } from '../../utils/request';
export class testDataSource {
    constructor(
        private request = new Request("https://www.easy-mock.com")
    ) { }
    /**
     * 请求接口
     */
    async  onGetMock() {
        try {
            const data = await this.request.ajax({
                url: '/mock/5a9130e5a2f38c18c96bce97/example/mock',
                // // method: "POST",
                // headers: {
                //     credentials: 'include',
                //     accept: "*/*",
                //     "Content-Type": "application/json",
                // }
            }).toPromise();
            // console.log("TCL: EntitiesTimeStore -> onTest -> data", data)
            return data;
        } catch (error) {
            console.error(error)
        }
    }
}


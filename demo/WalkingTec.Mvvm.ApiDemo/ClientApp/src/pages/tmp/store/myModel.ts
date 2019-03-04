import { observable, computed } from 'mobx';
import Request from 'utils/Request';
import { message } from 'antd';
export class Store {
    constructor() {
    }
    @observable private __visible = false;
    @computed
    public get visible() {
        return this.__visible;
    }
    public set visible(value) {
        this.__visible = value;
    }
    async onSubmit(params) {
        // const res = await Request.post("", params).toPromise()
        message.success("提交数据")
    }
}
export default new Store();

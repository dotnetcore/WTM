import StoreBasice from 'store/table';
import { message } from 'antd';
import { action, computed, observable, runInAction } from 'mobx';
export class Store extends StoreBasice {
    constructor() {
        super();
    }
    /** 数据 ID 索引 */
    IdKey = 'id';
    Urls = {
        search: {
            src: "/SampleData/WeatherForecasts",
            method: "get"
        },
        details: {
            src: "/test/details/{id}",
            method: "get"
        },
        insert: {
            src: "/test/insert",
            method: "post"
        },
        update: {
            src: "/test/update",
            method: "post"
        },
        delete: {
            src: "/test/delete",
            method: "post"
        },
        import: {
            src: "/test/import",
            method: "post"
        },
        export: {
            src: "/test/export",
            method: "post"
        },
        template: {
            src: "/test/template",
            method: "post"
        }
    }

    /**
     * 加载数据 列表
     * @param params 搜索参数
     */
    async onSearch(params: any = {}, page: any = { pageNo: 1, pageSize: 10 }) {
        console.log(this);
        if (this.pageState.loading == true) {
            return message.warn('数据正在加载中')
        }
        this.onPageState("loading", true);
        this.searchParams = { ...this.searchParams, ...params };
        params = {
            ...page,
            search: this.searchParams
        }
        const method = this.Urls.search.method;
        const src = this.Urls.search.src;
        const res = await this.Request[method](src, params).map(data => {
            const newData = {
                list: []
            }
            // if (data.list) {
            //     data.list = data.list.map((x, i) => {
            //         // antd table 列表属性需要一个唯一key
            //         return { key: i, ...x }
            //     })
            // }
            newData.list = data.map((x, i) => {
                // antd table 列表属性需要一个唯一key
                return { key: i, ...x }
            })
            return newData
        }).toPromise()
        runInAction(() => {
            this.dataSource = res || this.dataSource
            this.onPageState("loading", false)
        })
        return res
    }
}
export default new Store();
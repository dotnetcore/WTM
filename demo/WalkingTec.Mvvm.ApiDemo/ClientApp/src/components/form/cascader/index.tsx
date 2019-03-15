/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-02-24 17:06:42
 * @modify date 2019-02-24 17:06:42
 * @desc [description]
 */
import { Cascader } from 'antd';
import { DesError } from 'components/decorators'; //错误
import lodash from 'lodash';
import React from 'react';
import { Observable } from 'rxjs';
import { CascaderProps } from 'antd/lib/cascader';
import Request from 'utils/Request';

interface IAppProps {
    dataSource: Observable<any[]> | any[] | Promise<any[]>;
    value?: any;
    disabled?: boolean;
    placeholder?: string;
    CascaderProps?: CascaderProps;
    display?: boolean;
    /** 最后一级数据 */
    lastData?: boolean;
    [key: string]: any;
}
@DesError
export class WtmCascader extends React.Component<IAppProps, any> {
    static wtmType = "Cascader";
    key = "Value";
    title = "Text";
    children = "Children";
    childrenUrl = "ChildrenUrl";
    // description = "Text";
    state = {
        loading: true,
        mockData: [],
        targetKeys: [],
    }
    // shouldComponentUpdate(nextProps, nextState, nextContext) {
    //     if (lodash.isEqual(this.state, nextState)) {
    //         return true
    //     }
    //     return !lodash.isEqual(this.props.value, nextProps.value)
    // }
    Unmount = false
    componentWillUnmount() {
        this.Unmount = true;
    }
    async  componentDidMount() {
        const { dataSource } = this.props;
        let mockData = [],
            targetKeys = [],
            res = [];
        try {
            // 值为 数组 
            if (lodash.isArray(dataSource)) {
                res = dataSource;
            }
            // 值为 Promise
            else if (dataSource instanceof Promise) {
                res = await dataSource;
            }
            // 值为 Observable 
            else if (dataSource instanceof Observable) {
                res = await dataSource.toPromise();
            }
            // 转换 数据 为 渲染 格式
            mockData = res;
        } catch (error) {
            console.error("获取数据出错", error)
        }
        if (this.Unmount) return;
        this.formatDataSource(mockData)
    }
    /**
     * 格式化 插入 数据
     * @param mockData 
     */
    formatDataSource(mockData: any[]) {
        try {
            this.setState({
                mockData: mockData.map(this.mapData.bind(this)),
                loading: false
            })
        } catch (error) {
            console.error("获取数据出错", error)
        }
    }
    mapData(item) {
        let children: any[] = lodash.get(item, this.children);
        const childrenUrl = lodash.get(item, this.childrenUrl);
        const newItem = {
            ...item,
            value: lodash.toString(lodash.get(item, this.key)),
            label: lodash.get(item, this.title),
            childrenUrl,
            // isLeaf: false,
            // children: children && children.map(mapData)
        }
        // 远程 路径 
        if (childrenUrl) {
            lodash.set(newItem, 'isLeaf', false);
        }
        if (children && children.length > 0) {
            lodash.set(newItem, 'children', children.map(this.mapData.bind(this)));
        }
        return newItem
    }
    // filterOption = (inputValue, option) => option.description.indexOf(inputValue) > -1
    handleChange = (targetKeys) => {
        // 多选 返回 值 为数组 的情况下 有 dataKey 重组 数据
        if (this.props.dataKey && lodash.isArray(targetKeys)) {
            targetKeys = targetKeys.map(x => (
                { [this.props.dataKey]: x }
            ))
        }
        if (this.props.lastData) {
            targetKeys = lodash.last(targetKeys);
        }
        this.props.onChange && this.props.onChange(targetKeys);
    }
    loadData = async (selectedOptions) => {
        const targetOption = lodash.head(selectedOptions) as any;
        const childrenUrl = lodash.get(targetOption, this.childrenUrl)
        if (childrenUrl) {
            targetOption.loading = true;
            const time = Date.now();
            const children: any[] = await Request.cache({ url: childrenUrl });
            // 强制 执行加载最少 400 毫秒
            await new Promise((res, rej) => {
                lodash.delay(res, 400 - (Date.now() - time))
            });
            targetOption.loading = false;
            lodash.set(targetOption, 'children', children.map(this.mapData.bind(this)));
            if (this.Unmount) return;
            this.setState({
                mockData: [...this.state.mockData],
            });
        }
        // load options lazily
        // setTimeout(() => {
        //     targetOption.loading = false;
        //     targetOption.children = [{
        //         label: `${targetOption.label} Dynamic 1`,
        //         value: 'dynamic1',
        //     }, {
        //         label: `${targetOption.label} Dynamic 2`,
        //         value: 'dynamic2',
        //     }];
        //     this.setState({
        //         mockData: [...this.state.mockData],
        //     });
        // }, 1000);
    }
    render() {
        let config: CascaderProps = {
            placeholder: this.props.placeholder,
            options: this.state.mockData,
            allowClear: true,
            loadData: this.loadData,
            changeOnSelect: true,
            disabled: this.props.disabled,
            onChange: this.handleChange
        }
        // if (this.props.display) {
        //     if (!this.state.loading) {
        //         // 多选的
        //         if (lodash.isArray(config.defaultValue)) {
        //             return lodash.intersectionBy(this.state.mockData, (config.defaultValue as string[]).map(x => ({ key: x })), "key").map(x => x.title).join(",")
        //         }
        //         return <span>{lodash.get(lodash.find(this.state.mockData, ["key", lodash.toString(config.defaultValue)]), "title")}</span>
        //     }
        //     return <span></span>
        // }
        return (
            <Cascader {...config}></Cascader>
        );
    }

}

export default WtmCascader
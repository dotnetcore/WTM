/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-02-24 17:06:42
 * @modify date 2019-02-24 17:06:42
 * @desc [description]
 */
import { Cascader, Spin } from 'antd';
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
    ParentIdKey = "ParentId";
    children = "Children";
    childrenUrl = "ChildrenUrl";
    // description = "Text";
    state = {
        loading: true,
        mockData: [],
        originalData: [] // 原始数据
    }
    /**
      * 优化渲染
      * @param nextProps 
      * @param nextState 
      * @param nextContext 
      */
    shouldComponentUpdate(nextProps, nextState, nextContext) {
        if (!lodash.eq(this.state.loading, nextState.loading)) {
            return true
        }
        if (!lodash.eq(this.state.originalData, nextState.originalData)) {
            return true
        }
        if (!lodash.eq(this.props.value, nextProps.value)) {
            return true
        }
        return false
    }
    Unmount = false
    componentWillUnmount() {
        this.Unmount = true;
    }
    async  componentDidMount() {
        const { dataSource } = this.props;
        let mockData = [],
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
            mockData = this.recursionTree(res, null);
        } catch (error) {
            console.error("获取数据出错", error)
        }
        if (this.Unmount) return;
        this.setState({
            mockData,
            originalData: res,
            loading: false
        })
        // this.formatDataSource(mockData)
    }
    /**
     * 递归 格式化 树
     * @param datalist 
     * @param ParentId 
     * @param children 
     */
    recursionTree(datalist, ParentId, children = []) {
        lodash.filter(datalist, [this.ParentIdKey, ParentId]).map(data => {
            data = {
                ...data,
                value: lodash.toString(lodash.get(data, this.key)),
                label: lodash.get(data, this.title),
            }
            children.push(data);
            data.children = this.recursionTree(datalist, data.Value, data.children || [])
        });
        return children;
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
        if (lodash.get(this.props, 'lastData', true)) {
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
    // 查找父级
    getParent(value, ids = []) {
        const key = lodash.get(lodash.find(this.state.originalData, [this.key, value]), this.ParentIdKey);
        if (key) {
            this.getParent(key, ids)
            ids.push(key);
        }
        return ids;
    }
    // 获取默认值
    getDefaultValue(value) {
        if (value && !this.state.loading) {
            const defaultValue = this.getParent(value);
            defaultValue.push(value);
            if (defaultValue.length > 0) {
                return {
                    value: defaultValue
                }
            }
        }
        return {
        }
    }
    renderDisplay(config: CascaderProps) {
        let str = ''
        if (!this.state.loading && lodash.isArray(config.value)) {
            str = config.value.map(value => {
                return lodash.get(lodash.find(this.state.originalData, [this.key, value]), this.title)
            }).join(" / ")
        }
        return <span>{str}</span>
    }
    render() {
        let config: CascaderProps = {
            placeholder: this.props.placeholder,
            options: this.state.mockData,
            allowClear: true,
            loadData: this.loadData,
            changeOnSelect: true,
            disabled: this.props.disabled,
            onChange: this.handleChange,
            ...this.getDefaultValue(this.props.value)
            // defaultValue: this.props.value
        }
        return (
            <Spin spinning={this.state.loading}>
                {this.props.display ? this.renderDisplay(config) : <Cascader {...config}></Cascader>}
            </Spin>
        );
    }

}

export default WtmCascader
/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-02-24 17:05:58
 * @modify date 2019-02-24 17:05:58
 * @desc [description]
 */
import { Button, Col, Divider, Form, Icon, Row, Spin } from 'antd';
import { WrappedFormUtils } from 'antd/lib/form/Form';
import { DesError } from 'components/decorators';
import GlobalConfig from 'global.config';
import lodash from 'lodash';
import { action, observable, runInAction } from 'mobx';
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from 'store/dataSource';
import { Debounce, BindAll } from 'lodash-decorators';

import "./style.less";
interface IAppProps {
    /** 状态 */
    Store: Store;
    /** 表单 */
    form: WrappedFormUtils;
    /** 列数 */
    columnCount?: number;
    /** 提交方法 */
    onSubmit?: (event?) => void;
    /** 重置方法 */
    onReset?: (event?) => void;
    [key: string]: any;
}
/**
 * 搜索标题组件 
 * 
 * 不要直接修改 wtm 组件 使用继承重写的方式修改
 */
@DesError
@observer
@BindAll()
export class DataViewSearch extends React.Component<IAppProps, any> {
    Store: Store = this.props.Store;
    @observable toggle = false;
    columnCount = GlobalConfig.searchColumnCount || 3;
    @observable key = Date.now();
    /**
     * 提交表单
     * @param e 
     */
    // @Debounce(500)
    onSubmit(e) {
        if (this.props.onSubmit) {
            return this.props.onSubmit(e)
        }
        e.preventDefault();
        e.stopPropagation();
        this.props.form.validateFields((err, values) => {
            console.log("搜索参数", values);
            if (!err) {
                this.Store.onSearch(values)
            }
        });
    }
    /**
     * 重置表单
     * @param e 
     */
    // @Debounce(500)
    onReset(e) {
        if (this.props.onReset) {
            return this.props.onReset(e)
        }
        e.preventDefault();
        e.stopPropagation();
        const { resetFields } = this.props.form;
        resetFields();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                this.Store.onSearch(lodash.mapValues(values, x => undefined))
                runInAction(() => { this.key = Date.now() })
            }
        });
    }
    // @Debounce(500)
    @action
    onToggle() {
        this.toggle = !this.toggle;
        document.body.style.overflowY = "hidden";
        lodash.defer(() => {
            // 主动触发 浏览器 resize 事件
            dispatchEvent(new CustomEvent('resize'));
            lodash.delay(() => document.body.style.overflowY = "", 500)
        })
    }
    render() {
        let items = [],
            columnCount = this.props.columnCount || this.columnCount,
            childrens = React.Children.toArray(this.props.children),
            toggleShow = childrens.length >= columnCount,
            colSpan = 24 / columnCount,//每列 值
            colSpanSearch = colSpan;
        // debugger
        // 展开收起
        if (this.toggle) {
            items = childrens;
        } else {
            items = [...childrens].splice(0, columnCount - 1);
        }
        const itemslength = items.length
        // 列行数
        if (itemslength === columnCount) {
            colSpanSearch = 24;
        } else if (itemslength > columnCount) {
            colSpanSearch = (columnCount - (itemslength % columnCount)) * colSpan
        } else {
            colSpanSearch = (columnCount - itemslength) * colSpan
        }
        const { PageState } = this.Store;
        return (
            <>
                <DataSpin Store={this.Store} />
                {items.length > 0 && <Form className="data-view-search" onSubmit={this.onSubmit}>
                    {/* <Spin spinning={PageState.tableLoading}> */}
                    <Row type="flex" >
                        {items.map(x => <Col key={`${this.key}_${x.key}`} span={colSpan}>{x}</Col>)}
                        <Col span={colSpanSearch} className="data-view-search-right" >
                            <Button icon="search" type="primary" htmlType="submit" >搜索</Button>
                            <Divider type="vertical" />
                            <Button icon="retweet" onClick={this.onReset} >重置</Button>
                            {
                                toggleShow && <>
                                    <Divider type="vertical" />
                                    <a className="data-view-search-toggle" onClick={this.onToggle}>
                                        {this.toggle ? <>收起 <Icon type='up' /></> : <>展开 <Icon type='down' /></>}
                                    </a>
                                </>
                            }
                        </Col>
                    </Row>
                    {/* </Spin> */}
                    <div className="data-view-search-divider"></div>
                </Form>}
            </>
        );
    }
}
@observer
class DataSpin extends React.Component<any, any> {
    Store: Store = this.props.Store;
    public render() {
        const { PageState } = this.Store;
        return (
            <Spin spinning={PageState.tableLoading} className="data-view-search-spin" size="large" />
        );
    }
}

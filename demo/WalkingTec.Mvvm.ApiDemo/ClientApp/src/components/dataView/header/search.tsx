/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-02-24 17:05:58
 * @modify date 2019-02-24 17:05:58
 * @desc [description]
 */
import { Button, Col, Divider, Form, Icon, Row } from 'antd';
import { WrappedFormUtils } from 'antd/lib/form/Form';
import { DesError } from 'components/decorators';
import GlobalConfig from 'global.config';
import lodash from 'lodash';
import { action, observable, runInAction } from 'mobx';
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from 'store/dataSource';
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
export class DataViewSearch extends React.Component<IAppProps, any> {
    Store: Store = this.props.Store;
    @observable toggle = false;
    columnCount = GlobalConfig.searchColumnCount || 3;
    @observable key = Date.now();
    /**
     * 提交表单
     * @param e 
     */
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
    @action.bound
    onToggle() {
        this.toggle = !this.toggle
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
        return (
            <Form className="data-view-search" onSubmit={this.onSubmit.bind(this)}>
                <Row type="flex" >
                    {items.map(x => <Col key={`${this.key}_${x.key}`} span={colSpan}>{x}</Col>)}
                    <Col span={colSpanSearch} className="data-view-search-right" >
                        <Button icon="search" type="primary" htmlType="submit" loading={this.Store.pageState.loading}>搜索</Button>
                        <Divider type="vertical" />
                        <Button icon="retweet" onClick={this.onReset.bind(this)} loading={this.Store.pageState.loading}>重置</Button>
                        {
                            toggleShow && <>
                                <Divider type="vertical" />
                                <a className="data-view-search-toggle" onClick={this.onToggle}>
                                    {this.toggle ? <>收起 <Icon type='down' /></> : <>展开 <Icon type='up' /></>}
                                </a>
                            </>
                        }
                    </Col>
                </Row>
                <div className="data-view-search-divider"></div>
            </Form>
        );
    }
}
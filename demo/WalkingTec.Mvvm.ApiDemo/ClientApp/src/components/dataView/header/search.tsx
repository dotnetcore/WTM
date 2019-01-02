import { Button, Col, Divider, Form, Row, Select, Spin, Drawer, Checkbox, List, Icon } from 'antd';
import Store from 'store/dataSource';
import * as React from 'react';
import lodash from 'lodash';
import moment from 'moment';
import { observer } from 'mobx-react';
import { observable, action } from 'mobx';
import decoForm from 'components/decorators/form';
import { WrappedFormUtils } from 'antd/lib/form/Form';
import "./style.less"
/**
 * 装饰器
 * @param params 
 */
export function DecoratorsSearch(params: {
    /** 状态 */
    Store: Store,
    /** 搜索表单 items */
    FormItems: (props: WrappedFormUtils) => React.ReactNodeArray
}) {
    return function (Component: any) {
        return class extends React.Component<any, any> {
            render() {
                return (
                    <SearchComponent {...params} {...this.props}>
                        <Component Store={params.Store} {...this.props} />
                    </SearchComponent>
                ) as any
            }
        } as any
    }
}
/**
 * 搜索标题组件 
 * 
 * 不要直接修改 wtm 组件 使用继承重写的方式修改
 */
@decoForm
@observer
export default class SearchComponent extends React.Component<any, any> {
    Store: Store = this.props.Store;
    @observable toggle = false;
    onSubmit(e) {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                // 转换时间对象  moment 对象 valueOf 为时间戳，其他类型数据 为原始数据。
                // values = mapValues(values, this.Store.Format.date)
                console.log(values);
                this.Store.onSearch(values)
            }
        });
    }
    onReset() {
        const { resetFields } = this.props.form;
        resetFields();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                this.Store.onSearch(lodash.mapValues(values, x => undefined))
            }
        });
    }
    @action.bound
    onToggle() {
        this.toggle = !this.toggle
    }
    FormItems = this.props.FormItems(this.props.form)
    render() {
        let items = null
        const FormItems = this.props.FormItems(this.props.form)
        if (Array.isArray(FormItems)) {
            if (this.toggle) {
                items = FormItems;
            } else {
                items = [...FormItems].splice(0, 4);
            }
        }
        const toggleShow = FormItems.length >= 4;
        return (
            <Form className="data-view-search" onSubmit={this.onSubmit.bind(this)}>
                <Row type="flex" gutter={16}>
                    {items}
                </Row>
                <Row type="flex" gutter={16} justify="end">
                    <Col span={16} className="data-view-search-left" >
                        {this.props.children}
                    </Col>
                    <Col span={8} className="data-view-search-right" >
                        <Button icon="retweet" onClick={this.onReset.bind(this)} loading={this.Store.pageState.loading}>重置</Button>
                        <Divider type="vertical" />
                        <Button icon="search" htmlType="submit" loading={this.Store.pageState.loading}>搜索</Button>
                        <Divider type="vertical" />
                        {
                            toggleShow ? <a onClick={this.onToggle}>
                                {this.toggle ? <>收起 <Icon type='down' /></> : <>展开 <Icon type='up' /></>}
                            </a> : null
                        }

                    </Col>
                </Row>
            </Form>
        );
    }
}
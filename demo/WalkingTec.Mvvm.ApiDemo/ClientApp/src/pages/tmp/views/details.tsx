import { Form, Col } from 'antd';
import { InfoShell, InfoShellCol, InfoShellFooter, ToImg, toValues } from 'components/dataView';
import { DesError, DesForm } from 'components/decorators'; //错误
import GlobalConfig from 'global.config'; //全局配置
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from '../store'; //页面状态
import { FormItem } from './models'; //模型
const formItemLayout = { ...GlobalConfig.formItemLayout };//布局
const formItemLayoutRow = { ...GlobalConfig.formItemLayoutRow };
/**
 *  详情 窗口 
 *  根据 类型 显示不同的 窗口
 */
@DesError
@observer
export default class extends React.Component<any, any> {
    /**
     * 根据状态类型 渲染  添加。修改，详情信息
     * @param detailsType 
     */
    renderBody(detailsType) {
        switch (detailsType) {
            case 'Insert':
                //添加
                return <InsertForm {...this.props} />
                break;
            case 'Update':
                // 修改
                return <UpdateForm {...this.props} />
                break;
            default:
                // 详情
                return <InfoForm {...this.props} />
                break;
        }
    }
    render() {
        const enums = {
            Insert: "新建",
            Update: "修改",
            Info: "详情"
        };
        const { detailsType, visibleEdit, loadingEdit } = Store.pageState
        return <InfoShell
            title={enums[detailsType]}
            onClose={() => { Store.onPageState("visibleEdit", false) }}
            visible={visibleEdit}
        >
            {this.renderBody(detailsType)}
        </InfoShell>
    }
}
/**
 * 添加表单
 */
@DesError
@DesForm
@observer
class InsertForm extends React.Component<any, any> {
    onSubmit(e) {
        e.stopPropagation();
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            console.log("数据", values);
            if (!err) {
                Store.onEdit(values);
            }
        });
    }
    render() {
        return <Form onSubmit={this.onSubmit.bind(this)}>
            <FooterFormItem submit>
                <FormItem {...this.props} fieId="ITCode" />
                <FormItem {...this.props} fieId="Password" />
                <FormItem {...this.props} fieId="Email" />
                <FormItem {...this.props} fieId="Name" />
                <FormItem {...this.props} fieId="Sex" />
                <InfoShellCol span={24}>
                    <FormItem {...this.props} fieId="PhotoId" formItemProps={{ ...formItemLayoutRow }} />
                </InfoShellCol>
            </FooterFormItem>

        </Form>
    }
}
/**
 * 修改表单
 */
@DesError
@DesForm
@observer
class UpdateForm extends React.Component<any, any> {
    onSubmit(e) {
        e.stopPropagation();
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            console.log("数据", values);
            if (!err) {
                // values = mapValues(values, "YYYY-MM-DD")
                Store.onEdit(values);
            }
        });
    }
    render() {
        return <Form onSubmit={this.onSubmit.bind(this)}>
            <FooterFormItem submit>
                <FormItem {...this.props} fieId="ITCode" defaultValue />
                <FormItem {...this.props} fieId="Password" defaultValue />
                <FormItem {...this.props} fieId="Email" defaultValue />
                <FormItem {...this.props} fieId="Name" defaultValue />
                <FormItem {...this.props} fieId="Sex" defaultValue />
                <Col span={24}>
                    <FormItem {...this.props} fieId="PhotoId" defaultValue formItemProps={{ ...formItemLayoutRow }} />
                </Col>
            </FooterFormItem>
        </Form>
    }
}
/**
 * 详情
 */
@DesError
@observer
class InfoForm extends React.Component<any, any> {
    render() {
        const details = { ...Store.details };
        return <Form >
            <FooterFormItem>
                <Form.Item label="账号" {...formItemLayout}>
                    <span>{toValues(details['ITCode'], "span")}</span>
                </Form.Item >
                <Form.Item label="邮箱" {...formItemLayout}>
                    <span>{toValues(details['Email'], "span")}</span>
                </Form.Item >
                <Form.Item label="姓名" {...formItemLayout}>
                    <span>{toValues(details['Name'], "span")}</span>
                </Form.Item >
                <Col span={24}>
                    <Form.Item label="头像" {...formItemLayoutRow}>
                        <span>
                            <ToImg fileID={details['PhotoId']} />
                        </span>
                    </Form.Item >
                </Col>
            </FooterFormItem>
        </Form>
    }
}
/**
 * Items 外壳
 */
@DesError
@observer
class FooterFormItem extends React.Component<{ submit?: boolean }, any> {
    render() {
        const { loadingEdit } = Store.pageState;
        return <InfoShellFooter
            submit={this.props.submit}
            loadingEdit={loadingEdit}
            onCancel={() => Store.onPageState("visibleEdit", false)}>
            {this.props.children}
        </InfoShellFooter>
    }
}
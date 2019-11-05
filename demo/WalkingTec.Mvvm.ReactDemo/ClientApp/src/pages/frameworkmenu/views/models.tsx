import { Input, Switch, Icon, Select, Upload, message, Modal, InputNumber, Row, Col, Checkbox } from 'antd';
import { WtmCascader, WtmCheckbox, WtmDatePicker, WtmEditor, WtmRadio, WtmSelect, WtmTransfer, WtmUploadImg, WtmUpload } from 'components/form';
import { FormItem } from 'components/dataView';
import * as React from 'react';
import lodash from 'lodash';
import { Observable } from 'rxjs';
import Regular from 'utils/Regular';
import AntIcons from "@ant-design/icons/lib/manifest";
import Fonts from "assets/font/font";
import Request from 'utils/Request';
import { BindAll } from 'lodash-decorators';
Fonts.unshift({
    name: "Antd",
    class: "Antd",
    icons: AntIcons.fill,
})
/**
 * label  标识
 * rules   校验规则，参考下方文档  https://ant.design/components/form-cn/#components-form-demo-validate-other
 * formItem  表单组件
 */
const pages = new Observable<{ Text: string, Value: string, Url: string }[]>((sub) => {
    import("pages/index").then(pages => {
        const PagesList = [];
        lodash.map(pages.default, (item) => {
            if (item.controller) {
                PagesList.push({ Text: item.name, Value: item.controller, Url: item.path })
            }
        })
        sub.next(PagesList);
        sub.complete();
    })

})
export default {
    /**
     * 表单模型 
     * @param props 
     */
    editModels(props?): WTM.FormItem {
        return {
            /** 页面名称 */
            "Entity.PageName": {
                label: "页面名称",
                rules: [{ "required": true, "message": "页面名称不能为空" }],
                formItem: <Input placeholder="请输入 页面名称" />
            },
            /** 模块名称 */
            "SelectedModule": {
                label: "模块名称",
                rules: [],
                formItem: <WtmSelect placeholder="选择模块"
                    dataSource={pages}
                    onChange={(value, option) => {
                        pages.subscribe(data => {
                            props.form.setFieldsValue({
                                'Entity.Url': lodash.get(lodash.find(data, ['Value', value]), "Url")
                            })
                        })
                    }}
                />
            },
            /** 动作名称 */
            "SelectedActionIDs": {
                label: "动作名称",
                rules: [],
                formItem: <WtmSelect placeholder="选择动作"
                    mode="multiple"
                    linkage={["SelectedModule"]}
                    dataSource={(props) => {
                        const ModelName = lodash.get(props, 'SelectedModule');
                        return ModelName && Request.cache({
                            url: "/api/_FrameworkMenu/GetActionsByModel", body: { ModelName }
                        })
                    }}
                />
            },
            /** 目录 */
            "Entity.FolderOnly": {
                label: "目录",
                rules: [],
                formItem: <Switch checkedChildren={<Icon type="check" />} unCheckedChildren={<Icon type="close" />} />
            },
            /** 继承 */
            "Entity.IsInherit": {
                label: "继承",
                rules: [],
                formItem: <Switch checkedChildren={<Icon type="check" />} unCheckedChildren={<Icon type="close" />} />
            },
            /** 菜单显示 */
            "Entity.ShowOnMenu": {
                label: "菜单显示",
                rules: [],
                formItem: <Switch checkedChildren={<Icon type="check" />} unCheckedChildren={<Icon type="close" />} />
            },
            /** 公开 */
            "Entity.IsPublic": {
                label: "公开",
                rules: [],
                formItem: <Switch checkedChildren={<Icon type="check" />} unCheckedChildren={<Icon type="close" />} />
            },
            /** 顺序 */
            "Entity.DisplayOrder": {
                label: "顺序",
                rules: [{ "required": true, "message": "顺序不能为空" }],
                formItem: <InputNumber placeholder="请输入 顺序" />
            },
            /** 内部地址 */
            "Entity.IsInside": {
                label: "地址类型",
                rules: [{ "required": true, "message": "内部地址不能为空" }],
                formItem: <WtmRadio
                    dataSource={[
                        { Text: "内部地址", Value: true },
                        { Text: "外部地址", Value: false },
                    ]} />
            },
            /** Url */
            "Entity.Url": {
                label: "Url",
                rules: [],
                formItem: <Input placeholder="请输入 Url" />
            },
            "Entity.ICon": {
                label: "图标",
                rules: [],
                formItem: <IConId />
            },
            /** 父目录 */
            "Entity.ParentId": {
                label: "父目录",
                rules: [],
                formItem: <WtmSelect placeholder="父目录"
                    dataSource={Request.cache({ url: "/api/_FrameworkMenu/GetFolders" })}
                />
            }
        }
    },
    /**
     * 搜索 模型 
     * @param props 
     */
    searchModels(props?): WTM.FormItem {
        return {

        }
    },
    /**
     * 渲染 模型
     */
    renderModels(props?) {
        return lodash.map(props.models, (value, key) => {
            return <FormItem {...props} fieId={key} key={key} />
        })
    },

    getValue(props: WTM.FormProps, fieId, defaultvalue = undefined) {
        var rv = lodash.toString(props.form.getFieldValue(fieId) || lodash.get(props.defaultValues, fieId));
        console.log("rv=" + rv);
        if (rv == "") {
            rv = lodash.toString(defaultvalue);
        }
        return rv;
    }
}
@BindAll()
class IConId extends React.Component<any, any> {
    state = {
        // 自定义
        // custom: false,
        iconType: 'Antd',
        iconItems: this.onGetItems('Antd')
    }
    onGetItems(iconType): string[] {
        return lodash.get(lodash.find(Fonts, ['class', iconType]), 'icons', [])
    }
    onChange(event) {
        this.props.onChange(event);
    }
    onChangeType(event) {
        this.setState({ iconType: event, iconItems: this.onGetItems(event) });
        this.onChange(undefined)
    }
    onSearch() {
    }
    componentDidMount() {
    }
    UNSAFE_componentWillMount() {
        if (this.props.value) {
            // 某人 懒得加字段。就自己用一个字段截取吧
            const fontClass = lodash.trim(this.props.value).split(' ');
            // 自定义
            if (fontClass.length > 1) {
                const iconType = fontClass[0];
                this.setState({ iconType: iconType, iconItems: this.onGetItems(iconType) })
            }
        }
    }
    onGetIcon(icon) {
        // 自定义 图标 需要 使用 name 和 class名称拼接，并且前面需要一个空格
        return this.state.iconType === "Antd" ? icon : ` ${this.state.iconType} ${icon}`;
    }
    render() {
        return (
            <Row type="flex">
                <Col span={24}>
                    <Select
                        showSearch
                        style={{ width: '100%' }}
                        placeholder="Icon Type"
                        onChange={this.onChangeType}
                        value={this.state.iconType}
                        allowClear
                        // onSearch={this.onSearch}
                        filterOption={(input, option: any) => option.props.value && option.props.value.toLowerCase().indexOf(input && input.toLowerCase()) >= 0}
                    >
                        {Fonts.map(data => {
                            return <Select.Option key={data.name} value={data.class}><span>{data.name}</span></Select.Option>
                        })}
                    </Select>
                </Col>
                <Col span={24}>
                    <Select
                        showSearch
                        style={{ width: '100%' }}
                        placeholder="Icon"
                        onChange={this.onChange}
                        value={this.props.value || undefined}
                        allowClear
                        // onSearch={this.onSearch}
                        filterOption={(input, option: any) => option.props.value && option.props.value.toLowerCase().indexOf(input && input.toLowerCase()) >= 0}
                    >
                        {this.state.iconItems.map(data => {
                            const key = this.onGetIcon(data);
                            return <Select.Option key={key} value={key}><Icon type={key} />：<span>{data}</span></Select.Option>
                        })}
                    </Select>
                </Col>
            </Row>
        );
    }
}

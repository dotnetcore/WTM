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
import { mergeLocales, getLocalesValue, getLocalesTemplate } from 'locale';
import { FormattedMessage } from 'react-intl';
Fonts.unshift({
    name: "Antd",
    class: "Antd",
    icons: AntIcons.fill,
})

mergeLocales({
    "zh-CN": {
        'frameworkmenu.PageName': '页面名称',
        'frameworkmenu.SelectedModule': '模块名称',
        'frameworkmenu.SelectedActionIDs': '动作名称',
        'frameworkmenu.FolderOnly': '目录',
        'frameworkmenu.ShowOnMenu': '菜单显示',
        'frameworkmenu.IsPublic': '公开',
        'frameworkmenu.DisplayOrder': '顺序',
        'frameworkmenu.IsInside': '地址类型',
        'frameworkmenu.IsInside.0': '内部地址',
        'frameworkmenu.IsInside.1': '外部地址',
        'frameworkmenu.Url': 'Url',
        'frameworkmenu.ICon': '图标',
        'frameworkmenu.ParentId': '父目录',
    },
    "en-US": {
        'frameworkmenu.PageName': 'PageName',
        'frameworkmenu.SelectedModule': 'Module',
        'frameworkmenu.SelectedActionIDs': 'Actions',
        'frameworkmenu.FolderOnly': 'Folder',
        'frameworkmenu.ShowOnMenu': 'ShowOnMenu',
        'frameworkmenu.IsPublic': 'IsPublic',
        'frameworkmenu.DisplayOrder': 'DisplayOrder',
        'frameworkmenu.IsInside': 'UrlType',
        'frameworkmenu.IsInside.0': 'Inside Url',
        'frameworkmenu.IsInside.1': 'Ourside Url',
        'frameworkmenu.Url': 'Url',
        'frameworkmenu.ICon': 'Icon',
        'frameworkmenu.ParentId': 'ParentFolder',
    }
});

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
                PagesList.push({ Text: getLocalesValue(item.name), Value: item.controller, Url: item.path })
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
                label: <FormattedMessage id='frameworkmenu.PageName' />,
                rules: [{
                    "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('frameworkmenu.PageName') }} />
                }],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('frameworkmenu.PageName') })} />
            },
            /** 模块名称 */
            "SelectedModule": {
                label: <FormattedMessage id='frameworkmenu.SelectedModule' />,
                rules: [],
                formItem: <WtmSelect placeholder={getLocalesTemplate('tips.placeholder.choose', { txt: getLocalesValue('frameworkmenu.SelectedModule') })}
                    dataSource={pages}
                    onChange={(value, option) => {
                        pages.subscribe(data => {
                            props.form.setFieldsValue({
                                'Entity.Url': lodash.get(lodash.find(data, ['Value', value]), "Url"),
                                'SelectedActionIDs': undefined
                            })
                        })
                    }}
                />
            },
            /** 动作名称 */
            "SelectedActionIDs": {
                label: <FormattedMessage id='frameworkmenu.SelectedActionIDs' />,
                rules: [],
                formItem: <WtmSelect placeholder={getLocalesTemplate('tips.placeholder.choose', { txt: getLocalesValue('frameworkmenu.SelectedActionIDs') })}
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
                label: <FormattedMessage id='frameworkmenu.FolderOnly' />,
                rules: [],
                formItem: <Switch checkedChildren={<Icon type="check" />} unCheckedChildren={<Icon type="close" />} />
            },
            /** 菜单显示 */
            "Entity.ShowOnMenu": {
                label: <FormattedMessage id='frameworkmenu.ShowOnMenu' />,
                rules: [],
                formItem: <Switch checkedChildren={<Icon type="check" />} unCheckedChildren={<Icon type="close" />} />
            },
            /** 公开 */
            "Entity.IsPublic": {
                label: <FormattedMessage id='frameworkmenu.IsPublic' />,
                rules: [],
                formItem: <Switch checkedChildren={<Icon type="check" />} unCheckedChildren={<Icon type="close" />} />
            },
            /** 顺序 */
            "Entity.DisplayOrder": {
                label: <FormattedMessage id='frameworkmenu.DisplayOrder' />,
                rules: [{
                    "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('frameworkmenu.DisplayOrder') }} />
                }],
                formItem: <InputNumber placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('frameworkmenu.DisplayOrder') })} />
            },
            /** 内部地址 */
            "Entity.IsInside": {
                label: <FormattedMessage id='frameworkmenu.IsInside' />,
                rules: [{
                    "required": true, "message": <FormattedMessage id='tips.error.required' values={{ txt: getLocalesValue('frameworkmenu.IsInside') }} />
                }],
                formItem: <WtmRadio
                    dataSource={[
                        { Text: <FormattedMessage id='frameworkmenu.IsInside.0' />, Value: true },
                        { Text: <FormattedMessage id='frameworkmenu.IsInside.1' />, Value: false },
                    ]} />
            },
            /** Url */
            "Entity.Url": {
                label: <FormattedMessage id='frameworkmenu.Url' />,
                rules: [],
                formItem: <Input placeholder={getLocalesTemplate('tips.placeholder.input', { txt: getLocalesValue('frameworkmenu.Url') })} />
            },
            "Entity.ICon": {
                label: <FormattedMessage id='frameworkmenu.ICon' />,
                rules: [],
                formItem: <IConId />
            },
            /** 父目录 */
            "Entity.ParentId": {
                label: <FormattedMessage id='frameworkmenu.ParentId' />,
                rules: [],
                formItem: <WtmSelect placeholder={getLocalesTemplate('tips.placeholder.choose', { txt: getLocalesValue('frameworkmenu.ParentId') })}
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

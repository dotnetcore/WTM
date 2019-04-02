import { Input, Switch, Icon, Select, Upload, message, Modal, InputNumber } from 'antd';
import { WtmCascader, WtmCheckbox, WtmDatePicker, WtmEditor, WtmRadio, WtmSelect, WtmTransfer, WtmUploadImg, WtmUpload } from 'components/form';
import { FormItem } from 'components/dataView';
import * as React from 'react';
import lodash from 'lodash';
import { Observable } from 'rxjs';
import Regular from 'utils/Regular';
import Request from 'utils/Request';
/**
 * label  标识
 * rules   校验规则，参考下方文档  https://ant.design/components/form-cn/#components-form-demo-validate-other
 * formItem  表单组件
 */
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
                    dataSource={new Observable((sub) => {
                        import("pages/index").then(pages => {
                            const PagesList = [];
                            lodash.map(pages.default, (item) => {
                                if (item.controller) {
                                    PagesList.push({ Text: item.name, Value: item.controller })
                                }
                            })
                            sub.next(PagesList);
                            sub.complete();
                        })

                    })} />
            },
            /** 动作名称 */
            "SelectedActionIDs": {
                label: "动作名称",
                rules: [],
                formItem: <WtmSelect placeholder="选择动作"
                    multiple
                    linkageModels="SelectedModule"
                    dataSource={(parentid) => Request.cache({
                        url: "/api/_FrameworkMenu/GetActionsByModel", body: { "ModelName": parentid }
                    })}
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
            /** 图标 */
            "Entity.IConId": {
                label: "图标",
                rules: [],
                formItem: <WtmUploadImg />
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
    }
}
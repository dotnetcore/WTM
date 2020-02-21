import Request from '@leng/public/src/utils/request';
import lodash from 'lodash';
import { CreateElement } from 'vue';
import { EntitiesItems } from '../../../components/utils/type';

/**
 * label  标识
 * rules   校验规则，参考下方文档  https://ant.design/components/form-cn/#components-form-demo-validate-other
 * children  表单组件
 * 验证消息 https://github.com/yiminghe/async-validator#messages
 */
export default {
    /**
     * 表单模型 
     * @param props 
     */
    editEntities(props?): EntitiesItems {
        return {
            "Entity.ID":{
                label: "",
                options: {
                },
                children: `<a-input v-decorator />`
            },
            /** 权限类型 */
            "DpType": {
                
                label: "权限类型",
                options: {
                    rules: [{ required: true }]
                },
                dataSource: [
                    { label: "用户组权限", value: 0 },
                    { label: "用户权限", value: 1 },
                ],
                children: `<a-radio-group v-decorator />`
            },
            /** 权限名称 */
            "Entity.TableName": {
                
                label: "权限名称",
                options: {
                },
                dataSource: Request.cache({ url: "/api/_DataPrivilege/GetPrivileges" }),
                children: `<a-select v-decorator />`
            },
            /** 允许访问 */
            "SelectedItemsID": {
                
                label: "允许访问",
                options: {
                },
                linkage: ['Entity.TableName'],
                dataSource: ({ linkageValue }) => {
                    const table = lodash.get(linkageValue, 'Entity.TableName')
                    return table && Request.cache({
                        url: "/api/_DataPrivilege/GetPrivilegeByTableName", body: { table }
                    })
                },
                children: `<a-select v-decorator />`
            },
            /** 全部权限 */
            "IsAll": {
                
                label: "全部权限",
                options: {
                    initialValue: true
                },
                children: `<a-switch v-decorator >
                <a-icon type="check" slot="checkedChildren" />
                <a-icon type="close" slot="unCheckedChildren" />
              </a-switch>`
            },
            /** 用户Id */
            "UserItCode": {
                
                label: "用户账户",
                options: {
                },
                children: `<a-input v-decorator />`
            },
            /** 用户组 */
            "Entity.GroupId": {
                
                label: "用户组",
                options: {
                },
                dataSource: Request.cache({ url: "/api/_DataPrivilege/GetUserGroups" }),
                children: `<a-select v-decorator />`
            }
        }
    },
    /**
     * 搜索 模型 
     * @param props 
     */
    filterEntities(props?, h?: CreateElement): EntitiesItems {
        return {
            /** 权限名称 */
            "TableName": {
                label: "权限名称",
                options: {
                },
                children: `<a-input v-decorator />`
            },
            /** 权限类型 */
            "DpType": {
                label: "权限类型",
                options: {
                    initialValue:0
                },
                dataSource: [
                    { label: "用户组权限", value: 0 },
                    { label: "用户权限", value: 1 },
                ],
                children: `<a-radio-group v-decorator />`
            }
        }
    },
}
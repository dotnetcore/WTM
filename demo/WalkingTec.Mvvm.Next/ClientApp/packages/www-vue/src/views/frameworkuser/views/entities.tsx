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
            /** 唯一标识 的隐藏域 */
            "Entity.ID": {
                label: "ID",
                options: {},
                children: `<a-input v-decorator />`
            },
            /** 账号 */
            "Entity.ITCode": {
                label: "ITCode",
                options: {
                    initialValue: 'admin',
                    rules: [{ required: true }],
                },
                children: `<a-input v-decorator />`
            },
            /** 密码 */
            "Entity.Password": {
                label: { 'zh-CN': '密码', 'en-US': "Password" },
                options: {
                    rules: [{ required: true }]
                },
                children: `<a-input v-decorator />`
            },
            /** 邮箱 */
            "Entity.Email": {
                label: { 'zh-CN': '邮箱', 'en-US': "Email" },
                options: {
                },
                children: `<a-input v-decorator />`
            },
            /** 姓名 */
            "Entity.Name": {
                label: { 'zh-CN': '姓名', 'en-US': "Name" },
                options: {
                },
                children: `<a-input v-decorator />`
            },
            /** 性别 */
            "Entity.Sex": {
                label: { 'zh-CN': '性别', 'en-US': "Sex" },
                options: {
                    initialValue: 0,
                },
                dataSource: [
                    { Text: "男", Value: 0 },
                    { Text: "女", Value: 1 }
                ],
                children: `<a-select v-decorator />`
            },
            /** 手机 */
            "Entity.CellPhone": {
                label: { 'zh-CN': '手机', 'en-US': "CellPhone" },
                options: {
                },
                children: `<a-input v-decorator />`
            },
            /** 座机 */
            "Entity.HomePhone": {
                label: { 'zh-CN': '座机', 'en-US': "HomePhone" },
                options: {
                },
                children: `<a-input v-decorator />`
            },
            /** 住址 */
            "Entity.Address": {
                label: { 'zh-CN': '住址', 'en-US': "Address" },
                options: {
                },
                children: `<a-input v-decorator />`
            },
            // /** 邮编 */
            "Entity.ZipCode": {
                label: { 'zh-CN': '邮编', 'en-US': "ZipCode" },
                options: {
                },
                children: `<a-input v-decorator />`
            },
            /** 照片 */
            "Entity.PhotoId": {
                label: { 'zh-CN': '照片', 'en-US': "Photo" },
                options: {
                },
                children: `<w-avatar v-decorator />`,
                // children: `<a-upload action="https://www.mocky.io/v2/5cc8019d300000980a055e76" v-decorator v-display> <a-button> <a-icon type="upload" /> Click to Upload </a-button>
                // </a-upload>`
            },
            /** 是否有效 */
            "Entity.IsValid": {
                label: { 'zh-CN': '是否有效', 'en-US': "IsValid" },
                options: {
                    initialValue: true
                },
                children: `<a-switch v-decorator >
                <a-icon type="check" slot="checkedChildren" />
                <a-icon type="close" slot="unCheckedChildren" />
              </a-switch>`
            },
            /** 角色 */
            "Entity.UserRoles": {
                span: { span: 24 },
                label: { 'zh-CN': '角色', 'en-US': "UserRoles" },
                options: {
                },
                mapKey: 'RoleId',
                dataSource: () => Request.cache({ url: "/api/_FrameworkUserBase/GetFrameworkRoles" }),
                children: `<a-transfer v-decorator />`
            },
            /** 角色 */
            "Entity.UserRoles2": {
                span: { span: 24 },
                label: "UserRoles",
                options: {
                },
                linkage: ['Entity.UserRoles'],
                dataSource: ({ linkageValue }) => {
                    const UserRoles = lodash.get(linkageValue, 'Entity.UserRoles', []);
                    if (UserRoles.length) {
                        return Request.cache({ url: "/api/_FrameworkUserBase/GetFrameworkRoles" })
                    }
                },
                // dataSource: Request.cache({ url: "/api/_FrameworkUserBase/GetFrameworkRoles" }),
                children: `<a-checkbox-group v-decorator />`
            },
            /** 角色 */
            "Entity.UserRoles3": {
                span: { span: 24 },
                label: "UserRoles",
                options: {
                },
                dataSource: () => Request.cache({ url: "/api/_FrameworkUserBase/GetFrameworkRoles" }),
                children: `<a-radio-group v-decorator />`
            },
            /** 用户组 */
            "Entity.UserGroups": {
                span: { span: 24 },
                label: { 'zh-CN': '用户组', 'en-US': "UserGroups" },
                options: {
                },
                mapKey: 'GroupId',
                dataSource: () => Request.cache({ url: "/api/_FrameworkUserBase/GetFrameworkGroups" }),
                children: `<a-transfer v-decorator />`
            }

        }
    },
    /**
     * 搜索 模型 
     * @param props 
     */
    filterEntities(props?, h?: CreateElement): EntitiesItems {
        return {
            /** 账号 */
            "ITCode": {
                label: "ITCode",
                options: {
                },
                children: `<a-input v-decorator />`
            },
            /** 姓名 */
            "Name": {
                label: { 'zh-CN': '姓名', 'en-US': "Name" },
                children: `<a-input v-decorator />`
            },
            /** 性别 */
            "Entity.Sex": {
                label: { 'zh-CN': '性别', 'en-US': "Sex" },
                options: {
                },
                dataSource: [
                    { label: "男", value: 0 },
                    { label: "女", value: 1 }
                ],
                children: `<a-select v-decorator />`
            },
        }
    },
}
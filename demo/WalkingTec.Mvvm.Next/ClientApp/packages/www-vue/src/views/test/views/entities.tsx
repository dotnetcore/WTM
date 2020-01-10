import { Input } from 'ant-design-vue';
import { CreateElement } from 'vue';
import { EntitiesItems } from '../../../components/utils/entitiesHelp';
import Request from '@leng/public/src/utils/request';

/**
 * label  标识
 * rules   校验规则，参考下方文档  https://ant.design/components/form-cn/#components-form-demo-validate-other
 * children  表单组件
 */
export default {
    /**
     * 表单模型 
     * @param props 
     */
    editEntities(props?): EntitiesItems {
        return {
            /** 账号 */
            "Entity.ITCode": {
                label: "ITCode",
                options: {
                    initialValue: 'admin',
                    rules: [{ required: true, message: "ITCode 不能为空" }],
                },
                children: `<a-input v-decorator />`
            },
            /** 密码 */
            "Entity.Password": {
                label: "Password",
                options: {
                    rules: [{ required: true, message: "Password 不能为空" }]
                },
                children: `<a-input v-decorator />`
            },
            /** 邮箱 */
            "Entity.Email": {
                label: "Email",
                options: {
                },
                children: `<a-input v-decorator />`
            },
            /** 姓名 */
            "Entity.Name": {
                label: "Name",
                options: {
                },
                children: `<a-input v-decorator />`
            },
            /** 性别 */
            "Entity.Sex": {
                label: "Sex",
                options: {
                },
                dataSource: [
                    { label: "男", value: 0 },
                    { label: "女", value: 1 }
                ],
                children: `<a-select v-decorator />`
            },
            /** 手机 */
            "Entity.CellPhone": {
                label: "CellPhone",
                options: {
                },
                children: `<a-input v-decorator />`
            },
            /** 座机 */
            "Entity.HomePhone": {
                label: "HomePhone",
                options: {
                },
                children: `<a-input v-decorator />`
            },
            /** 住址 */
            "Entity.Address": {
                label: "Address",
                options: {
                },
                children: `<a-input v-decorator />`
            },
            // /** 邮编 */
            "Entity.ZipCode": {
                label: "ZipCode",
                options: {
                },
                children: `<a-input v-decorator />`
            },
            /** 照片 */
            "Entity.PhotoId": {
                label: "PhotoId",
                options: {
                },
                children: `<a-input v-decorator />`
            },
            /** 是否有效 */
            "Entity.IsValid": {
                label: "IsValid",
                options: {
                },
                children: `<a-input v-decorator />`
            },
            /** 角色 */
            "Entity.UserRoles": {
                label: "UserRoles",
                options: {
                },
                dataSource: Request.cache({ url: "/api/_FrameworkUserBase/GetFrameworkRoles" }),
                children: `<a-transfer v-decorator />`
            },
            /** 用户组 */
            "Entity.UserGroups": {
                label: "UserGroups",
                options: {
                },
                children: `<a-input v-decorator />`
            }

        }
    },
    /**
     * 搜索 模型 
     * @param props 
     */
    filterEntities(props, h: CreateElement): EntitiesItems {
        return {
            /** 账号 */
            "ITCode": {
                label: "账号",
                // options: { initialValue: 'admin' },
                children: <Input />
            },
            /** 姓名 */
            "Name": {
                label: "姓名",
                children: <Input />
            },
        }
    },
}
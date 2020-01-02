import { Input } from 'ant-design-vue';
import { CreateElement } from 'vue';
import { EntitiesItems } from '../../../components/utils/entitiesHelp';
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
            // /** 邮箱 */
            // "Entity.Email": {
            //     label: "邮箱",
            //     options: {
            //     },
            //     children: <Input placeholder="请输入 邮箱" />
            // },
            // /** 姓名 */
            // "Entity.Name": {
            //     label: "姓名",
            //     options: {
            //     },
            //     children: <Input placeholder="请输入 姓名" />
            // },
            // /** 性别 */
            // "Entity.Sex": {
            //     label: "性别",
            //     options: {
            //     },
            //     children: <WtmSelect placeholder="性别" dataSource={[
            //         { Text: "男", Value: 0 },
            //         { Text: "女", Value: 1 }
            //     ]} />
            // },
            // /** 手机 */
            // "Entity.CellPhone": {
            //     label: "手机",
            //     options: {
            //     },
            //     children: <Input placeholder="请输入 手机" />
            // },
            // /** 座机 */
            // "Entity.HomePhone": {
            //     label: "座机",
            //     options: {
            //     },
            //     children: <Input placeholder="请输入 座机" />
            // },
            // /** 住址 */
            // "Entity.Address": {
            //     label: "住址",
            //     options: {
            //     },
            //     children: <Input placeholder="请输入 住址" />
            // },
            // /** 邮编 */
            // "Entity.ZipCode": {
            //     label: "邮编",
            //     options: {
            //     },
            //     children: <Input placeholder="请输入 邮编" />
            // },
            // /** 照片 */
            // "Entity.PhotoId": {
            //     label: "照片",
            //     options: {
            //     },
            //     children: <WtmUploadImg />
            // },
            // /** 是否有效 */
            // "Entity.IsValid": {
            //     label: "是否有效",
            //     options: {
            //     },
            //     children: <Switch checkedChildren={<Icon type="check" />} unCheckedChildren={<Icon type="close" />} />
            // },
            // /** 角色 */
            // "Entity.UserRoles": {
            //     label: "角色",
            //     options: {
            //     },
            //     children: <WtmTransfer
            //         dataSource={Request.cache({ url: "/api/_FrameworkUserBase/GetFrameworkRoles" })}
            //         mapKey="RoleId"
            //     />
            // },
            // /** 用户组 */
            // "Entity.UserGroups": {
            //     label: "用户组",
            //     options: {
            //     },
            //     children: <WtmTransfer
            //         dataSource={Request.cache({ url: "/api/_FrameworkUserBase/GetFrameworkGroups" })}
            //         mapKey="GroupId"
            //     />
            // }

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
                options: { initialValue: 'admin' },
                children: <Input />
            },
            /** 姓名 */
            "Name": {
                label: "姓名",
                children: <Input />
            },
            /** 姓名 */
            "Name2": {
                label: "姓名",
                children: <Input />
            },
            /** 姓名 */
            "Name3": {
                label: "姓名",
                children: <Input />
            },
            /** 姓名 */
            "Name4": {
                label: "姓名",
                children: <Input />
            },
            /** 姓名 */
            "Name5": {
                label: "姓名",
                children: <Input />
            },
            /** 姓名 */
            "Name6": {
                label: "姓名",
                children: <Input />
            },

        }
    },
}
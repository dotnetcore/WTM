import { Form, Input, Col, Row } from 'ant-design-vue';
import lodash from 'lodash';
import { FieldDecoratorOptions } from 'ant-design-vue/types/form/form';
import { CreateElement } from 'vue';
interface FormItem {
    label: string;
    options?: FieldDecoratorOptions;
    children: JSX.Element;
}
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
    editModels(props?): { [key: string]: FormItem } {
        return {
            /** 账号 */
            "Entity.ITCode": {
                label: "账号",
                options: {
                    rules: [{ "required": true, "message": "账号不能为空" }]
                },
                children: <Input {...{ placeholder: "请输入 账号" }} />
            },
            /** 密码 */
            "Entity.Password": {
                label: "密码",
                options: {
                    rules: [{ "required": true, "message": "密码不能为空" }]
                },
                children: <Input {...{ placeholder: "请输入 密码" }} />
            },
            // /** 邮箱 */
            // "Entity.Email": {
            //     label: "邮箱",
            //     rules: [],
            //     formItem: <Input placeholder="请输入 邮箱" />
            // },
            // /** 姓名 */
            // "Entity.Name": {
            //     label: "姓名",
            //     rules: [{ "required": true, "message": "姓名不能为空" }],
            //     formItem: <Input placeholder="请输入 姓名" />
            // },
            // /** 性别 */
            // "Entity.Sex": {
            //     label: "性别",
            //     rules: [],
            //     formItem: <WtmSelect placeholder="性别" dataSource={[
            //         { Text: "男", Value: 0 },
            //         { Text: "女", Value: 1 }
            //     ]} />
            // },
            // /** 手机 */
            // "Entity.CellPhone": {
            //     label: "手机",
            //     rules: [],
            //     formItem: <Input placeholder="请输入 手机" />
            // },
            // /** 座机 */
            // "Entity.HomePhone": {
            //     label: "座机",
            //     rules: [],
            //     formItem: <Input placeholder="请输入 座机" />
            // },
            // /** 住址 */
            // "Entity.Address": {
            //     label: "住址",
            //     rules: [],
            //     formItem: <Input placeholder="请输入 住址" />
            // },
            // /** 邮编 */
            // "Entity.ZipCode": {
            //     label: "邮编",
            //     rules: [],
            //     formItem: <Input placeholder="请输入 邮编" />
            // },
            // /** 照片 */
            // "Entity.PhotoId": {
            //     label: "照片",
            //     rules: [],
            //     formItem: <WtmUploadImg />
            // },
            // /** 是否有效 */
            // "Entity.IsValid": {
            //     label: "是否有效",
            //     rules: [
            //         // { "required": true, "message": "是否有效不能为空" }
            //     ],
            //     formItem: <Switch checkedChildren={<Icon type="check" />} unCheckedChildren={<Icon type="close" />} />
            // },
            // /** 角色 */
            // "Entity.UserRoles": {
            //     label: "角色",
            //     rules: [],
            //     formItem: <WtmTransfer
            //         dataSource={Request.cache({ url: "/api/_FrameworkUserBase/GetFrameworkRoles" })}
            //         mapKey="RoleId"
            //     />
            // },
            // /** 用户组 */
            // "Entity.UserGroups": {
            //     label: "用户组",
            //     rules: [],
            //     formItem: <WtmTransfer
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
    filterModels(props, h: CreateElement): { [key: string]: FormItem } {
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

        }
    },
    /**
     * 渲染 模型
     */
    renderModels({ models, form }, h: CreateElement) {
        function render() {
            return lodash.map(models, (item: FormItem, key) => {
                return <Col props={{ span: 8 }}>
                    <Form.Item props={{ label: item.label }} >
                        {form.getFieldDecorator(key, item.options)(item.children)}
                    </Form.Item>
                </Col>
            })
        }
        return <Row props={{ gutter: 20 }}>
            {render()}
        </Row>
    }
}
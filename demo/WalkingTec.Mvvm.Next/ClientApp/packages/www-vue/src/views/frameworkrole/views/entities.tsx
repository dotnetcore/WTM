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
            /** 角色编号 */
            "Entity.RoleCode": {
                label: "角色编号",
                span: 24,
                options: {
                    rules: [{ required: true }],
                },
                children: `<a-input v-decorator />`
            },
            /** 角色名称 */
            "Entity.RoleName": {
                label: "角色名称",
                span: 24,
                options: {
                    initialValue: 'admin',
                    rules: [{ required: true }],
                },
                children: `<a-input v-decorator />`
            },
            /** 备注 */
            "Entity.RoleRemark": {
                label: "备注",
                span: 24,
                children: `<a-input v-decorator />`
            }
        }
    },
    /**
     * 搜索 模型 
     * @param props 
     */
    filterEntities(props?, h?: CreateElement): EntitiesItems {
        return {
            /** 角色编号 */
            "RoleCode": {
                label: "角色编号",
                children: `<a-input v-decorator />`
            },
            /** 角色名称 */
            "RoleName": {
                label: "角色名称",
                children: `<a-input v-decorator />`
            },
        }
    },
}
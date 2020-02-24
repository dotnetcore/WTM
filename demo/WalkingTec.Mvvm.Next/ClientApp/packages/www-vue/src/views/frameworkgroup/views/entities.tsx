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
            /** 用户组编码 */
            "Entity.GroupCode": {
                label: "用户组编码",
                span:24,
                options: {
                    rules: [{ required: true }]
                },
                children: `<a-input v-decorator />`
            },
            /** 用户组名称 */
            "Entity.GroupName": {
                label: "用户组名称",
                span:24,
                options: {
                    rules: [{ required: true }]
                },
                children: `<a-input v-decorator />`
            },
            /** 备注 */
            "Entity.GroupRemark": {
                label: "备注",
                span:24,
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
    filterEntities(props?, h?: CreateElement): EntitiesItems {
        return {
            /** 用户组编码 */
            "GroupCode": {
                label: "用户组编码",
                options: {
                },
                children: `<a-input v-decorator />`
            },
            /** 用户组名称 */
            "GroupName": {
                label: "用户组名称",
                options: {
                },
                children: `<a-input v-decorator />`
            },
        }
    },
}
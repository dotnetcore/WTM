import { Input } from 'antd';
import { FormItem } from 'components/dataView';
import { WtmCascader, WtmCheckbox, WtmDatePicker, WtmEditor, WtmRadio, WtmSelect, WtmTransfer, WtmUploadImg, WtmUpload } from 'components/form';
import lodash from 'lodash';
import moment from 'moment';
import * as React from 'react';
import Regular from 'utils/Regular'; //正则
import Store from '../store';

/**
 * label  标识
 * rules   校验规则，参考下方文档  https://ant.design/components/form-cn/#components-form-demo-validate-other
 * formItem  表单组件
 */
export default {
    /**
     * 编辑 模型 
     * @param props 
     */
    editModels(props?): WTM.FormItem {
        return {
            /** ITCode */
            ITCode: {
                label: "账号",
                rules: [{ "required": true, "message": "账号 不能为空" }],
                formItem: <Input placeholder="请输入 ITCode" />
            },
            /** Password */
            Password: {
                label: "密码",
                rules: [{ "required": true, "message": "密码 不能为空" }],
                formItem: <Input placeholder="请输入 Password" />
            },
            /** Email */
            Email: {
                label: "邮箱",
                rules: [{ pattern: Regular.email, message: "请输入正确的 邮箱" }],
                formItem: <Input placeholder="请输入 Email" />
            },
            /** Name */
            Name: {
                label: "名称",
                rules: [],
                formItem: <Input placeholder="请输入 Name" />
            },
            /** 照片 */
            PhotoId: {
                label: "照片",
                rules: [],
                formItem: <WtmUploadImg />
            },
            /** 性别 */
            Sex: {
                label: "性别",
                rules: [],
                formItem: <WtmSelect
                    placeholder="性别"
                    //请求 数据 Observable 对象，
                    dataSource={Store.Request.cache({ url: "/frameworkuser/GetSex" })}
                />
            },
            /** 用户组 */
            UserGroups: {
                label: "用户组",
                rules: [],
                formItem: <WtmSelect
                    placeholder="用户组"
                    multiple // 多选标记
                    //请求 数据 Observable 对象，
                    dataSource={Store.Request.cache({ url: "/frameworkuser/GetUserGroups" })}
                    dataKey="GroupId"
                />
            },
            /** 角色 */
            UserRoles: {
                label: "角色",
                rules: [],
                formItem: <WtmTransfer
                    //请求 数据 Observable 对象，
                    dataSource={Store.Request.cache({ url: "/frameworkuser/GetUserRoles" })}
                    dataKey="RoleId"
                />
            },
            CreateTime: {
                label: "时间测试",
                rules: [],
                formItem: <WtmDatePicker placeholder=" 请输入时间测试" />
            },
            Date2: {
                label: "时间测试2",//显示 时间，禁用 小于当前天数
                rules: [],
                formItem: <WtmDatePicker showTime disabledDate={(current) => current < moment().subtract(1, "day").endOf('day')} />
            },
            Editer: {
                label: "富文本",
                rules: [{ "required": true, "message": "富文本 不能为空" }],
                formItem: <WtmEditor placeholder="输入正文内容" />
            }
        }
    },
    /**
     * 搜索 模型 
     * @param props 
     */
    searchModels(props?): WTM.FormItem {
        return {
            /** ITCode */
            ITCode: {
                label: "账号",
                rules: [],
                formItem: <Input placeholder="请输入 ITCode" />
            },
            /** Name */
            Name: {
                label: "名称",
                rules: [],
                formItem: <Input placeholder="请输入 Name" />
            },
            /** 用户组 */
            UserGroups: {
                label: "用户组",
                rules: [],
                formItem: <WtmSelect
                    placeholder="用户组"
                    //请求 数据 Observable 对象，
                    dataSource={Store.Request.cache({ url: "/frameworkuser/GetUserGroups" })}
                    dataKey="GroupId"
                />
            }
        }
    },
    testModels(props?): WTM.FormItem {
        return {
            Input: {
                label: "文本",
                rules: [{ "required": true, "message": "账号 不能为空" }],
                formItem: <Input placeholder="请输入 文本" />
            },
            Email: {
                label: "正则文本",
                rules: [{ pattern: Regular.email, message: "请输入正确的 邮箱" }],
                formItem: <Input placeholder="请输入 Email" />
            },
            /** 照片 */
            PhotoId: {
                label: "照片",
                rules: [],
                formItem: <WtmUploadImg />
            },
            /** 地点 */
            PlaceId: {
                label: "地点",
                rules: [],
                formItem: <WtmCascader placeholder="地点"
                    dataSource={Store.Request.cache({ url: "/School/GetCitys" })}
                />
            },
            /** 地点2 */
            Place2_Sheng: {
                label: "省",
                rules: [],
                formItem: <WtmSelect placeholder="省"
                    dataSource={Store.Request.cache({ url: "/School/GetSubCities", body: { parentid: null } })}
                />
            },
            /** 地点2 */
            Place2_Shi: {
                label: "市",
                rules: [],
                formItem: <WtmSelect placeholder="市"
                    linkageModels="Place2_Sheng"
                    dataSource={(parentid) => Store.Request.cache({ url: "/School/GetSubCities", body: { parentid } })}
                />
            },
            /** 地点2 */
            Place2Id: {
                label: "区",
                rules: [],
                formItem: <WtmSelect placeholder="区"
                    linkageModels="Place2_Shi"
                    dataSource={(parentid) => Store.Request.cache({ url: "/School/GetSubCities", body: { parentid } })}
                />
            },
            /** 性别 */
            // Sex: {
            //     label: "性别",
            //     rules: [],
            //     formItem: <WtmSelect
            //         placeholder="性别"
            //         //请求 数据 Observable 对象，
            //         dataSource={Store.Request.cache({ url: "/frameworkuser/GetSex" })}
            //     />
            // },
            /** 性别 */
            WtmRadio: {
                label: "性别",
                rules: [],
                formItem: <WtmRadio
                    placeholder="性别"
                    // value="1"
                    //请求 数据 Observable 对象，
                    dataSource={Store.Request.cache({ url: "/frameworkuser/GetSex" })}
                />
            },
            /** 用户组 */
            // UserGroups: {
            //     label: "用户组",
            //     rules: [],
            //     formItem: <WtmSelect
            //         placeholder="用户组"
            //         multiple // 多选标记
            //         //请求 数据 Observable 对象，
            //         dataSource={Store.Request.cache({ url: "/frameworkuser/GetUserGroups" })}
            //         dataKey="GroupId"
            //     />
            // },
            WtmCheckbox: {
                label: "用户组",
                rules: [],
                formItem: <WtmCheckbox
                    placeholder="用户组"
                    //请求 数据 Observable 对象，
                    dataSource={Store.Request.cache({ url: "/frameworkuser/GetUserGroups" })}
                    dataKey="GroupId"
                />
            },
            /** 角色 */
            UserRoles: {
                label: "角色",
                rules: [],
                formItem: <WtmTransfer
                    //请求 数据 Observable 对象，
                    dataSource={Store.Request.cache({ url: "/frameworkuser/GetUserRoles" })}
                    dataKey="RoleId"
                />
            },
            CreateTime: {
                label: "时间测试",
                rules: [],
                formItem: <WtmDatePicker />
            },
            Date2: {
                label: "时间测试2",//显示 时间，禁用 小于当前天数
                rules: [],
                formItem: <WtmDatePicker showTime disabledDate={(current) => current < moment().subtract(1, "day").endOf('day')} />
            },
            Editer: {
                label: "富文本",
                rules: [{ "required": true, "message": "富文本 不能为空" }],
                formItem: <WtmEditor placeholder="输入正文内容" />
            },
            WtmUpload: {
                label: "文件上传",
                rules: [{ "required": true, "message": "文件 不能为空" }],
                formItem: <WtmUpload />
            },
            Test: {
                label: "测试获取数据",
                rules: [],
                formItem: () => {
                    return <Input placeholder="请输入 Email" />
                }
            }
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
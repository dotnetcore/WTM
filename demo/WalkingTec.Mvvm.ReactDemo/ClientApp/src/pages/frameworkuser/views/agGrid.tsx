import { columnsRender, columnsRenderImg, AgGrid } from 'components/dataView';
import { DesError } from 'components/decorators';
import React from 'react';
import Store from '../store';
import Action from './action';
import { Switch, Icon } from 'antd';
/**
 * 表格
 */
@DesError
export default class extends React.Component<any, any> {
    render() {
        return <AgGrid
            Store={Store}
            columnDefs={[
                {
                    headerName: "账号", field: "ITCode",
                },
                {
                    headerName: "姓名", field: "Name",
                },
                {
                    headerName: "性别", field: "Sex",
                },
                {
                    headerName: "照片", field: "PhotoId", cellRenderer: "columnsRenderImg"
                },
                {
                    headerName: "是否有效", field: "IsValid",cellRenderer: "columnsRenderBoolean"
                },
                {
                    headerName: "角色", field: "RoleName_view",
                },
                {
                    headerName: "用户组", field: "GroupName_view",
                },
            ]}
            rowHeight={110}
            frameworkComponents={{
                columnsRenderImg: (props) => {
                    return columnsRenderImg(props.value, props.data)
                },
                columnsRenderBoolean: (props) => {
                    return props.value ? <Switch checkedChildren={<Icon type="check" />} unCheckedChildren={<Icon type="close" />} disabled defaultChecked /> : <Switch checkedChildren={<Icon type="check" />} unCheckedChildren={<Icon type="close" />} disabled />
                }
            }}
        />
    }
}

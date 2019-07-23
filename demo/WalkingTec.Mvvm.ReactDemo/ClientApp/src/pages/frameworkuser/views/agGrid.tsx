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
                    headerName: "是否有效", field: "IsValid", cellRenderer: "columnsRenderBoolean"
                },
                {
                    headerName: "角色", field: "RoleName_view",
                },
                {
                    headerName: "用户组", field: "GroupName_view",
                },
                // 固定右侧 操作列
                {
                    headerName: "Action",
                    field: "RowAction",
                    cellRenderer: 'RowAction',
                    pinned: 'right',
                    editable: false,
                    filter: false,
                }
            ]}
            rowHeight={110}
            frameworkComponents={{
                /**
                * 行 动作
                */
                RowAction: Action.rowAction,
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

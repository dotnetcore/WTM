import { ColDef, ColGroupDef } from 'ag-grid-community';
import { Icon } from 'antd';
import { AgGrid } from 'components/dataView';
import { mergeLocales } from 'locale';
import React from 'react';
import Store from '../store';
import Action from './action';
mergeLocales({
    "zh-CN": {
        'frameworkmenu.DisplayOrder': '顺序',
        'frameworkmenu.ICon': '图标',
    },
    "en-US": {
        'frameworkmenu.DisplayOrder': 'DisplayOrder',
        'frameworkmenu.ICon': 'ICon',
    }
});
// 列配置
const columnDefs: (ColDef | ColGroupDef)[] = [
    // {
    //     headerName: "页面名称", field: "PageName"
    // },
    {
        headerName: "frameworkmenu.DisplayOrder", field: "DisplayOrder",
    },
    {
        headerName: "frameworkmenu.ICon", field: "ICon", cellRenderer: "renderIcon"
    },
]
/**
 * 表格
 */
export default class extends React.Component<any, any> {
    render() {
        return <AgGrid
            treeData
            groupDefaultExpanded={-1}
            getDataPath={data => data.treePath}
            autoGroupColumnDef={{
                headerName: "页面名称",
                cellRendererParams: { suppressCount: true }
            }}
            // 页面状态 
            Store={Store}
            // 列配置
            columnDefs={columnDefs}
            // 行操作 
            rowAction={Action.rowAction}
            // 行操作 col props 同 columnDefs配置相同
            // rowActionCol={{ headerName: "操作" }}
            frameworkComponents={{
                // 注册一个 图标渲染组件
                renderIcon: ({ value, data }) => {
                    if (data.ICon) {
                        return <div  >
                            <Icon type={data.ICon} style={{ fontSize: 20 }} />
                        </div>
                    }
                    return null
                }
            }}
        />
    }
}

import { ColDef, ColGroupDef } from 'ag-grid-community';
import { Icon } from 'antd';
import { AgGrid } from 'components/dataView';
import React from 'react';
import RequestFiles from 'utils/RequestFiles';
import Store from '../store';
import Action from './action';
// 列配置
const columnDefs: (ColDef | ColGroupDef)[] = [
    // {
    //     headerName: "页面名称", field: "PageName"
    // },
    {
        headerName: "顺序", field: "DisplayOrder",
    },
    {
        headerName: "图标", field: "ICon", cellRenderer: "renderIcon"
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
                    if (value) {
                        return <div  >
                            <img style={{ height: 20, width: 20, objectFit: "cover" }} src={RequestFiles.onFileDownload(value)} alt="" />
                        </div>
                    }
                    if (data.CustomICon) {
                        return <div  >
                            <Icon type={data.CustomICon} style={{ fontSize: 20 }} />
                        </div>
                    }
                    return null
                }
            }}
        />
    }
}

/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-06-26 16:55:28
 * @modify date 2019-06-26 16:55:28
 * @desc [description]
 */

import { ColDef } from 'ag-grid-community';
import { AgGridReactProps } from 'ag-grid-react';
import { PaginationProps } from 'antd/lib/pagination';
import * as React from 'react';
import Store from 'store/dataSource';
// import GridCommunity from './gridCommunity';
import "./style.less";
const GridCommunity = React.lazy(() => import('./gridCommunity'));
interface ITableProps extends AgGridReactProps {
    /** 状态 */
    Store: Store;
    /**
     * 行 操作
     */
    rowAction?: any;
    /**
     * 行 操作列配置
     */
    rowActionCol?: ColDef;
    /**
     * 容器样式
     */
    style?: React.CSSProperties;
    /**
     * 主题
     */
    theme?: string;
    /**
     * 样式
     */
    className?: string;
    /**
     * 分页
     */
    paginationProps?: PaginationProps | boolean;
    /**
     * 加载中
     */
    loading?: boolean;
    /**
     * 选择
     */
    checkboxSelection?: boolean;
}
export const AgGrid: React.FC<ITableProps> = (props) => (
    <React.Suspense fallback={<div style={{ height: 300 }}></div>}>
        <GridCommunity {...props} />
    </React.Suspense>
);
export default AgGrid
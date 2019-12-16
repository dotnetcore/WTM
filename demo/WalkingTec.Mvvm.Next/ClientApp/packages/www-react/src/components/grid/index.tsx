/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-06-26 16:55:28
 * @modify date 2019-06-26 16:55:28
 * @desc [description]
 */
import * as React from 'react';
import { AgGridReactProps } from 'ag-grid-react';
import GridPagination from './pagination';
import 'ag-grid-community/dist/styles/ag-grid.css';
import 'ag-grid-community/dist/styles/ag-theme-balham.css';
import 'ag-grid-community/dist/styles/ag-theme-material.css';
// import "./style.less";
const Gird = React.lazy(() => import('./grid'));
interface GridProps extends AgGridReactProps {

}
export const AgGrid: React.FC<GridProps> = (props) => (
    <React.Suspense fallback={<div style={{ height: 300 }}></div>}>
        <Gird {...props} >
            <GridPagination {...props} />
        </Gird>
    </React.Suspense>
);
export default AgGrid
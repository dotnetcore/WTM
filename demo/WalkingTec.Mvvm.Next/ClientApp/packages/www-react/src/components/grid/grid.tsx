import { AgGridReact, AgGridReactProps } from 'ag-grid-react';
import { LicenseManager } from 'ag-grid-enterprise';
import * as React from 'react';
import lodash from 'lodash';
const AgGridReactTable: any = AgGridReact;
// LicenseManager.setLicenseKey('SHI_International_Corp_-_USA__on_behalf_of_COLGATE__MultiApp_6Devs7_June_2020__MTU5MTQ4NDQwMDAwMA==8b6496bd559839df6c9ff807a6392b25');
LicenseManager.setLicenseKey('ag-Grid_Evaluation_License_Not_for_Production_100Devs30_August_2037__MjU4ODczMzg3NzkyMg==9e93ed5f03b0620b142770f2594a23a2');
export class AgGrid extends React.Component<AgGridReactProps> {
    componentDidMount() {
    }
    default: AgGridReactProps = {
        // columnDefs
        // groupUseEntireRow:true,
        masterDetail: true,
        rowGroupPanelShow: "always",
        onGridReady: (event) => {
            event.api.sizeColumnsToFit();
            this.props.onGridReady && this.props.onGridReady(event)
        }
    }
    render() {
        const { children, onGridReady, ...props } = this.props;
        return <div className='ag-theme-material'>
            <div style={{ height: 400 }}>
                <AgGridReactTable {...lodash.merge(this.default, props)} />
            </div>
            {children}
        </div>
    }
}
export default AgGrid
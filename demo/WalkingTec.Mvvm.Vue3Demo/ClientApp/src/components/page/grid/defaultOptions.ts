import { ColDef, ColGroupDef, GridOptions } from "ag-grid-community";
import lodash from "lodash";
import { VueI18n } from 'vue-i18n'
import { frameworkComponents } from "./framework";
import { AG_GRID_LOCALE_ZH } from "./locale";
export default (i18n: VueI18n) => {
    const defaultOptions: GridOptions = {
        suppressColumnVirtualisation: true,
        rowBuffer: 0,
        loadingOverlayComponent: frameworkComponents.loadingOverlay,
        noRowsOverlayComponent: frameworkComponents.noRowsOverlay,
        localeText: i18n.locale === 'zh' ? AG_GRID_LOCALE_ZH : undefined,
        rowSelection: "multiple",
        rowMultiSelectWithClick: true,
        // debug:true,
        defaultColDef: {
            sortable: true,
            resizable: true,
            headerValueGetter: (params) => {
                try {
                    return i18n.t(lodash.get(params, 'colDef.headerName'))
                } catch (error) {
                    return ''
                }
            },
            minWidth: 160
        },
        sideBar: {
            toolPanels: [
                {
                    id: "columns",
                    labelDefault: "Columns",
                    labelKey: "columns",
                    iconKey: "columns",
                    toolPanel: "agColumnsToolPanel",
                    toolPanelParams: {
                        // suppressRowGroups: true,
                        suppressValues: true,
                        suppressPivots: true,
                        suppressPivotMode: true,
                        // suppressSideButtons: true,
                        // suppressColumnFilter: true,
                        // suppressColumnSelectAll: true,
                        // suppressColumnExpandAll: true
                    },
                },
                {
                    id: "filters",
                    labelDefault: "Filters",
                    labelKey: "filters",
                    iconKey: "filter",
                    toolPanel: "agFiltersToolPanel",
                },
            ],
        }
    }
    return defaultOptions
}

/**
 * 行 操作
 * @param frameworkComponents 
 */
export function getColumnDefsAction(frameworkComponents): (ColGroupDef | ColDef)[] {
    if (lodash.has(frameworkComponents, 'RowAction')) {
        return [{
            // minWidth: 0,
            headerName: 'action_name',
            field: 'RowAction',
            cellRenderer: 'RowAction',
            cellClass: 'w-row-action',
            pinned: window.innerWidth > 701 ? 'right' : '',
            sortable: false,
            minWidth: 140,
            suppressMenu: true,
            suppressColumnsToolPanel: true,
        }]
    }
    return []
}
/**
 * 行 头部选择框
 * @param checkboxSelection 
 */
export function getColumnDefsCheckbox(checkboxSelection, theme: 'balham' | 'alpine' | 'material'): (ColGroupDef | ColDef)[] {
    if (lodash.eq(checkboxSelection, false)) {
        return []
    }
    const width = { material: 65, alpine: 50, balham: 40 }[theme]
    return [{
        pinned: window.innerWidth > 701 ? "left" : '',
        rowDrag: false,
        dndSource: false,
        lockPosition: true,
        suppressMenu: true,
        suppressSizeToFit: true,
        suppressMovable: true,
        suppressNavigable: true,
        suppressCellFlash: true,
        enableRowGroup: false,
        enablePivot: false,
        enableValue: false,
        editable: false,
        suppressColumnsToolPanel: true,
        filter: false,
        resizable: false,
        checkboxSelection: true,
        headerCheckboxSelection: true,
        width: width,
        minWidth: width,
        maxWidth: width,
    }]
}


import { $i18n, WTM_EntitiesField, WTM_ValueType, FieldRequest } from '@/client';
import router from '@/router';
import { ColGroupDef, GridOptions } from 'ag-grid-community';
import { ColDef } from 'ag-grid-community';
import lodash from 'lodash';
import { EnumLocaleLabel } from '../locales';

/**
 * 页面实体
 */
class Entity {
    readonly [WTM_ValueType.text]: WTM_EntitiesField = {
        name: WTM_ValueType.text,
        label: WTM_ValueType.text,
        rules: [{ required: true }]
    }
    readonly [WTM_ValueType.password]: WTM_EntitiesField = {
        name: WTM_ValueType.password,
        label: WTM_ValueType.password,
        valueType: WTM_ValueType.password,
    }
    readonly [WTM_ValueType.textarea]: WTM_EntitiesField = {
        name: WTM_ValueType.textarea,
        label: WTM_ValueType.textarea,
        valueType: WTM_ValueType.textarea,
    }
    readonly [WTM_ValueType.rate]: WTM_EntitiesField = {
        name: WTM_ValueType.rate,
        label: WTM_ValueType.rate,
        valueType: WTM_ValueType.rate,
    }
    readonly [WTM_ValueType.switch]: WTM_EntitiesField = {
        name: WTM_ValueType.switch,
        label: WTM_ValueType.switch,
        valueType: WTM_ValueType.switch,
    }
    readonly [WTM_ValueType.slider]: WTM_EntitiesField = {
        name: WTM_ValueType.slider,
        label: WTM_ValueType.slider,
        valueType: WTM_ValueType.slider,
    }
    readonly [WTM_ValueType.date]: WTM_EntitiesField = {
        name: WTM_ValueType.date,
        label: WTM_ValueType.date,
        valueType: WTM_ValueType.date,
    }
    readonly [WTM_ValueType.dateMonth]: WTM_EntitiesField = {
        name: WTM_ValueType.dateMonth,
        label: WTM_ValueType.dateMonth,
        valueType: WTM_ValueType.dateMonth,
    }
    readonly [WTM_ValueType.dateRange]: WTM_EntitiesField = {
        name: WTM_ValueType.dateRange,
        label: WTM_ValueType.dateRange,
        valueType: WTM_ValueType.dateRange,
    }
    readonly [WTM_ValueType.dateWeek]: WTM_EntitiesField = {
        name: WTM_ValueType.dateWeek,
        label: WTM_ValueType.dateWeek,
        valueType: WTM_ValueType.dateWeek,
    }
    readonly [WTM_ValueType.image]: WTM_EntitiesField = {
        name: WTM_ValueType.image,
        label: WTM_ValueType.image,
        valueType: WTM_ValueType.image,
    }
    readonly [WTM_ValueType.upload]: WTM_EntitiesField = {
        name: WTM_ValueType.upload,
        label: WTM_ValueType.upload,
        valueType: WTM_ValueType.upload,
    }
    readonly [WTM_ValueType.select]: WTM_EntitiesField = {
        name: WTM_ValueType.select,
        label: WTM_ValueType.select,
        valueType: WTM_ValueType.select,
        request: async () => FieldRequest('/api/_FrameworkUserBase/GetFrameworkRoles'),
    }
    readonly SelectedModule: WTM_EntitiesField = {
        name: 'SelectedModule',
        label: EnumLocaleLabel.SelectedModule,
        request: router.onGetRequest,
        valueType: WTM_ValueType.select
    }
    readonly SelectedActionIDs: WTM_EntitiesField = {
        name: 'SelectedActionIDs',
        label: EnumLocaleLabel.SelectedActionIDs,
        // 联动 SelectedModule  ['Entity', 'FolderOnly'] name 使用 Entity.FolderOnly
        linkage: ['SelectedModule'],
        request: async (formState) => {
            // console.log("LENG ~ Entity ~ request: ~ formState", formState)
            // await of(1).pipe(delay(1000)).toPromise() 模拟网速慢
            const ModelName = lodash.get(formState, 'SelectedModule');
            return ModelName ? FieldRequest({ url: "/api/_FrameworkMenu/GetActionsByModel", body: { ModelName } }) : []
        },
        valueType: WTM_ValueType.select,
        fieldProps: { mode: 'tags' }
    }
    readonly [WTM_ValueType.radio]: WTM_EntitiesField = {
        name: WTM_ValueType.radio,
        label: WTM_ValueType.radio,
        valueType: WTM_ValueType.radio,
        // 远程数据
        request: async (formState) => {
            return [
                { label: $i18n.t(EnumLocaleLabel.Sex_Male), value: 'Male' },
                { label: $i18n.t(EnumLocaleLabel.Sex_Female), value: 'Female' }
            ]
        },
        rules: [{ required: true }]
    }
    readonly [WTM_ValueType.checkbox]: WTM_EntitiesField = {
        name: WTM_ValueType.checkbox,
        label: WTM_ValueType.checkbox,
        valueType: WTM_ValueType.checkbox,
        request: async () => FieldRequest('/api/_FrameworkUserBase/GetFrameworkRoles'),
    }
    readonly [WTM_ValueType.transfer]: WTM_EntitiesField = {
        name: WTM_ValueType.transfer,
        label: WTM_ValueType.transfer,
        valueType: WTM_ValueType.transfer,
        request: async () => FieldRequest('/api/_FrameworkUserBase/GetFrameworkRoles'),
    }
    readonly [WTM_ValueType.editor]: WTM_EntitiesField = {
        name: WTM_ValueType.editor,
        label: WTM_ValueType.editor,
        valueType: WTM_ValueType.editor,
    }
    readonly [WTM_ValueType.grid]: WTM_EntitiesField = {
        name: WTM_ValueType.grid,
        label: WTM_ValueType.grid,
        valueType: WTM_ValueType.grid,
        fieldProps: { columnDefs: this.columnDefs, gridOptions: this.gridOptions }
    }
    get columnDefs(): (ColDef | ColGroupDef)[] {
        return [
            { field: "Name", headerName: "Name" },
            { field: "Age", headerName: "Age" },
            {
                field: "Sex",
                headerName: "Sex",
                cellEditor: "agSelectCellEditor",
                cellEditorParams: {
                    values: ["男", "女"]
                }
            }
        ];
    }
    get gridOptions(): GridOptions {
        return {};
    }
}
export const PageEntity = new Entity()
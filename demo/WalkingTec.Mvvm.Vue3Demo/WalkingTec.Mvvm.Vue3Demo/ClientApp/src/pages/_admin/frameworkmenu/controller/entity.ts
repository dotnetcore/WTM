
import { $i18n, WTM_EntitiesField, WTM_ValueType, FieldRequest } from '@/client';
import { EnumLocaleLabel } from '../locales';
import router from '@/router';
import lodash from 'lodash';
import { async, defer, of } from 'rxjs';
import { delay } from 'rxjs/operators';

/**
 * 页面实体
 */
class Entity {
    readonly ID: WTM_EntitiesField = {
        name: ['Entity', 'ID'],
        label: EnumLocaleLabel.ID,
    }
    readonly PageName: WTM_EntitiesField = {
        name: ['Entity', 'PageName'],
        label: EnumLocaleLabel.Name,
        rules: [{ required: true }],
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
    readonly FolderOnly: WTM_EntitiesField = {
        name: ['Entity', 'FolderOnly'],
        label: EnumLocaleLabel.FolderOnly,
        valueType: WTM_ValueType.switch
    }
    readonly ShowOnMenu: WTM_EntitiesField = {
        name: ['Entity', 'ShowOnMenu'],
        label: EnumLocaleLabel.ShowOnMenu,
        valueType: WTM_ValueType.switch
    }
    readonly IsPublic: WTM_EntitiesField = {
        name: ['Entity', 'IsPublic'],
        label: EnumLocaleLabel.IsPublic,
        valueType: WTM_ValueType.switch
    }
    readonly DisplayOrder: WTM_EntitiesField = {
        name: ['Entity', 'DisplayOrder'],
        label: EnumLocaleLabel.DisplayOrder,
        rules: [{ required: true }],
    }
    readonly IsInside: WTM_EntitiesField = {
        name: ['Entity', 'IsInside'],
        label: EnumLocaleLabel.IsInside,
        valueType: WTM_ValueType.radio,
        request: async (formState) => {
            return [
                { label: $i18n.t(EnumLocaleLabel.IsInside_0), value: true },
                { label: $i18n.t(EnumLocaleLabel.IsInside_1), value: false }
            ]
        },
        rules: [{ required: true, type: 'boolean' }],
    }
    readonly Url: WTM_EntitiesField = {
        name: ['Entity', 'Url'],
        label: EnumLocaleLabel.Url,
    }
    readonly Icon: WTM_EntitiesField = {
        name: ['Entity', 'Icon'],
        label: EnumLocaleLabel.Icon,
        valueType: WTM_ValueType.icons,
    }
    readonly ParentId: WTM_EntitiesField = {
        name: ['Entity', 'ParentId'],
        label: EnumLocaleLabel.ParentId,
        valueType: WTM_ValueType.select,
        request: async () => FieldRequest("/api/_FrameworkMenu/GetFolders")
    }

}
export const PageEntity = new Entity()
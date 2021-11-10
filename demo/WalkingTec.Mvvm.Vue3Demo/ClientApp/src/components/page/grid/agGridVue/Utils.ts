import {ComponentUtil} from 'ag-grid-community';

export const kebabProperty = (property: string) => {
    return property.replace(/([a-z])([A-Z])/g, '$1-$2').toLowerCase();
};

export const kebabNameToAttrEventName = (kebabName: string) => {
    // grid-ready for example would become onGrid-ready in Vue
    return `on${kebabName.charAt(0).toUpperCase()}${kebabName.substring(1, kebabName.length)}`
};

export interface Properties {
    [propertyName: string]: any;
}

export const getAgGridProperties = (): [Properties, Properties, { prop: string, event: string }] => {
    const props: Properties = {
        gridOptions: {
            default() {
                return {};
            },
        },
        autoParamsRefresh: false,
        componentDependencies: {
            default() {
                return []
            }
        },
        modules: {
            default() {
                return []
            }
        },
        rowDataModel: undefined
    };

    // for example, 'grid-ready' would become 'onGrid-ready': undefined
    // without this emitting events results in a warning
    // and adding 'grid-ready' (and variations of this to the emits option in AgGridVue doesn't help either)
    const eventNameAsProps = ComponentUtil.EVENTS.map(eventName => kebabNameToAttrEventName(kebabProperty(eventName)));
    eventNameAsProps.reduce((accumulator, eventName) => {
        accumulator[eventName] = undefined
        return accumulator;
    }, props);

    const watch: Properties = {
        rowDataModel(currentValue: any, previousValue: any) {
            this.processChanges('rowData', currentValue, previousValue);
        },
    };

    ComponentUtil.ALL_PROPERTIES.forEach((propertyName) => {
        props[propertyName] = {};

        watch[propertyName] = function (currentValue: any, previousValue: any) {
            this.processChanges(propertyName, currentValue, previousValue);
        };
    });

    const model: { prop: string, event: string } = {
        prop: 'rowDataModel',
        event: 'data-model-changed',
    };

    return [props, watch, model];
};
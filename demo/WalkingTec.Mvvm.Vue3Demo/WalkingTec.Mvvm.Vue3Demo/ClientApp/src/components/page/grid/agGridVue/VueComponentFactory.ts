import { createApp, defineComponent } from 'vue';
import { Vue } from 'vue-class-component';
import lodash from 'lodash';
import { AgGridVue } from './AgGridVue';
// import { AgGridVue } from "ag-grid-vue3";
import client from '@/client/use'
import components from '@/components/use'
import router from '@/router'
export class VueComponentFactory {

    private static getComponentDefinition(component: any, parent: AgGridVue) {
        // vue-class-component 组件会注入一个 __c
        component = lodash.get(component, '__c', component)
        lodash.update(component, '__props', old => {
            lodash.remove(old, { params: undefined })
            return old
        })
        let componentDefinition: any;

        // when referencing components by name - ie: cellRendererFramework: 'MyComponent'
        if (typeof component === 'string') {
            // look up the definition in Vue
            componentDefinition = this.searchForComponentInstance(parent, component);

            // it's probably an SFC, but if it has template attribute it's probably
            // an inline/non-sfc component (ie an object a template property)
            if (componentDefinition.template) {
                // inline / non sfc component
                componentDefinition = { ...defineComponent(componentDefinition) };
            } else {
                // SFC
                componentDefinition = { extends: defineComponent(componentDefinition) };
            }
        } else {
            componentDefinition = { extends: component }
        }
        if (!componentDefinition) {
            console.error(`Could not find component with name of ${component}. Is it in Vue.components?`);
        }
        return componentDefinition;
    }

    private static createComponentParams(params: any, parent: AgGridVue) {
        const extendedParams = {
            params: Object.freeze(params),
            parent,
        };

        if (parent.componentDependencies) {
            parent.componentDependencies.forEach((dependency) =>
                (extendedParams as any)[dependency] = (parent as any)[dependency],
            );
        }

        return extendedParams;
    }

    public static createAndMountComponent(component: any, params: any, parent: AgGridVue) {
        const componentDefinition = VueComponentFactory.getComponentDefinition(component, parent);
        if (!componentDefinition) {
            return;
        }

        const componentParams = VueComponentFactory.createComponentParams(params, parent);

        // the inner defineComponent allows us to re-declare the component, with the outer one allowing us to
        // provide the grid's params and capture the resulting component instance
        let componentInstance: any = null;
        const extendedComponentDefinition = defineComponent({
            ...componentDefinition,
            data: () => componentParams,
            created() { // note: function - don't use arrow functions here (for the correct "this" to be used)
                componentInstance = (this as any).$root;
            }
        });

        // with vue 3 we need to provide a container to mount into (not necessary in vue 2), so create a wrapper div here
        const container = document.createElement('div');
        const mountedComponent = createApp(extendedComponentDefinition)
        mountedComponent
            .use(client)
            .use(components)
            .use(router.Router)
            .mount(container);

        // note that the component creation is synchronous so that componentInstance is set by this point
        return { mountedComponent, componentInstance };
    }

    private static searchForComponentInstance(parent: AgGridVue, component: any, maxDepth = 10) {
        let componentInstance: any = null;

        let currentParent: Vue<any> = parent.$parent;
        let depth = 0;
        while (!componentInstance &&
            currentParent &&
            currentParent.$options &&
            (++depth < maxDepth)) {
            componentInstance = (currentParent as any).$options.components![component as any];
            currentParent = currentParent.$parent;
        }

        if (!componentInstance) {
            console.error(`Could not find component with name of ${component}. Is it in Vue.components?`);
            return null;
        }
        return componentInstance;
    }
}
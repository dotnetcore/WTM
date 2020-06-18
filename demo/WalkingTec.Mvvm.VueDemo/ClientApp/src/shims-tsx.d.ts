// jsx
import Vue, { VNode } from "vue";
import * as lodash from "lodash";
declare global {
    const _: typeof lodash;
    namespace JSX {
        // tslint:disable no-empty-interface
        interface Element extends VNode {}
        // tslint:disable no-empty-interface
        interface ElementClass extends Vue {}
        interface IntrinsicElements {
            [elem: string]: any;
        }
    }
}

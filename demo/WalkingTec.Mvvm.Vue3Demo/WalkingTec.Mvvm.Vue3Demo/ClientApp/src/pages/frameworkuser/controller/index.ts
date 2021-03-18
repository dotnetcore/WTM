import * as WTM from "@/client";
export * from './entity';
export class PageController extends WTM.ControllerBasics {
    constructor() {
        super(null)
        this.onReset({
            key: "ITCode",
        })
    }
}
export default new PageController()
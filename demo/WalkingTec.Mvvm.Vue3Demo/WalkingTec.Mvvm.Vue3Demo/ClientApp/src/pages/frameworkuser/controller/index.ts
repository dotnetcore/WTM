import { observable, action } from "mobx";
import * as WTM from "@/client";
export * from './entity'
class PageController extends WTM.ControllerBasics {
    constructor() {
        super(null)
        this.onReset({
            key: "ITCode",
        })
    }
}
export default new PageController()
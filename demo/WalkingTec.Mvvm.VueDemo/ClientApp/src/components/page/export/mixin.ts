import { Component, Vue } from "vue-property-decorator";
import { Action, Mutation, State } from "vuex-class";
@Component
export default class ExportExcel extends Vue {
    @State("session")
    session;
    @State("progress")
    progress;
    @State("isFinish")
    isFinish;
    @State("exceed")
    exceed;
    @State("exceedMsg")
    exceedMsg;
    @State("position")
    position;
    @State("downloadUrl")
    downloadUrl;
    @Mutation("setSession")
    setSession;
    @Mutation("setProgress")
    setProgress;
    @Mutation("setIsFinish")
    setIsFinish;
    @Mutation("setPosition")
    setPosition;
    @Mutation("setExceed")
    setExceed;
    @Mutation("setExceedMsg")
    setExceedMsg;
    @Action("getExportInfo") getExportInfo;
    @Action("getProgress") getProgress;
}

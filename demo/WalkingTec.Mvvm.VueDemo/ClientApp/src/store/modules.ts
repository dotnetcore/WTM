import Vue from "vue";
import Vuex from "vuex";
import { IAppState } from "./modules/app";
import { IUserState } from "./modules/user";
import { ITagsViewState } from "./modules/tags-view";
import { IErrorLogState } from "./modules/error-log";
import { IRoutesModule } from "./modules/routes";
import { ISettingsState } from "./modules/settings";
Vue.use(Vuex);

export interface IRootState {
    app: IAppState;
    user: IUserState;
    tagsView: ITagsViewState;
    errorLog: IErrorLogState;
    permission: IRoutesModule;
    settings: ISettingsState;
}

export default new Vuex.Store<IRootState>({});

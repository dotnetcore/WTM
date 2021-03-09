import Vue from "vue";
import Vuex from "vuex";
import { IAppState } from "./app";
import { IUserState } from "./user";
import { ITagsViewState } from "./tags-view";
import { IErrorLogState } from "./error-log";
import { IRoutesModule } from "./routes";
import { ISettingsState } from "./settings";
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

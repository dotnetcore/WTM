import {
  VuexModule,
  Module,
  Mutation,
  Action,
  getModule
} from "vuex-module-decorators";
import { Route } from "vue-router";
import store from "@/store/modules/index";

export interface ITagView extends Partial<Route> {
  title?: string;
}

export interface ITagsViewState {
  visitedViews: ITagView[];
  cachedViews: (string | undefined)[];
}

@Module({ dynamic: true, store, name: "tagsView" })
class TagsView extends VuexModule implements ITagsViewState {
  public visitedViews: ITagView[] = [];
  public cachedViews: (string | undefined)[] = [];

  @Mutation
  private ADD_VISITED_VIEW(view: ITagView) {
    if (this.visitedViews.some(v => v.path === view.path)) return;
    this.visitedViews.push(
      Object.assign({}, view, {
        title: view.meta.title || "no-name"
      })
    );
  }

  @Mutation
  private ADD_CACHED_VIEW(view: ITagView) {
    let cachedPath = "";
    if (view.path && _.startsWith(view.path, "/")) {
      cachedPath = view.path.substr(1);
    }
    if (this.cachedViews.includes(cachedPath)) return;
    if (!view.meta.noCache) {
      this.cachedViews.push(cachedPath);
    }
  }

  @Mutation
  private DEL_VISITED_VIEW(view: ITagView) {
    for (const [i, v] of Object.entries(this.visitedViews)) {
      if (v.path === view.path) {
        this.visitedViews.splice(_.parseInt(i), 1);
        break;
      }
    }
  }

  @Mutation
  private DEL_CACHED_VIEW(view: ITagView) {
    for (const [i, v] of Object.entries(this.cachedViews)) {
      const cachedPath = view.path ? view.path.substr(1) : view.name;
      if (v === cachedPath) {
        this.cachedViews.splice(_.parseInt(i), 1);
        break;
      }
    }
  }

  @Mutation
  private DEL_OTHERS_VISITED_VIEWS(view: ITagView) {
    this.visitedViews = this.visitedViews.filter(v => {
      return v.meta.affix || v.path === view.path;
    });
  }

  @Mutation
  private DEL_OTHERS_CACHED_VIEWS(view: ITagView) {
    for (const [i, v] of Object.entries(this.cachedViews)) {
      if (v === view.name) {
        this.cachedViews = this.cachedViews.slice(
          _.parseInt(i),
          _.parseInt(i) + 1
        );
        break;
      }
    }
  }

  @Mutation
  private DEL_ALL_VISITED_VIEWS() {
    // keep affix tags
    const affixTags = this.visitedViews.filter(tag => tag.meta.affix);
    this.visitedViews = affixTags;
  }

  @Mutation
  private DEL_ALL_CACHED_VIEWS() {
    this.cachedViews = [];
  }

  @Mutation
  private UPDATE_VISITED_VIEW(view: ITagView) {
    for (let v of this.visitedViews) {
      if (v.path === view.path) {
        v = Object.assign(v, view);
        break;
      }
    }
  }

  @Action
  public addView(view: ITagView) {
    this.ADD_VISITED_VIEW(view);
    this.ADD_CACHED_VIEW(view);
  }

  @Action
  public addVisitedView(view: ITagView) {
    this.ADD_VISITED_VIEW(view);
  }

  @Action
  public delView(view: ITagView) {
    this.DEL_VISITED_VIEW(view);
    this.DEL_CACHED_VIEW(view);
  }

  @Action
  public delCachedView(view: ITagView) {
    this.DEL_CACHED_VIEW(view);
  }

  @Action
  public delOthersViews(view: ITagView) {
    this.DEL_OTHERS_VISITED_VIEWS(view);
    this.DEL_OTHERS_CACHED_VIEWS(view);
  }

  @Action
  public delAllViews() {
    this.DEL_ALL_VISITED_VIEWS();
    this.DEL_ALL_CACHED_VIEWS();
  }

  @Action
  public delAllCachedViews() {
    this.DEL_ALL_CACHED_VIEWS();
  }

  @Action
  public updateVisitedView(view: ITagView) {
    this.UPDATE_VISITED_VIEW(view);
  }
}

export const TagsViewModule = getModule(TagsView);

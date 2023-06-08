// aside
declare type AsideState = {
	menuList: RouteRecordRaw[];
	clientWidth: number;
};

// columnsAside
declare type ColumnsAsideState<T = any> = {
	columnsAsideList: T[];
	liIndex: number;
	liOldIndex: null | number;
	liHoverIndex: null | number;
	liOldPath: null | string;
	difference: number;
	routeSplit: string[];
};

// navBars breadcrumb
declare type BreadcrumbState<T = any> = {
	breadcrumbList: T[];
	routeSplit: string[];
	routeSplitFirst: string;
	routeSplitIndex: number;
};

// navBars search
declare type SearchState<T = any> = {
	isShowSearch: boolean;
	menuQuery: string;
	tagsViewList: T[];
};

// navBars tagsView
declare type TagsViewState<T = any> = {
	routeActive: string | T;
	routePath: string | unknown;
	dropdown: {
		x: string | number;
		y: string | number;
	};
	sortable: T;
	tagsRefsIndex: number;
	tagsViewList: T[];
	tagsViewRoutesList: T[];
};

// navBars parent
declare type ParentViewState<T = any> = {
	refreshRouterViewKey: string;
	iframeRefreshKey: string;
	keepAliveNameList: string[];
	iframeList: T[];
};

// navBars link
declare type LinkViewState = {
	title: string;
	isLink: string;
};

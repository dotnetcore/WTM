import * as React from 'react';
import { AuthorizeDecorator } from 'store/system/authorize';
import Store from './store';
import Action from './views/action';
import Other from './views/other';
import Search from './views/search';
import Table from './views/table';
/**
 * 页面入口
 *  Action：页面动作
 *  Details：详情信息，添加，编辑，详情。
 *  Other：其他部件，导入导出
 *  Search：搜索参数
 *  Table：表格
 */
@AuthorizeDecorator({ PageStore: Store })
export default class App extends React.Component<any, any> {
  render() {
    return (
        <div className="app-page-actionlog" key="app-page-actionlog">
        <Search {...this.props} />
        <Action.pageAction {...this.props} />
        <Table {...this.props} />
        <Other {...this.props} />
      </div>
    );
  }
}

import * as React from 'react';
import Action from './views/action';
import Details from './views/details';
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
export default class App extends React.Component<any, any> {
  componentWillMount() {
    // 权限
  }
  render() {
    return (
        <div className="app-page-testmodel" key="app-page-testmodel">
        <Search {...this.props} />
        <Action.pageAction {...this.props} />
        <Table {...this.props} />
        <Details {...this.props} />
        <Other {...this.props} />
      </div>
    );
  }
}

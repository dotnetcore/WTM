import { Divider } from 'antd';
import * as React from 'react';
import Details from './views/details';
import Search from './views/search';
import Action from './views/action';
import Table from './views/table';


export default class App extends React.Component<any, any> {
  render() {
    return (
      <div className="app-table-content">
        <Search {...this.props} >
          <Action />
        </Search>
        <Divider />
        <Table {...this.props} />
        <Details {...this.props} />
      </div>
    );
  }
}

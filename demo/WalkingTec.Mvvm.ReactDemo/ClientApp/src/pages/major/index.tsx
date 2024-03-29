﻿import * as React from 'react';
import { AuthorizeDecorator } from 'store/system/authorize';
import Store from './store';
import Action from './views/action';
import Other from './views/other';
import Search from './views/search';
import Table from './views/table';

@AuthorizeDecorator({ PageStore: Store })
export default class App extends React.Component<any, any> {
  render() {
    return (
        <div className="app-page-major" key="app-page-major">
        <Search {...this.props} />
        <Action.pageAction {...this.props} />
        <Table {...this.props} />
        <Other {...this.props} />
      </div>
    );
  }
}

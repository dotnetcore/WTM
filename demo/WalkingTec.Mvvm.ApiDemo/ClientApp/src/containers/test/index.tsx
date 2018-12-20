import * as React from 'react';
import Header from './components/Header';
import Edit from './components/Edit';
import Body from './components/Body';
import Store from './store';
import "./style.less";

export default class App extends React.Component<any, any> {
  render() {
    return (
      <div className="app-table-content">
        <Header Store={Store} />
        <Edit Store={Store} />
        <Body Store={Store} />
      </div>
    );
  }
}

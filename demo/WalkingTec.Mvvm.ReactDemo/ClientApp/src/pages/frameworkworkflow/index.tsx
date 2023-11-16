import * as React from 'react';
import Store from 'store/index';
import { AuthorizeDecorator } from 'store/system/authorize';


export default class App extends React.Component<any, any> {
  render() {
    return (
        <div key="frameworkworkflow">
            <iframe src={"http://localhost:5555/_workflow/inner?access_token=" + window.localStorage.getItem('__token')} frameborder="0" style="width:100%;height:100%" />
      </div>
    );
  }
}

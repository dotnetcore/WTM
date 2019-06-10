import * as React from 'react';
import { Observable } from 'rxjs';

import { DialogForm, DialogFormDes, DialogFormSubmit, FormItem, InfoShellLayout, DialogLoadData, } from 'components/dataView';
import { DesForm } from 'components/decorators';

import { WtmEditor } from 'components/form';
// @DialogFormDes({
//     onFormSubmit(values) {
//         return new Observable<boolean>(sub => {
//             sub.next(false);
//             sub.complete();
//         }).toPromise();
//     }
// })
@DesForm
export default class App extends React.Component<any, any> {
    render() {
        console.log(this)
        return (
            <div>
                <WtmEditor />
            </div>
        );
    }
}

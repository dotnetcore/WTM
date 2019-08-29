import { FormItem, InfoShellLayout } from 'components/dataView';
import { DesForm } from 'components/decorators';
import { WtmEditor, WtmDatePicker, WtmRangePicker, WtmSelect } from 'components/form';
import Request from 'utils/Request';
import * as React from 'react';
import { Button, DatePicker } from 'antd';
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
    models: WTM.FormItem = {
        "WtmEditor": {
            label: "富文本",
            rules: [],
            formItem: <WtmEditor />
        },
        "WtmDatePicker": {
            label: "WTM日历",
            rules: [],
            formItem: <WtmDatePicker />
        },
        "WtmDatePicker2": {
            label: "WTM日历时间",
            rules: [],
            formItem: <WtmDatePicker showTime />
        },
        "DatePicker": {
            label: "Antd日历",//返回 Moment 对象
            rules: [],
            formItem: <DatePicker />
        },
        "DatePicker2": {
            label: "Antd日历时间",//返回 Moment 对象
            rules: [],
            formItem: <DatePicker showTime />
        },
        "WtmRangePicker": {
            label: "WTM区间",
            rules: [],
            formItem: <WtmRangePicker />
        },
        "WtmRangePicker2": {
            label: "WTM区间 时间",
            rules: [],
            formItem: <WtmRangePicker showTime />
        },
        "Select": {
            label: "Select",
            rules: [],
            formItem: <WtmSelect dataSource={Request.cache({ url: "/api/_DataPrivilege/GetPrivileges" })} />
        },
        "Select2": {
            label: "Select",
            rules: [],
            formItem: <WtmSelect mode="multiple" dataSource={Request.cache({ url: "/api/_DataPrivilege/GetPrivileges" })} />
        },
    }
    onSubmit() {
        this.props.form.validateFields((err, values) => {
            console.log("TCL: App -> onSubmit -> values", values)
        });
    }
    render() {
        const props = {
            ...this.props,
            models: this.models,
        }
        return (
            <div>
                <div>
                    <Button onClick={this.onSubmit.bind(this)}>打印数据（控制台）</Button>
                </div>
                <InfoShellLayout>
                    <FormItem fieId="WtmDatePicker" {...props} value="2018-01-01" />
                    <FormItem fieId="WtmDatePicker2" {...props} />
                    <FormItem fieId="DatePicker" {...props} />
                    <FormItem fieId="DatePicker2" {...props} />
                    <FormItem fieId="WtmRangePicker" {...props} />
                    <FormItem fieId="WtmRangePicker2" {...props} />
                    <FormItem fieId="Select" {...props} value={'School'} />
                    <FormItem fieId="Select2" {...props} value={['School']} />
                    <FormItem fieId="WtmEditor" {...props} layout="row" />
                </InfoShellLayout>
            </div>
        );
    }
}

import { DataViewSearch } from 'components/dataView/header/search';
import { DesForm } from 'components/decorators';
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from '../store';
import { FormItem } from './models'; //模型
@DesForm
@observer
export default class extends React.Component<any, any> {
    render() {
        return <DataViewSearch
            // columnCount={4} 默认全局
            // onReset={() => { }} 覆盖默认方法
            // onSubmit={() => { }} 覆盖默认方法
            Store={Store}
            form={this.props.form}
        >
            <FormItem {...this.props} fieId="ITCode" type="searchParams" />
            <FormItem {...this.props} fieId="Name" type="searchParams" />
        </DataViewSearch>
    }
}

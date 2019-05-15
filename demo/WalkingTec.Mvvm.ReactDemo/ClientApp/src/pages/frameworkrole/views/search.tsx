import { FormItem } from 'components/dataView';
import { DataViewSearch } from 'components/dataView/header/search';
import { DesForm } from 'components/decorators';
import { toJS } from 'mobx';
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from '../store';
import Models from './models'; 

@DesForm
@observer
export default class extends React.Component<any, any> {
    // 创建模型
    models = Models.searchModels(this.props);
    render() {
        // item 的 props
        const props = {
            ...this.props,
            models: this.models,
            defaultValues: toJS(Store.DataSource.searchParams)
        }
        return <DataViewSearch
            // columnCount={4} 默认全局
            // onReset={() => { }} 覆盖默认方法
            // onSubmit={() => { }} 覆盖默认方法
            Store={Store}
            form={this.props.form}
        >
            {Models.renderModels(props)}
        </DataViewSearch>
    }
}
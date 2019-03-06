import { DataViewSearch } from 'components/dataView/header/search';
import { DesForm } from 'components/decorators';
import { toJS } from 'mobx';
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from '../store';
import Models from './models'; //模型
@DesForm
@observer
export default class extends React.Component<any, any> {
    // 创建模型
    models = Models.searchModels(this.props);
    render() {
        // item 的 props
        const props = {
            ...this.props,
            // 模型
            models: this.models,
            // 默认值  
            defaultValues: toJS(Store.searchParams)
        }
        return <DataViewSearch
            // columnCount={4} 默认全局
            // onReset={() => { }} 覆盖默认方法
            // onSubmit={() => { }} 覆盖默认方法
            Store={Store}
            form={this.props.form}
        >
            {/* <FormItem {...props} fieId="ITCode" />
            <FormItem {...props} fieId="Name" /> */}
            {Models.renderModels(props)}
        </DataViewSearch>
    }
}

/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-02-24 17:06:42
 * @modify date 2019-02-24 17:06:42
 * @desc [description]
 */
import { Select, notification, Spin, Table, Button, ConfigProvider, Divider, Form, Popconfirm } from 'antd';
import { SelectProps } from 'antd/lib/select';
import { DesError, DesForm } from 'components/decorators'; //错误
import lodash from 'lodash';
import { Debounce } from 'lodash-decorators';
import React from 'react';
import { Observable } from 'rxjs';
import { WrappedFormUtils } from 'antd/lib/form/Form';
import { DialogForm, DialogFormDes, FormItem, InfoShellLayout } from 'components/dataView';
import { ColumnProps } from 'antd/lib/table';
import { Help } from 'utils/Help';
import { observable, action, runInAction, toJS } from 'mobx';
import { observer } from 'mobx-react';
import './style.less';
const EditableContext = React.createContext(null);
@DesForm
class EditableRow extends React.Component<any, any> {
    FieldsValue = {};
    isEdit = false;
    componentDidUpdate() {
        const {
            form,
            models,
            handleUpdate,
            ...restProps
        } = this.props;
        const FieldsValue = form.getFieldsValue();
        if (!lodash.isEqual(this.FieldsValue, FieldsValue)) {
            this.FieldsValue = FieldsValue;
            this.isEdit = true;
        }
    }
    componentDidMount() {
        const {
            form,
            models,
            handleUpdate,
            ...restProps
        } = this.props;
        const FieldsValue = form.getFieldsValue();
        if (!lodash.isEqual(this.FieldsValue, FieldsValue)) {
            this.FieldsValue = FieldsValue;
        }
    }
    componentWillUnmount() {
        const {
            form,
            handleUpdate,
            ...restProps
        } = this.props;
        // const FieldsValue = form.getFieldsValue();
        if (this.isEdit) {
            this.isEdit = false;
            // console.log(lodash.get(restProps, 'data-row-key'), this.FieldsValue)
            handleUpdate(lodash.get(restProps, 'data-row-key'), this.FieldsValue);
        }
    }
    render() {
        const {
            form,
            models,
            handleUpdate,
            ...restProps
        } = this.props;
        return (
            <EditableContext.Provider value={{
                form: form,
                models: models,
            }}>
                {/* <Popconfirm placement="top" title={'保存确认'} okText="Yes" cancelText="No"> */}
                <tr {...restProps} />
                {/* </Popconfirm> */}
            </EditableContext.Provider>
        );
    }
}
class EditableCell extends React.Component<any, any>{
    renderForm(props) {
        // console.log(toJS(this.props.record))
        return <FormItem fieId={this.props.dataIndex} layout="row-hidden-label"  {...props} defaultValues={toJS(this.props.record)} />
    }
    render() {
        const {
            dataIndex,
            title,
            record,
            index,
            width,
            ...restProps
        } = this.props;
        let style: any = {};
        if (width) {
            style.width = width - 36;
        }
        return (
            <td {...restProps}>
                <div style={style}>
                    {record ? <EditableContext.Consumer>
                        {this.renderForm.bind(this)}
                    </EditableContext.Consumer> : restProps.children}
                </div>
            </td>
        );
    }
}

@observer
@DesError
export class EditTable extends React.Component<IAppProps, any> {
    @observable dataSource = [];
    @observable columns = [];
    @action
    onUpdateDataSource(dataSource) {
        this.dataSource = dataSource;
        this.handleChange();
    }
    Unmount = false
    componentWillUnmount() {
        this.Unmount = true;
    }
    componentDidUpdate() {
    }
    componentDidMount() {
        const columns = this.createColumns();
        columns.push({
            title: "操作",
            width: 70,
            // fixed: "right",
            render: (text, record, index) => <div><a onClick={this.handleRemove.bind(this, record)}>删除</a></div>
        });
        // this.setState({ columns })
        runInAction(() => this.columns = columns)
    }
    @Debounce(100)
    handleChange() {
        this.props.onChange(toJS(this.dataSource));
    }
    handleAdd() {
        const dataSource = [...this.dataSource];
        dataSource.push({
            __key: Help.GUID(),
            ...lodash.mapValues(this.props.models, (value) => {
                return undefined
            })
        });
        this.onUpdateDataSource(dataSource)
    }
    handleSave() {
        this.onUpdateDataSource(toJS(this.dataSource))
    }
    handleRemove(record) {
        const dataSource = [...this.dataSource];
        lodash.remove(dataSource, ['__key', record.__key]);
        this.onUpdateDataSource(dataSource);
    }
    handleUpdate = (rowKey, data) => {
        const dataSource = [...this.dataSource];
        const index = lodash.findIndex(dataSource, ['__key', rowKey]);
        lodash.update(dataSource, `[${index}]`, value => {
            return {
                ...value,
                ...data,
            }
        });
        this.onUpdateDataSource(dataSource);
    }
    createColumns(): ColumnProps<any>[] {
        return lodash.map(this.props.models, (value, key) => {
            return {
                key,
                dataIndex: key,
                title: value.label,
                width: 200,
                onCell: (record, rowIndex) => ({
                    record,
                    dataIndex: key,
                    title: value.label,
                    width: 200,
                    // handleUpdate: this.handleUpdate(key),
                })
                // render: (text, record, index) => <div>{value.formItem}</div>
            }
        })
    }
    render() {
        const columns = [...this.columns];
        const dataSource = [...this.dataSource];
        return (
            <ConfigProvider renderEmpty={() => <div>暂无数据</div>}>
                <Button onClick={this.handleAdd.bind(this)}>新建</Button>
                <Divider type="vertical" />
                <Button onClick={this.handleSave.bind(this)}>保存</Button>
                <Table
                    rowKey="__key"
                    size="middle"
                    bordered
                    components={{
                        body: {
                            row: (props) => <EditableRow {...props} models={this.props.models} handleUpdate={this.handleUpdate} />,
                            cell: EditableCell
                        }
                    }}
                    className="WtmEditTable"
                    columns={columns}
                    dataSource={dataSource}
                    pagination={false}
                    scroll={{
                        x: columns.reduce((accumulator, currentValue) => {
                            return Math.ceil(accumulator + (currentValue.width || 0))
                        }, 0)
                    }}
                />

            </ConfigProvider>
        );
    }
}

interface IAppProps {
    /** 模型 */
    models: WTM.FormItem;
    onChange?: (value) => void;
}
@DesError
export class WtmEditTable extends React.Component<IAppProps, any> {
    static wtmType = "EditTable";
    Unmount = false
    componentWillUnmount() {
        this.Unmount = true;
    }
    componentDidUpdate() {
    }
    componentDidMount() {
    }
    handleChange = (dataSource) => {
        console.log(dataSource)
        this.props.onChange(dataSource);
    }
    render() {
        return <EditTable models={this.props.models} onChange={this.handleChange} />
    }
}
export default WtmEditTable
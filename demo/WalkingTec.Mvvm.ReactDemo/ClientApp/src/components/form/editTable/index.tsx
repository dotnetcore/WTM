/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-02-24 17:06:42
 * @modify date 2019-02-24 17:06:42
 * @desc [description]
 */
import { Select, notification, Spin, Table, Button, ConfigProvider, Divider, Form, Popconfirm, Alert } from 'antd';
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
let EditableContext = React.createContext(null);
@DesForm
class EditableRow extends React.Component<any, any> {
    constructor(props) {
        super(props);
    }
    FieldsValue = {};
    rowKey = lodash.get(this.props, 'data-row-key');
    onEdit(isEdit = true) {
        const {
            handleUpdate,
            ...restProps
        } = this.props;
        this.props.form.validateFields((err, values) => {
            if (!lodash.isEqual(this.FieldsValue, values)) {
                this.FieldsValue = values;
                handleUpdate(this.rowKey, this.FieldsValue);
            }
        })
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
        this.props.form.validateFields()
        // console.log("componentDidMount")
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
                <tr {...restProps}
                    onMouseEnter={this.onEdit.bind(this, true)} onMouseLeave={this.onEdit.bind(this, false)} />
            </EditableContext.Provider>
        );
    }
}
class EditableCell extends React.Component<any, any>{
    constructor(props) {
        super(props)
    }
    renderForm(props) {
        return <FormItem
            fieId={this.props.dataIndex}
            layout="row-hidden-label"
            {...props}
            {...lodash.get(props, `models.${this.props.dataIndex}.formItemProps`)}
            defaultValues={toJS(this.props.record)}
        />
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
                {record ? <EditableContext.Consumer>
                    {this.renderForm.bind(this)}
                </EditableContext.Consumer> : restProps.children}
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
        if (this.props.deleteButton && !(this.props.disabled || this.props.display)) {
            columns.push({
                title: "操作",
                width: 70,
                // fixed: "right",
                render: (text, record, index) => <div><a onClick={this.handleRemove.bind(this, record)}>删除</a></div>
            });
        }
        // this.setState({ columns })
        runInAction(() => {
            this.columns = columns;
            if (lodash.isArray(this.props.value) && this.props.value.length > 0) {
                this.dataSource = this.props.value.map(x => {
                    // console.log(x, this.props.rowKey, lodash.get(x, this.props.rowKey))
                    return {
                        ...this.props.setValues,
                        ...x,
                        __key: lodash.get(x, this.props.rowKey)
                    }
                })
            }
        });
    }
    @Debounce(100)
    handleChange() {
        this.props.onChange(toJS(this.dataSource));
    }
    @Debounce(100)
    handleAdd() {
        const dataSource = [...this.dataSource];
        dataSource.push({
            ...this.props.setValues,
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
                width: 150,
                ...value.columnsProps,
                onCell: (record, rowIndex) => ({
                    record,
                    dataIndex: key,
                    title: value.label,
                    width: 150,
                    ...value.columnsProps,
                })
            }
        })
    }
    render() {
        const columns = [...this.columns];
        const dataSource = [...this.dataSource];
        const models = lodash.mapValues(this.props.models, value => {
            return {
                ...value,
                formItemProps: {
                    ...value.formItemProps,
                    disabled: !!this.props.disabled,
                    display: !!this.props.display,
                }
            }
        });
        return (
            <ConfigProvider renderEmpty={() => <div>暂无数据</div>}>
                {this.props.addButton && !(this.props.disabled || this.props.display) && <Button onClick={this.handleAdd.bind(this)}>新建</Button>}
                {/* <Divider type="vertical" />
                <Alert message="验证未通过行提交数据将忽略" type="warning" showIcon closable style={{ display: "inline-block" }} /> */}
                <Table
                    rowKey="__key"
                    bordered
                    components={{
                        body: {
                            row: (props) => <EditableRow {...props} models={models} handleUpdate={this.handleUpdate} />,
                            cell: EditableCell
                        }
                    }}
                    className="WtmEditTable"
                    style={{
                        display: dataSource.length <= 0 ? "none" : ""
                    }}
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
    /** 行 Key 值 默认 ID */
    rowKey?: string;
    setValues?: any;
    onChange?: (value) => void;
    value?: any;
    addButton?: boolean;
    deleteButton?: boolean;
    display?: boolean;
    disabled?: boolean;
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
        console.log(this.props)
    }
    handleChange = (dataSource) => {
        this.props.onChange(dataSource);
    }
    render() {
        return <EditTable
            rowKey={lodash.get(this.props, "rowKey", "ID")}
            value={this.props.value}
            models={this.props.models}
            onChange={this.handleChange}
            setValues={this.props.setValues}
            disabled={lodash.get(this.props, "disabled", false)}
            display={lodash.get(this.props, "display", false)}
            addButton={lodash.get(this.props, "addButton", true)}
            deleteButton={lodash.get(this.props, "deleteButton", true)}
        />
    }
}
export default WtmEditTable
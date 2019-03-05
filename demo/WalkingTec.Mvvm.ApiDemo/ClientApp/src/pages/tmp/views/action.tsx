import { Button, Divider, Dropdown, Menu, message, Modal, Popconfirm, Row } from 'antd';
import { DialogForm, Visible } from 'components/dataView';
import { DesError } from 'components/decorators';
import lodash from 'lodash';
import { observer } from 'mobx-react';
import * as React from 'react';
import { EnumAuthorizeActions, onAuthorizeActions } from 'store/system/authorize';
import Store from '../store';
import { InsertForm, UpdateForm, InfoForm } from './details';
/**
 * 动作事件
 */
export const ActionEvents = {
    onTest(event: React.MouseEvent<HTMLButtonElement, MouseEvent>) {
        event.preventDefault();
        event.stopPropagation();
    },
    /**
     * 添加
     */
    onAdd() {
        Store.onModalShow({}, "Insert")
    },
    /**
     * 详情
     * @param data 
     */
    onInfo(data) {
        Store.onModalShow(data, "Info")
    },
    /**
     * 修改
     * @param data 
     */
    onUpdate(data) {
        Store.onModalShow(data, "Update")
    },
    /**
     * 导入
     */
    onImport() {
        Store.onPageState("visiblePort", true)
    },
    /**
     * 导出
     */
    onExport() {
        Store.onExport()
    },
    /**
     * 批量导出
     */
    onExportIds() {
        Store.onExportIds()
    },
    /**
     * 删除
     * @param data 
     */
    onDelete(data) {
        Store.onDelete([lodash.get(data, Store.IdKey)])
    },
    /**
    * 删除
    */
    onDeleteList() {
        const length = Store.selectedRowKeys.length
        if (length > 0) {
            Modal.confirm({
                title: `确定删除 ${length} 条数据?`,
                onOk: async () => {
                    Store.onDelete([...Store.selectedRowKeys])
                },
                onCancel() { },
            });
        }
    },
    /**
     * 多选修改
     */
    onUpdateList() {
        return lodash.find(Store.dataSource.Data, ['key', lodash.head(Store.selectedRowKeys)])
    },
    onGetId() {
        return lodash.get(lodash.find(Store.dataSource.Data, ['key', lodash.head(Store.selectedRowKeys)]), Store.IdKey);
    }
}
/**
 * 表格 所有 动作
 */
@DesError
@observer
class PageAction extends React.Component<any, any> {
    render() {
        const { selectedRowKeys, Actions } = Store;
        const deletelength = selectedRowKeys.length;
        const disabled = deletelength < 1;
        return (
            <Row className="data-view-page-action">
                <Visible visible={onAuthorizeActions(Store, EnumAuthorizeActions.insert)}>
                    <DialogForm
                        title="新建"
                        icon="plus"
                    // onFormSubmit={InsertForm.onFormSubmit}
                    >
                        <InsertForm />
                    </DialogForm>
                </Visible>
                <Visible visible={onAuthorizeActions(Store, EnumAuthorizeActions.update)}>
                    <Divider type="vertical" />
                    <DialogForm
                        title="修改"
                        icon="edit"
                        disabled={deletelength != 1}
                    >
                        <UpdateForm Details={ActionEvents.onUpdateList} />
                    </DialogForm>
                </Visible>
                <Visible visible={onAuthorizeActions(Store, EnumAuthorizeActions.delete)}>
                    <Divider type="vertical" />
                    <Button icon="delete" onClick={ActionEvents.onDeleteList} disabled={disabled}> 删除  </Button>
                </Visible>
                <Visible visible={onAuthorizeActions(Store, EnumAuthorizeActions.import)}>
                    <Divider type="vertical" />
                    <Button icon="folder-add" onClick={ActionEvents.onImport}>导入</Button>
                </Visible>
                <Visible visible={onAuthorizeActions(Store, EnumAuthorizeActions.export)}>
                    <Divider type="vertical" />
                    <Dropdown overlay={<Menu>
                        <Menu.Item>
                            <a onClick={ActionEvents.onExport}>导出全部</a>
                        </Menu.Item>
                        <Menu.Item disabled={disabled}>
                            <a onClick={ActionEvents.onExportIds}>导出勾选</a>
                        </Menu.Item>
                    </Menu>}>
                        <Button icon="download" >导出</Button>
                    </Dropdown>
                </Visible>

            </Row>
        );
    }
}
/**
 * 表格 行 动作
 */
@DesError
@observer
class RowAction extends React.Component<{
    /** 数据详情 */
    data: any;
    [key: string]: any;
}, any> {
    render() {
        const { Actions } = Store;
        const { data } = this.props
        return (
            <Row className="data-view-row-action">
                <DialogForm
                    title="详情"
                    showSubmit={false}
                    type="a"
                >
                    <InfoForm Details={data} />
                </DialogForm>
                <Visible visible={onAuthorizeActions(Store, EnumAuthorizeActions.update)}>
                    <Divider type="vertical" />
                    <DialogForm
                        title="修改"
                        type="a"
                    >
                        <UpdateForm Details={data} />
                    </DialogForm>
                </Visible>
                <Visible visible={onAuthorizeActions(Store, EnumAuthorizeActions.delete)}>
                    <Divider type="vertical" />
                    <Popconfirm title="确定删除?" onConfirm={() => { ActionEvents.onDelete(data) }} >
                        <a >删除</a>
                    </Popconfirm>
                </Visible>
            </Row>
        );
    }
}
export default {
    /**
     * 页面动作
     */
    pageAction: PageAction,
    /**
     * 数据行动作
     */
    rowAction: RowAction
}
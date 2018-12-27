import { Button, Divider, Dropdown, Menu, message, Popconfirm, Row, Icon } from 'antd';
import Visible from 'components/dataView/help/visible';
import lodash from 'lodash';
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from '../store';
@observer
export default class IApp extends React.Component<any, any> {
    Store = Store;
    onAdd() {
        Store.onModalShow({})
    }
    onImport() {
        this.Store.onPageState("visiblePort", true)
    }
    onExport() {
        this.Store.onExport()
    }
    onExportIds() {
        this.Store.onExportIds()
    }
    /**
   * 多选删除
   */
    async onDelete() {
        const params = this.Store.dataSource.Data.filter(x => this.Store.selectedRowKeys.some(y => y == x.key));
        // await this.Store.onDelete(params)
    }
    /**
     * 多选修改
     */
    async onUpdate() {
        if (this.Store.selectedRowKeys.length == 1) {
            this.Store.onModalShow(lodash.find(this.Store.dataSource.Data, ['key', lodash.head(this.Store.selectedRowKeys)]))
        } else {
            message.warn("请选择一条数据")
        }
    }

    render() {
        const { selectedRowKeys, Actions } = this.Store;
        const deletelength = selectedRowKeys.length;
        const disabled = deletelength < 1;
        return (
            <Row>
                <Visible visible={Actions.insert}>
                    <Button icon="plus" onClick={this.onAdd.bind(this)} >新建</Button>
                </Visible>
                <Visible visible={Actions.update}>
                    <Divider type="vertical" />
                    <Button icon="edit" onClick={this.onUpdate.bind(this)} disabled={disabled}>修改</Button>
                </Visible>
                <Visible visible={Actions.delete}>
                    <Divider type="vertical" />
                    <Popconfirm placement="right" title={`确定删除 ${deletelength}条 数据？`}
                        onConfirm={this.onDelete.bind(this)}
                        okText="确定" cancelText="取消">
                        <Button icon="delete" disabled={disabled}> 删除  </Button>
                    </Popconfirm>
                </Visible>
                <Visible visible={Actions.import}>
                    <Divider type="vertical" />
                    <Button icon="folder-add" onClick={this.onImport.bind(this)}>导入</Button>
                </Visible>
                <Divider type="vertical" />
                {/* <Button icon="download" onClick={this.onExport.bind(this)}>导出</Button> */}
                <Dropdown trigger={["click"]} overlay={<Menu>
                    <Menu.Item>
                        <a onClick={this.onExport.bind(this)}>导出全部</a>
                    </Menu.Item>
                    <Menu.Item disabled={disabled}>
                        <a onClick={this.onExportIds.bind(this)}>导出勾选</a>
                    </Menu.Item>
                </Menu>}>
                    <Button icon="download" >导出</Button>
                </Dropdown>
                <Divider type="vertical" />
                <Dropdown overlay={<Menu>
                    <Menu.Item>
                        <a >按钮</a>
                    </Menu.Item>
                </Menu>}
                    placement="bottomCenter">
                    <Button icon="ellipsis" />
                </Dropdown>
            </Row>
        );
    }
}

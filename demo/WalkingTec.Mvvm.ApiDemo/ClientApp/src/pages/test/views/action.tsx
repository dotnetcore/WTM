import { Button, Divider, Dropdown, Icon, Menu, message, Modal, Popconfirm, Row } from 'antd';
import Dragger from 'antd/lib/upload/Dragger';
import Visible from 'components/dataView/help/visible';
import lodash from 'lodash';
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from '../store';
/**
 * 表格 所有 动作
 */
@observer
export default class IApp extends React.Component<any, any> {
    onAdd() {
        Store.onModalShow({}, "Insert")
    }
    onImport() {
        Store.onPageState("visiblePort", true)
    }
    onExport() {
        Store.onExport()
    }
    onExportIds() {
        Store.onExportIds()
    }
    /**
     * 多选删除
     */
    async onDelete() {
        const params = Store.dataSource.Data.filter(x => Store.selectedRowKeys.some(y => y == x.key));
        // await Store.onDelete(params)
    }
    /**
     * 多选修改
     */
    async onUpdate() {
        if (Store.selectedRowKeys.length == 1) {
            Store.onModalShow(lodash.find(Store.dataSource.Data, ['key', lodash.head(Store.selectedRowKeys)]), "Update")
        } else {
            message.warn("请选择一条数据")
        }
    }

    render() {
        const { selectedRowKeys, Actions } = Store;
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
                    {disabled ?
                        <Button icon="delete" disabled={disabled}> 删除  </Button> :
                        <Popconfirm placement="right" title={`确定删除 ${deletelength}条 数据？`}
                            onConfirm={this.onDelete.bind(this)}
                            okText="确定" cancelText="取消">
                            <Button icon="delete"> 删除  </Button>
                        </Popconfirm>}
                </Visible>
                <Visible visible={Actions.import}>
                    <Divider type="vertical" />
                    <Button icon="folder-add" onClick={this.onImport.bind(this)}>导入</Button>
                </Visible>
                <Divider type="vertical" />
                <Dropdown overlay={<Menu>
                    <Menu.Item>
                        <a onClick={this.onExport.bind(this)}>导出全部</a>
                    </Menu.Item>
                    <Menu.Item disabled={disabled}>
                        <a onClick={this.onExportIds.bind(this)}>导出勾选</a>
                    </Menu.Item>
                </Menu>}>
                    <Button icon="download" >导出</Button>
                </Dropdown>
                <PortComponent />
            </Row>
        );
    }
}
/**
 * 导入导出
 */
@observer
class PortComponent extends React.Component<any, any> {
    render() {
        return (
            <Modal
                title="导入"
                centered
                visible={Store.pageState.visiblePort}
                destroyOnClose={true}
                width={600}
                cancelText="取消"
                footer={null}
                onCancel={() => { Store.onPageState("visiblePort", false) }}
            >
                <div >
                    <div >
                        导入说明：请下载模版，然后在把信息输入到模版中   <Divider type="vertical" /> <Button icon="download" onClick={() => { Store.onTemplate() }}>下载模板</Button>
                    </div>
                    <Divider style={{ margin: "5px 0" }} />
                    <Dragger {...Store.importConfig}>
                        <p className="ant-upload-drag-icon">
                            <Icon type="inbox" />
                        </p>
                        <p className="ant-upload-text">单击或拖动文件到该区域上载</p>
                    </Dragger>
                </div>
            </Modal>
        );
    }
}
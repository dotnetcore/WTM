/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-10 05:07:36
 * @modify date 2018-09-10 05:07:36
 * @desc [description]
*/
import { Alert, Button, Drawer, Icon, message, Tree } from 'antd';
import { toJS } from 'mobx';
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from 'store/index';
const TreeNode = Tree.TreeNode;
@observer
export default class IApp extends React.Component<any, any> {
    onSelect(selectedKeys, e) {
        console.log(selectedKeys);
    }
    onDragEnter = (info) => {

    }
    onDrop = (info) => {
        const dropKey = info.node.props.eventKey;
        const dragKey = info.dragNode.props.eventKey;
        const dropPos = info.node.props.pos.split('-');
        if (dropPos.length > 2) {
            return message.warning('只支持二级菜单');
        }
        if (dragKey == "system" || dropKey == "system") {
            return message.warning('系统设置不可更改');
        }
        const dropPosition = info.dropPosition - Number(dropPos[dropPos.length - 1]);
        // const dragNodesKeys = info.dragNodesKeys;
        const loop = (data, key, callback) => {
            data.forEach((item, index, arr) => {
                if (item.Key === key) {
                    return callback(item, index, arr);
                }
                if (item.Children) {
                    return loop(item.Children, key, callback);
                }
            });
        };
        const data = [...toJS(Store.Meun.subMenu)];
        let dragObj;
        loop(data, dragKey, (item, index, arr) => {
            arr.splice(index, 1);
            dragObj = item;
        });
        // debugger
        if (info.dropToGap) {
            let ar;
            let i;
            loop(data, dropKey, (item, index, arr) => {
                ar = arr;
                i = index;
            });
            if (dropPosition === -1) {
                ar.splice(i, 0, dragObj);
            } else {
                ar.splice(i + 1, 0, dragObj);
            }
        } else {
            loop(data, dropKey, (item) => {
                item.Children = item.Children || [];
                // where to insert 示例添加到尾部，可以是随意位置
                item.Children.push(dragObj);
            });
        }
        Store.Meun.setSubMenu(data);
    }

    render() {
        const TreeNodeConfig = {
            disableCheckbox: true,
            // selectable: false
        }
        return (
            <div>
                <Alert message="除了Home 和 system 之外 仅用于 开发配置 " type="info" showIcon />
                <Tree
                    showLine
                    showIcon
                    draggable={true}
                    defaultExpandedKeys={['0-0-0']}
                    onSelect={this.onSelect.bind(this)}
                    onDragEnter={this.onDragEnter}
                    onDrop={this.onDrop}
                >
                    {Store.Meun.subMenu.map((x, i) => <TreeNode {...TreeNodeConfig} title={x.Name} key={x.Key} icon={<Icon type={x.Icon} />} >
                        {x.Children && x.Children.map((y, yi) => <TreeNode {...TreeNodeConfig} title={y.Name} key={y.Key} icon={<Icon type={y.Icon} />} />)}
                    </TreeNode>)}
                </Tree>
                {/* <Button loading={Store.updateSubMenuLoading} onClick={() => { Store.updateSubMenu() }}>{Store.updateSubMenuLoading ? '等待编译...' : '保存'} </Button> */}
                <DrawerComponent />
            </div>
        );
    }
}

class DrawerComponent extends React.Component<any, any> {
    state = { visible: false, childrenDrawer: false };

    showDrawer = () => {
        this.setState({
            visible: true,
        });
    };

    onClose = () => {
        this.setState({
            visible: false,
        });
    };

    showChildrenDrawer = () => {
        this.setState({
            childrenDrawer: true,
        });
    };

    onChildrenDrawerClose = () => {
        this.setState({
            childrenDrawer: false,
        });
    };
    public render() {
        return (
            <div>
                <Button type="primary" onClick={this.showDrawer}>
                    保存菜单
        </Button>
                <Drawer
                    title="Multi-level drawer"
                    width={520}
                    closable={false}
                    onClose={this.onClose}
                    visible={this.state.visible}
                >
                    <Button type="primary" onClick={this.showChildrenDrawer}>
                        Two-level drawer
          </Button>
                    <Drawer
                        title="Two-level Drawer"
                        width={320}
                        closable={false}
                        onClose={this.onChildrenDrawerClose}
                        visible={this.state.childrenDrawer}
                    >
                        This is two-level drawer
          </Drawer>
                    <div
                        style={{
                            position: 'absolute',
                            bottom: 0,
                            width: '100%',
                            borderTop: '1px solid #e8e8e8',
                            padding: '10px 16px',
                            textAlign: 'right',
                            left: 0,
                            background: '#fff',
                            borderRadius: '0 0 4px 4px',
                        }}
                    >
                        <Button
                            style={{
                                marginRight: 8,
                            }}
                            onClick={this.onClose}
                        >
                            Cancel
            </Button>
                        <Button onClick={this.onClose} type="primary">
                            Submit
            </Button>
                    </div>
                </Drawer>
            </div>
        );
    }
}

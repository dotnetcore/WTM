/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-02-24 17:06:15
 * @modify date 2019-02-24 17:06:15
 * @desc [description]
 */
import * as React from 'react';

interface IAppProps {
    visible: boolean
}
/**
 * 控制组件 展示
 */
export class Visible extends React.Component<IAppProps, any> {
    render() {
        if (this.props.visible) {
            return this.props.children;
        }
        return null;
    }
}

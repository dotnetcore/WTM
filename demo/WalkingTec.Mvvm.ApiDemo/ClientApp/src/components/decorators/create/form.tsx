import * as React from 'react';
import { Form } from 'antd';
export function DesForm(Component: any) {
    return Form.create()(class extends React.PureComponent<any, any> {
        render() {
            // 扩展 默认 的 函数 传递 form 对象
            const getFieldDecorator = this.props.form.getFieldDecorator;
            this.props.form.getFieldDecorator = (id, options) => {
                return (node) => {
                    const reactNode = getFieldDecorator(id, options)(node)
                    return React.cloneElement(reactNode, {
                        form: this.props.form
                    })
                }
            }
            return (
                <Component {...this.props} />
            );
        }
    }) as any
}

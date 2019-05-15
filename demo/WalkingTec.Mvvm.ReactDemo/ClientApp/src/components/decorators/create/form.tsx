/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-02-24 17:07:00
 * @modify date 2019-02-24 17:07:00
 * @desc [description]
 */
import * as React from 'react';
import { Form } from 'antd';
export function DesForm(Component: React.ComponentClass<any, any>) {
    // console.log("创建表单")
    // console.dir(Component)
    return Form.create()(Component) as any //:React.ComponentClass<RcBaseFormProps & Omit<P, keyof FormComponentProps>>
    // return Form.create()(class AppForm extends React.PureComponent<any, any> {
    //     render() {
    //         // 扩展 默认 的 函数 传递 form 对象
    //         const getFieldDecorator = this.props.form.getFieldDecorator;
    //         this.props.form.getFieldDecorator = (id, options) => {
    //             return (node) => {
    //                 // 执行 表单创建的 装饰器
    //                 const reactNode = getFieldDecorator(id, options)(node);
    //                 let props: any = {
    //                     form: this.props.form,
    //                 }
    //                 try {
    //                     props = {
    //                         form: this.props.form,
    //                         // history: this.props.history,
    //                         // location: this.props.location,
    //                         // match: this.props.match,
    //                         // route: this.props.route,
    //                     }
    //                 } catch (error) {

    //                 }
    //                 return React.cloneElement(reactNode, props)
    //             }
    //         }
    //         return (
    //             <Component {...this.props} />
    //         );
    //     }
    // }) as any
}

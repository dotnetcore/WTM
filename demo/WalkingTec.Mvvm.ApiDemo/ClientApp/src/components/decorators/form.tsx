import * as React from 'react';
import { Form } from 'antd';
export default function (Component: any) {
    return Form.create()(Component) as any
}
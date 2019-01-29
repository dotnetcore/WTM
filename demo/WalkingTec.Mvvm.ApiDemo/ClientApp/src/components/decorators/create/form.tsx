import * as React from 'react';
import { Form } from 'antd';
export function DesForm(Component: any) {
    return Form.create()(Component) as any
}
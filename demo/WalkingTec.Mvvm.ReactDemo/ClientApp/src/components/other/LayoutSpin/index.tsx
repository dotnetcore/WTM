import { Icon, Spin } from 'antd';
import * as React from 'react';
import './style.less';
export default () => <div className='app-layout-spin'><Spin size="large" tip="loading..." indicator={<Icon type="loading" spin />} /></div>;
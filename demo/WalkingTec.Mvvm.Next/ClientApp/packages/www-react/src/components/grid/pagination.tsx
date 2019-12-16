import { Pagination } from 'antd';
import { observer } from 'mobx-react';
import * as React from 'react';
@observer
export default class GridPagination extends React.Component<any> {
    componentDidMount() {
    }
    render() {
        return <Pagination
            showSizeChanger
            defaultCurrent={3}
            total={500}
        />
    }
}

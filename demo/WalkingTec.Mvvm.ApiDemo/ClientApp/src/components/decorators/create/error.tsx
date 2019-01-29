import * as React from 'react';

export function DesError(Component: any) {
    return class extends React.Component<any, any> {
        state = {
            error: null,
            errorInfo: null
        };
        componentDidCatch(error, info) {
            this.setState({
                error: error,
                errorInfo: info
            })
        }
        render() {
            // 组件错误
            if (this.state.errorInfo) {
                return (
                    <div>
                        <h2>组件出错~</h2>
                        <details >
                            {this.state.error && this.state.error.toString()}
                            <br />
                            {this.state.errorInfo.componentStack}
                        </details>
                    </div>
                );
            }
            return <Component {...this.props} />;
        }
    } as any

}
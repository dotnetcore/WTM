/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-02-24 17:06:55
 * @modify date 2019-02-24 17:06:55
 * @desc [description]
 */
import * as React from 'react';
import lodash from "lodash";
export function DesError(Component: React.ComponentClass): any {
    // function <T extends { new(...args: any[]): React.Component<any, any> }>(constructor: T) {
    return class AppError extends Component {
        state = {
            ...this.state,
            __error: null,
            __errorInfo: null
        };
        componentDidCatch(error, info) {
            this.setState({
                __error: error,
                __errorInfo: info
            })
        }
        render() {
            // 组件错误
            if (this.state.errorInfo) {
                return (
                    <div>
                        <h2>组件出错~</h2>
                        <details >
                            <pre style={{ height: 300, background: "#f3f3f3" }}>
                                <code>{lodash.toString(this.state.__error)}</code>
                                <code>{lodash.get(this.state, '__errorInfo.componentStack')}</code>
                            </pre>
                        </details>
                    </div>
                );
            }
            return super.render();
        }
    }

}
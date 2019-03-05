/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-02-24 17:06:55
 * @modify date 2019-02-24 17:06:55
 * @desc [description]
 */
import * as React from 'react';
import lodash from "lodash";
export function DesError(Component: React.ComponentClass) {
    class AppError extends Component {
        state = {
            ...this.state,
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
                            <pre style={{ height: 300, background: "#f3f3f3" }}>
                                <code>{this.state.error && this.state.error.toString()}</code>
                                <code>{this.state.errorInfo.componentStack}</code>
                            </pre>
                        </details>
                    </div>
                );
            }
            return super.render();
        }
    }
    // 静态属性
    lodash.map(Component, (value, key) => {
        AppError[key] = value;
    })
    return AppError as any

}
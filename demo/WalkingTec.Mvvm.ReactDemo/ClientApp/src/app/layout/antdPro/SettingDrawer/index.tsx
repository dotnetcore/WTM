// import CopyToClipboard from 'react-copy-to-clipboard';
// import omit from 'omit.js';
// import defaultSettings, { Settings } from '@ant-design/pro-layout/es/defaultSettings';
// import getLocales, { getLanguage } from '@ant-design/pro-layout/es/locales';
import { Divider, Drawer, Icon, List, Select, Switch, Tooltip } from 'antd';
import React, { Component } from 'react';
import BlockCheckbox from './BlockCheckbox';
import ThemeColor from './ThemeColor';
import globalConfig from 'global.config';
import { getLocales } from 'locale';
import './index.less';
import { MenuTheme } from 'antd/es/menu/MenuContext';
export declare type ContentWidth = 'Fluid' | 'Fixed';
export interface Settings {
    /**
     * theme for nav menu
     */
    navTheme: MenuTheme | undefined;
    /**
     * nav menu position: `sidemenu` or `topmenu`
     */
    layout: 'sidemenu' | 'topmenu';
    /**
     * layout of content: `Fluid` or `Fixed`, only works when layout is topmenu
     */
    contentWidth: ContentWidth;
    /**
     * 主题色
     */
    primaryColor: string;
    /**
     * sticky header
     */
    fixedHeader: boolean;
    /**
     * auto hide header
     */
    autoHideHeader: boolean;
    /**
     * sticky siderbar
     */
    fixSiderbar: boolean;
    menu: {
        locale: boolean;
    };
    title: string;
    iconfontUrl: string;
    /**
     * 弹框类型
     *
     * @type {("Modal" | "Drawer")}
     * @memberof Settings
     */
    infoType?: "Modal" | "Drawer";
    /**
     * AgGrid 主题
     * ag-theme-balham
     * ag-theme-material
     */
    agGridTheme?: "ag-theme-balham" | "ag-theme-material" | 'ag-theme-alpine';
    /**
     *页签 页面
     *
     * @type {boolean}
     * @memberof Settings
     */
    tabsPage?: boolean;
}
// import { isBrowser } from '../utils/utils';
const isBrowser = () => typeof window !== 'undefined';

const { Option } = Select;
interface BodyProps {
    title: string;
}
type MergerSettingsType<T> = Partial<T> & {
    primaryColor?: string;
    colorWeak?: boolean;
};

const Body: React.FC<BodyProps> = ({ children, title }) => (
    <div style={{ marginBottom: 24 }}>
        <h3 className="ant-pro-setting-drawer-title">{title}</h3>
        {children}
    </div>
);

interface SettingItemProps {
    title: React.ReactNode;
    action: React.ReactElement;
    disabled?: boolean;
    disabledReason?: React.ReactNode;
}

export interface SettingDrawerProps {
    settings: MergerSettingsType<Settings>;
    collapse?: boolean;
    // for test
    getContainer?: any;
    onCollapseChange?: (collapse: boolean) => void;
    onSettingChange?: (settings: MergerSettingsType<Settings>) => void;
}

export interface SettingDrawerState extends MergerSettingsType<Settings> {
    collapse?: boolean;
    language?: string;
}

class SettingDrawer extends Component<SettingDrawerProps, SettingDrawerState> {
    state: SettingDrawerState = {
        collapse: false,
        language: globalConfig.language,
    };

    static getDerivedStateFromProps(
        props: SettingDrawerProps,
    ): SettingDrawerState | null {
        if ('collapse' in props) {
            return {
                collapse: !!props.collapse,
            };
        }
        return null;
    }

    componentDidMount(): void {
        if (isBrowser()) {
            window.addEventListener('languagechange', this.onLanguageChange, {
                passive: true,
            });
        }
    }

    componentWillUnmount(): void {
        if (isBrowser()) {
            window.removeEventListener('languagechange', this.onLanguageChange);
        }
    }

    onLanguageChange = (): void => {
        const language = globalConfig.language;//getLanguage();

        if (language !== this.state.language) {
            this.setState({
                language,
            });
        }
    };

    getLayoutSetting = (): SettingItemProps[] => {
        const { settings } = this.props;
        const formatMessage = this.getFormatMessage();
        const { contentWidth, fixedHeader, layout, autoHideHeader, fixSiderbar, infoType = "Modal", agGridTheme = 'ag-theme-material', tabsPage } = settings;
        return [
            {
                title: formatMessage({
                    id: 'app.setting.content-width',
                    defaultMessage: 'Content Width',
                }),
                action: (
                    <Select<string>
                        value={contentWidth}
                        size="small"
                        onSelect={value => this.changeSetting('contentWidth', value)}
                        style={{ width: 80 }}
                    >
                        {layout === 'sidemenu' ? null : (
                            <Option value="Fixed">
                                {formatMessage({
                                    id: 'app.setting.content-width.fixed',
                                    defaultMessage: 'Fixed',
                                })}
                            </Option>
                        )}
                        <Option value="Fluid">
                            {formatMessage({
                                id: 'app.setting.content-width.fluid',
                                defaultMessage: 'Fluid',
                            })}
                        </Option>
                    </Select>
                ),
            },
            {
                title: formatMessage({
                    id: 'app.setting.infoType',
                    defaultMessage: 'Info Type',
                }),
                action: (
                    <Select<string>
                        value={infoType}
                        size="small"
                        onSelect={value => this.changeSetting('infoType', value)}
                        style={{ width: 80 }}
                    >
                        <Option value="Modal">
                            {formatMessage({
                                id: 'app.setting.infoType.Modal',
                                defaultMessage: 'Modal',
                            })}
                        </Option>
                        <Option value="Drawer">
                            {formatMessage({
                                id: 'app.setting.infoType.Drawer',
                                defaultMessage: 'Drawer',
                            })}
                        </Option>
                    </Select>
                ),
            },
            {
                title: formatMessage({
                    id: 'app.setting.agGridTheme',
                    defaultMessage: 'Ag Grid Theme',
                }),
                action: (
                    <Select<string>
                        value={agGridTheme}
                        size="small"
                        onSelect={value => this.changeSetting('agGridTheme', value)}
                        style={{ width: 80 }}
                    >
                        <Option value="ag-theme-balham">
                            {formatMessage({
                                id: 'app.setting.agGridTheme.balham',
                                defaultMessage: 'balham',
                            })}
                        </Option>
                        <Option value="ag-theme-material">
                            {formatMessage({
                                id: 'app.setting.agGridTheme.material',
                                defaultMessage: 'material',
                            })}
                        </Option>
                        <Option value="ag-theme-alpine">
                            {formatMessage({
                                id: 'app.setting.agGridTheme.alpine',
                                defaultMessage: 'alpine',
                            })}
                        </Option>
                    </Select>
                ),
            },
            {
                title: formatMessage({
                    id: 'app.setting.tabsPage',
                    defaultMessage: 'Tabs Page',
                }),
                disabled: contentWidth === "Fixed",
                action: (
                    <Switch
                        size="small"
                        checked={!!tabsPage}
                        onChange={checked => this.changeSetting('tabsPage', checked)}
                    />
                ),
            },
            {
                title: formatMessage({
                    id: 'app.setting.fixedheader',
                    defaultMessage: 'Fixed Header',
                }),
                disabled: tabsPage,
                action: (
                    <Switch
                        size="small"
                        checked={!!fixedHeader}
                        onChange={checked => this.changeSetting('fixedHeader', checked)}
                    />
                ),
            },

            {
                title: formatMessage({
                    id: 'app.setting.hideheader',
                    defaultMessage: 'Hidden Header when scrolling',
                }),
                disabled: tabsPage || !fixedHeader,
                disabledReason: formatMessage({
                    id: 'app.setting.hideheader.hint',
                    defaultMessage: 'Works when Hidden Header is enabled',
                }),
                action: (
                    <Switch
                        size="small"
                        checked={!!autoHideHeader}
                        onChange={checked => this.changeSetting('autoHideHeader', checked)}
                    />
                ),
            },
            {
                title: formatMessage({
                    id: 'app.setting.fixedsidebar',
                    defaultMessage: 'Fixed Sidebar',
                }),
                disabled: tabsPage || layout === 'topmenu',
                disabledReason: formatMessage({
                    id: 'app.setting.fixedsidebar.hint',
                    defaultMessage: 'Works on Side Menu Layout',
                }),
                action: (
                    <Switch
                        size="small"
                        checked={!!fixSiderbar}
                        onChange={checked => this.changeSetting('fixSiderbar', checked)}
                    />
                ),
            },
        ];
    };

    changeSetting = (key: string, value: string | boolean) => {
        const { settings } = this.props;
        const nextState = { ...settings };
        nextState[key] = value;
        if (key === 'layout') {
            nextState.contentWidth = value === 'topmenu' ? 'Fixed' : 'Fluid';
            nextState.tabsPage = value === 'sidemenu';
        } else if (key === 'fixedHeader' && !value) {
            nextState.autoHideHeader = false;
        }
        this.setState(nextState, () => {
            const { onSettingChange } = this.props;
            if (onSettingChange) {
                onSettingChange(this.state as MergerSettingsType<Settings>);
            }
        });
    };

    togglerContent = () => {
        const { collapse } = this.state;
        const { onCollapseChange } = this.props;
        if (onCollapseChange) {
            onCollapseChange(!collapse);
            return;
        }
        this.setState({ collapse: !collapse });
    };

    renderLayoutSettingItem = (item: SettingItemProps) => {
        const action = React.cloneElement(item.action, {
            disabled: item.disabled,
        });
        return (
            <Tooltip
                title={item.disabled ? item.disabledReason : ''}
                placement="left"
            >
                <List.Item actions={[action]}>
                    <span style={{ opacity: item.disabled ? 0.5 : 1 }}>{item.title}</span>
                </List.Item>
            </Tooltip>
        );
    };

    getFormatMessage = (): ((data: {
        id: string;
        defaultMessage?: string;
    }) => string) => {
        const formatMessage = ({
            id,
            defaultMessage,
        }: {
            id: string;
            defaultMessage?: string;
        }): string => {
            const locales = getLocales();
            if (locales[id]) {
                return locales[id];
            }
            if (defaultMessage) {
                return defaultMessage as string;
            }
            return id;
        };
        return formatMessage;
    };

    render(): React.ReactNode {
        const { settings, getContainer } = this.props;
        const {
            navTheme = 'dark',
            primaryColor = '1890FF',
            layout = 'sidemenu',
            colorWeak,
        } = (settings) as any;
        const { collapse } = this.state;
        const formatMessage = this.getFormatMessage();
        return (
            <Drawer
                visible={collapse}
                width={300}
                onClose={this.togglerContent}
                placement="right"
                getContainer={getContainer}
                // handler={
                //   <div
                //     className="ant-pro-setting-drawer-handle"
                //     onClick={this.togglerContent}
                //   >
                //     <Icon
                //       type={collapse ? 'close' : 'setting'}
                //       style={{
                //         color: '#fff',
                //         fontSize: 20,
                //       }}
                //     />
                //   </div>
                // }
                style={{
                    zIndex: 999,
                }}
            >
                <div className="ant-pro-setting-drawer-content">
                    <Body
                        title={formatMessage({
                            id: 'app.setting.pagestyle',
                            defaultMessage: 'Page style setting',
                        })}
                    >
                        <BlockCheckbox
                            list={[
                                {
                                    key: 'dark',
                                    url:
                                        'https://gw.alipayobjects.com/zos/antfincdn/XwFOFbLkSM/LCkqqYNmvBEbokSDscrm.svg',
                                    title: formatMessage({
                                        id: 'app.setting.pagestyle.dark',
                                        defaultMessage: '',
                                    }),
                                },
                                {
                                    key: 'light',
                                    url:
                                        'https://gw.alipayobjects.com/zos/antfincdn/NQ%24zoisaD2/jpRkZQMyYRryryPNtyIC.svg',
                                    title: formatMessage({ id: 'app.setting.pagestyle.light' }),
                                },
                            ]}
                            value={navTheme}
                            onChange={value => this.changeSetting('navTheme', value)}
                        />
                    </Body>

                    <ThemeColor
                        title={formatMessage({ id: 'app.setting.themecolor' })}
                        value={primaryColor}
                        formatMessage={formatMessage}
                        onChange={color => this.changeSetting('primaryColor', color)}
                    />

                    <Divider />

                    <Body title={formatMessage({ id: 'app.setting.navigationmode' })}>
                        <BlockCheckbox
                            list={[
                                {
                                    key: 'sidemenu',
                                    url:
                                        'https://gw.alipayobjects.com/zos/antfincdn/XwFOFbLkSM/LCkqqYNmvBEbokSDscrm.svg',
                                    title: formatMessage({ id: 'app.setting.sidemenu' }),
                                },
                                {
                                    key: 'topmenu',
                                    url:
                                        'https://gw.alipayobjects.com/zos/antfincdn/URETY8%24STp/KDNDBbriJhLwuqMoxcAr.svg',
                                    title: formatMessage({ id: 'app.setting.topmenu' }),
                                },
                            ]}
                            value={layout}
                            onChange={value => this.changeSetting('layout', value)}
                        />
                    </Body>

                    <List
                        split={false}
                        dataSource={this.getLayoutSetting()}
                        renderItem={this.renderLayoutSettingItem}
                    />

                    <Divider />

                    {/* <Body title={formatMessage({ id: 'app.setting.othersettings' })}>
            <List
              split={false}
              renderItem={this.renderLayoutSettingItem}
              dataSource={[
                {
                  title: formatMessage({ id: 'app.setting.weakmode' }),
                  action: (
                    <Switch
                      size="small"
                      checked={!!colorWeak}
                      onChange={checked =>
                        this.changeSetting('colorWeak', checked)
                      }
                    />
                  ),
                },
              ]}
            />
          </Body>
          <Divider /> */}
                    {/* <CopyToClipboard
            text={JSON.stringify(omit(settings, ['colorWeak']), null, 2)}
            onCopy={() =>
              message.success(formatMessage({ id: 'app.setting.copyinfo' }))
            }
          >
            <Button block icon="copy">
              {formatMessage({ id: 'app.setting.copy' })}
            </Button>
          </CopyToClipboard> */}
                </div>
            </Drawer>
        );
    }
}

export default SettingDrawer;

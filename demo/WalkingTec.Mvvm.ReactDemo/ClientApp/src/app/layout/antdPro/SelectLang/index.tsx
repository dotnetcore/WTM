import { Icon, Menu } from 'antd';
import classNames from 'classnames';
import React from 'react';
import HeaderDropdown from '../HeaderDropdown';
import styles from './index.module.less';

interface SelectLangProps {
  className?: string;
  selectedLang?: string;
  changeLang?: (key) => void;
}
const SelectLang: React.FC<SelectLangProps> = props => {
  const { className, changeLang, selectedLang } = props;
  // const selectedLang = 'zh-CN';
  // const changeLang = ({ key }: ClickParam): void => setLocale(key, false);
  // const locales = ['zh-CN', 'zh-TW', 'en-US', 'pt-BR'];
  const locales = ['zh-CN', 'en-US'];
  const languageLabels = {
    'zh-CN': '简体中文',
    'zh-TW': '繁体中文',
    'en-US': 'English',
    'pt-BR': 'Português',
  };
  const languageIcons = {
    'zh-CN': '🇨🇳',
    'zh-TW': '🇭🇰',
    'en-US': '🇬🇧',
    'pt-BR': '🇧🇷',
  };
  const langMenu = (
    <Menu
      className={styles.menu}
      selectedKeys={[selectedLang]}
      onClick={changeLang}
    >
      {locales.map(locale => (
        <Menu.Item key={locale}>
          <span role="img" aria-label={languageLabels[locale]}>
            {languageIcons[locale]}
          </span>{' '}
          {languageLabels[locale]}
        </Menu.Item>
      ))}
    </Menu>
  );
  return (
    <HeaderDropdown overlay={langMenu} placement="bottomRight">
      <span className={classNames(styles.dropDown, className)}>
        <Icon type="global" title={languageLabels["zh-CN"]} />
      </span>
    </HeaderDropdown>
  );
};

export default SelectLang;

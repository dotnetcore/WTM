import React from 'react';
import SelectLang from '../SelectLang';
import UserMenu from './userMenu';
import styles from './index.module.less';

export type SiderTheme = 'light' | 'dark';
export interface GlobalHeaderRightProps {
  theme?: SiderTheme;
  layout?: 'sidemenu' | 'topmenu';
  changeLang?: (key) => void;
}

const GlobalHeaderRight: React.SFC<GlobalHeaderRightProps> = props => {
  const { theme, layout = 'sidemenu', changeLang } = props;
  let className = styles.right;
  if (theme === 'dark' && layout === 'topmenu') {
    className = `${styles.right}  ${styles.dark}`;
  }

  return (
    <div className={className}>
      <UserMenu {...props} />
      <SelectLang className={styles.action} changeLang={changeLang} />
    </div>
  );
};

export default GlobalHeaderRight;

import React from 'react';
import SelectLang from '../SelectLang';
import styles from './index.module.less';

export type SiderTheme = 'light' | 'dark';
export interface GlobalHeaderRightProps {
  theme?: SiderTheme;
  layout?: 'sidemenu' | 'topmenu';
  selectedLang?: string;
  changeLang?: (key) => void;
}

const GlobalHeaderRight: React.SFC<GlobalHeaderRightProps> = props => {
  const { theme, layout = 'sidemenu', changeLang, selectedLang } = props;
  let className = styles.right;
  if (theme === 'dark' && layout === 'topmenu') {
    className = `${styles.right}  ${styles.dark}`;
  }

  return (
    <div className={className}>
      {props.children}
      <SelectLang className={styles.action} selectedLang={selectedLang} changeLang={changeLang} />
    </div>
  );
};

export default GlobalHeaderRight;

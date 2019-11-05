import React, { createElement } from 'react';
import { Button } from 'antd';
import { Link } from 'react-router-dom';
import config from './typeConfig';
import './index.less';
import { FormattedMessage } from 'react-intl';
interface IExceptionProps {
  type?: '403' | '404' | '500';
  title?: React.ReactNode;
  desc?: React.ReactNode;
  img?: string;
  actions?: React.ReactNode;
  linkElement?: string | React.ComponentType;
  style?: React.CSSProperties;
  className?: string;
  backText?: React.ReactNode;
  redirect?: string;
  [key: string]: any;
}
class Exception extends React.PureComponent<IExceptionProps, any>{
  static defaultProps = {
    backText: 'back to home',
    redirect: '/',
  };

  constructor(props) {
    super(props);
    this.state = {};
  }

  render() {
    const {
      className,
      backText,
      linkElement = 'a',
      type,
      title,
      desc,
      img,
      actions,
      redirect,
      staticContext,
      ...rest
    } = this.props;
    const pageType = type in config ? type : '404';
    return (
      <div className={'exception'} {...rest}>
        <div className={'imgBlock'}>
          <div
            className={'imgEle'}
            style={{ backgroundImage: `url(${img || config[pageType].img})` }}
          />
        </div>
        <div className={'content'}>
          <h1>{title || config[pageType].title}</h1>
          <div className={'desc'}>{desc ||  <FormattedMessage id={`tips.status.${pageType}`} />}</div>
          <div className={'actions'}>
            {actions ||
              // createElement(
              //   linkElement as any,
              //   {
              //     to: redirect,
              //     href: redirect,
              //   },
              //   <Button type="primary">{backText}</Button>
              // )
              <Link to={redirect}><Button type="primary">{backText}</Button></Link>
            }
          </div>
        </div>
      </div>
    );
  }
}

export default Exception;

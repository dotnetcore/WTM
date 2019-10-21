/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-07-24 05:02:33
 * @modify date 2018-07-24 05:02:33
 * @desc [description]
 */
import * as React from 'react';
import { BrowserRouter } from 'react-router-dom';
import routes from './router';
// @observer
export default () => <BrowserRouter >
    {routes}
</BrowserRouter>
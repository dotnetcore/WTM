import action from './action';
import pages from './pages';
import settingDrawer from './settingDrawer';
import tips from './tips';
import other from './other';
export default {
    ...tips,
    ...other,
    ...action,
    ...settingDrawer,
    ...pages
};

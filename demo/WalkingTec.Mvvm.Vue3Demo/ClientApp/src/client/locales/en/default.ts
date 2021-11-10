import action from './action';
import pages from './pages';
import tips from './tips';
import other from './other';
export default {
    ...tips,
    ...other,
    ...action,
    ...pages
};

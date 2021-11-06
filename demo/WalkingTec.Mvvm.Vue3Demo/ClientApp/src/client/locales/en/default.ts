import action from './action';
import pages from './pages';
import tips from './tips';
import other from './other';
import sys from '../languagesys';
export default {
    ...tips,
    ...other,
    ...action,
    ...pages,
    ...sys.en
};

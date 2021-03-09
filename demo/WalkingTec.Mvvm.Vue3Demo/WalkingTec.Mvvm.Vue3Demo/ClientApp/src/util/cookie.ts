import Cookies from "js-cookie";
import config from "@/config/index";

// App
const sidebarStatusKey = "sidebar_status";
export const getSidebarStatus = () => Cookies.get(sidebarStatusKey);
export const setSidebarStatus = (sidebarStatus: string) =>
    Cookies.set(sidebarStatusKey, sidebarStatus, { expires: config.cookiesExpires });

const languageKey = "language";
export const getLanguage = () => Cookies.get(languageKey);
export const setLanguage = (language: string) =>
    Cookies.set(languageKey, language, { expires: config.cookiesExpires });

const sizeKey = "size";
export const getSize = () => Cookies.get(sizeKey);
export const setSize = (size: string) => Cookies.set(sizeKey, size, { expires: config.cookiesExpires });

// User
const tokenKey = config.tokenKey;
export const getToken = () => Cookies.get(tokenKey);
export const setToken = (token: string) => Cookies.set(tokenKey, token, { expires: config.cookiesExpires });
export const removeToken = () => Cookies.remove(tokenKey);


const settingsKey = "settings";
export const getSettings = () => Cookies.getJSON(settingsKey);
export const setSettings = (value: object) => Cookies.set(settingsKey, value, { expires: config.cookiesExpires });

/**
 * set cookie
 * @param key
 * @param value
 */
export const setCookie = (key, value) => {
    Cookies.set(key, value, { expires: config.cookiesExpires });
};

/**
 * get cookie
 * @param name
 */
export const getCookie = name => {
    return Cookies.get(name);
};

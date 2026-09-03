import { useUserInfo } from '/@/stores/userInfo';

/**
 * 单个权限验证
 * @param value 权限值
 * @returns 有权限，返回 `true`，反之则反
 */
export function auth(value: unknown): boolean {
	if (typeof value !== 'string') return false;
	const stores = useUserInfo();
	if (stores.userInfos.isDebug === true) return true;
	return stores.authBtnSet.has(value.trim().toLowerCase());
}

/**
 * 多个权限验证，满足一个则为 true
 * @param value 权限值
 * @returns 有权限，返回 `true`，反之则反
 */
export function auths(value: unknown): boolean {
	if (!Array.isArray(value) || !value.every((item) => typeof item === 'string')) return false;
	const stores = useUserInfo();
	if (stores.userInfos.isDebug === true) return true;
	return value.some((permission) => stores.authBtnSet.has(permission.trim().toLowerCase()));
}

/**
 * 多个权限验证，全部满足则为 true
 * @param value 权限值
 * @returns 有权限，返回 `true`，反之则反
 */
export function authAll(value: unknown): boolean {
	if (!Array.isArray(value) || !value.every((item) => typeof item === 'string')) return false;
	const stores = useUserInfo();
	if (stores.userInfos.isDebug === true) return true;
	return value.length > 0 && value.every((permission) => stores.authBtnSet.has(permission.trim().toLowerCase()));
}

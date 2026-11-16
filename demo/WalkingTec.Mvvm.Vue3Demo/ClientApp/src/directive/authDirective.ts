import { watch } from 'vue';
import type { App, ObjectDirective } from 'vue';
import { useUserInfo } from '/@/stores/userInfo';
import { auth, authAll, auths } from '/@/utils/authFunction';

type PermissionCheck = (value: unknown) => boolean;

interface PermissionElementState {
	bindingValue: unknown;
	checkPermission: PermissionCheck;
	wasInitiallyHidden: HTMLElement['hidden'];
}

// 注册所有使用权限指令的元素。这里只保存元素状态，不为每个元素创建 watcher。
// Map 需要在 unmounted 中主动删除，避免继续持有已经卸载的 DOM。
const permissionElements = new Map<HTMLElement, PermissionElementState>();
let stopWatchingPermissions: (() => void) | undefined;

function updateElementVisibility(el: HTMLElement, state: PermissionElementState) {
	const shouldHide = state.wasInitiallyHidden || !state.checkPermission(state.bindingValue);
	if (el.hidden !== shouldHide) el.hidden = shouldHide;
}

function updateAllPermissionElements() {
	permissionElements.forEach((state, el) => updateElementVisibility(el, state));
}

function startWatchingPermissions() {
	if (stopWatchingPermissions) return;

	const stores = useUserInfo();
	stopWatchingPermissions = watch(
		[() => stores.userInfos.isDebug, () => stores.authBtnSet],
		updateAllPermissionElements
	);
}

function createPermissionDirective(checkPermission: PermissionCheck): ObjectDirective<HTMLElement, unknown> {
	return {
		mounted(el, binding) {
			const state = {
				bindingValue: binding.value,
				checkPermission,
				wasInitiallyHidden: el.hidden,
			};
			permissionElements.set(el, state);
			updateElementVisibility(el, state);
			startWatchingPermissions();
		},
		updated(el, binding) {
			const state = permissionElements.get(el);
			if (!state) return;

			state.bindingValue = binding.value;
			updateElementVisibility(el, state);
		},
		unmounted(el) {
			permissionElements.delete(el);

			// 页面上已经没有权限指令时，连唯一的共享 watcher 也一起释放。
			if (permissionElements.size === 0) {
				stopWatchingPermissions?.();
				stopWatchingPermissions = undefined;
			}
		},
	};
}

/**
 * 用户权限指令
 * @directive 单个权限验证（v-auth="xxx"）
 * @directive 多个权限验证，满足一个则显示（v-auths="[xxx,xxx]"）
 * @directive 多个权限验证，全部满足则显示（v-auth-all="[xxx,xxx]"）
 */
export function authDirective(app: App) {
	// 单个权限验证（v-auth="xxx"）
	app.directive('auth', createPermissionDirective(auth));
	// 多个权限验证，满足一个则显示（v-auths="[xxx,xxx]"）
	app.directive('auths', createPermissionDirective(auths));
	// 多个权限验证，全部满足则显示（v-auth-all="[xxx,xxx]"）
	app.directive('auth-all', createPermissionDirective(authAll));
}

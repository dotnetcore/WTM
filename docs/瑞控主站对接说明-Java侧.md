# 瑞控主站对接说明（Java 侧）

对接背景：用户、密码、角色都在瑞控主站维护，我们这边（主站）是 Vue3 版。你们的系统作为子站接入，不存用户密码，登录和用户信息都从主站拿。

文中 `{主站}` 指瑞控主站的基地址（如 `http://rk.example.com`），联调前找我们要：主站地址、一个测试账号、约定好的角色编码。

---

## 一、分工先说清楚

- 用户、密码、角色、用户组：主站管。你们的系统里不要做这些管理功能，遇到相关需求直接让用户去主站操作。
- 登录验证：调主站接口完成，你们不自己校验密码。
- 界面权限：你们自己定。判断依据只有一个——主站返回的角色编码（RoleCode）。

---

## 二、你们要做的功能

### 1. 登录页（联调必备）

建议一上来就做这个，联调初期主站跳转还没打通，进你们系统全靠它：

1. 用户输入账号密码，调主站 **loginjwt**（见 3.2）换 token；
2. 拿 token 调 **checkuserinfo**（见 3.1）取用户信息；
3. 建立你们自己的会话，进首页。账号密码错了主站返回 400，提示"用户名或密码错误"即可。

自测的时候有个偷懒办法：用 postman 直接调主站 loginjwt 拿到 `access_token`，这个 token 就是 `_remotetoken`，直接拼个地址扔浏览器就能测你们的 SSO 入口：

```
http://你们的系统/sso/login?_remotetoken=刚才拿到的access_token
```

### 2. SSO 登录入口（正式用法）

用户在主站已登录，主站会往你们系统跳，URL 长这样：

```
http://你们的系统/sso/login?_remotetoken=<主站签发的JWT>
```

处理逻辑：

1. 取 `_remotetoken` 参数；
2. 调 3.1 的 checkuserinfo 验证，顺便拿到用户信息；
3. 通过了就把用户信息放进 Session（或你们自己的 token 机制），跳首页；
4. 不通过（主站返回 400）就回主站登录页 `{主站}/login`。如果你们的页面会被主站 iframe 嵌入，失败别跳转，按 6.3 第 3 条处理；
5. 跳转后 `_remotetoken` 就不在地址栏了。这个参数等于登录凭证，别打进日志。

Spring 的写法大致是：

```java
@GetMapping("/sso/login")
public String sso(@RequestParam("_remotetoken") String remoteToken, HttpSession session) {
    try {
        RkUserInfo user = rkClient.checkUserInfo(remoteToken);
        session.setAttribute("rkUser", user);
        session.setAttribute("remoteToken", remoteToken);
        return "redirect:/index";
    } catch (Exception e) {
        return "redirect:{主站}/login";
    }
}
```

### 3. 登出

用户在你们系统登出时：

1. 带上该用户的 token 调主站 3.5 的 logout；
2. 清你们本地会话；
3. 浏览器跳回 `{主站}`。

### 4. 权限判断

主站管理员会建几个角色（编码提前约定好，比如 `JAVA_ADMIN`、`JAVA_USER`），分配给用户。你们拿到 checkuserinfo 返回的 `Roles[].RoleCode`，在接口鉴权里判断包含哪个编码就行。角色本身不用同步到你们库里，每次登录现拿、加缓存即可。

---

## 三、要调的主站接口

除了 loginjwt，其余接口都要带请求头 `Authorization: Bearer <token>`。

### 3.1 获取用户信息（checkuserinfo）

```
GET {主站}/api/admin/account/checkuserinfo?IsApi=false
Authorization: Bearer <token>
```

token 有效就返回用户信息，无效或过期返回 400。你们验证 token 只有这一个办法，不用也不需要在 Java 侧验签。返回字段是 PascalCase：

```json
{
  "UserId": "b8b2a9d0-1111-2222-3333-444455556666",
  "ITCode": "admin",
  "Name": "管理员",
  "TenantCode": null,
  "CurrentTenant": null,
  "Roles": [
    { "ID": "3f0d...", "RoleCode": "001", "RoleName": "管理员", "Tenant": null }
  ],
  "Groups": [
    { "ID": "5a1c...", "GroupCode": "g001", "GroupName": "总部", "Manager": "admin", "ParentId": null, "Tenant": null }
  ],
  "Attributes": { "IsMainHost": false, "IsDebug": false },
  "TimeTick": 638000000000000000
}
```

实际用到的就几个：`ITCode`（登录名，当唯一标识）、`Name`（显示名）、`Roles[].RoleCode`（权限判断）、`Groups[].GroupCode`（如果你们要按组）。建议按 ITCode 缓存，5 到 10 分钟，别每个请求都打主站。

### 3.2 账号密码换 token（loginjwt）

```
POST {主站}/api/admin/account/loginjwt
Content-Type: application/json

{ "Account": "admin", "Password": "123456" }
```

成功返回（这组字段是 snake_case，注意和 3.1 不一样）：

```json
{
  "access_token": "eyJhbGciOiJIUzI1NiIs...",
  "expires_in": 3600,
  "token_type": "Bearer",
  "refresh_token": ""
}
```

账号或密码错误返回 400。

### 3.3 刷新 token（RefreshToken）

```
POST {主站}/api/admin/account/RefreshToken
Authorization: Bearer <还有效的token>
```

返回格式同 3.2，用新的 `access_token` 替换旧的。旧 token 已经过期的情况下刷不了，只能重新走登录或 SSO。建议在会话里记下 token 签发时间，快到 1 小时时主动刷一次。

### 3.4 登出（logout）

```
GET {主站}/api/admin/account/logout
Authorization: Bearer <token>
```

### 3.5 按角色查用户（GetUserByRole）

做消息推送之类的功能会用到：

```
GET {主站}/api/admin/frameworkuser/GetUserByRole?keywords=001
Authorization: Bearer <token>
```

```json
[ "admin", "zhangsan", "lisi" ]
```

### 3.6 按用户组查用户（GetUserByGroup）

```
GET {主站}/api/admin/frameworkuser/GetUserByGroup?keywords=g001
```

返回格式同 3.5。

### 3.7 用户模糊搜索（GetUserById）

做选人下拉框用，keywords 传前缀：

```
GET {主站}/api/admin/account/GetUserById?keywords=adm
```

```json
[ { "Text": "管理员(admin)", "Value": "admin" } ]
```

### 3.8 角色列表（GetFrameworkRoles）

```
GET {主站}/api/admin/account/GetFrameworkRoles
```

```json
[ { "Text": "管理员", "Value": "001" }, { "Text": "普通用户", "Value": "002" } ]
```

---

## 四、DTO 定义

对应 3.1 的返回，直接拿去改：

```java
public class RkUserInfo {
    private String userId;
    private String itCode;              // 登录名，唯一标识
    private String name;
    private String tenantCode;
    private List<RkRole> roles;
    private List<RkGroup> groups;
    private Map<String, Object> attributes;
}

public class RkRole {
    private String roleCode;            // 权限判断只用这个
    private String roleName;
}

public class RkGroup {
    private String groupCode;
    private String groupName;
}
```

```java
public class RkToken {
    @JsonProperty("access_token")
    private String accessToken;
    @JsonProperty("expires_in")
    private int expiresIn;
    @JsonProperty("token_type")
    private String tokenType;
}
```

---

## 五、几个容易翻车的点

1. 用户信息是 PascalCase，token 是 snake_case。`ITCode` 和 `access_token` 混在一个项目里，JSON 映射注意区分。
2. 失败返回的是 **400**，不是 401。checkuserinfo 和 loginjwt 验证不过都走 400。
3. `UserId`、`ID` 这些是 Guid 字符串，36 位带横线。
4. 值为 null 的字段直接不出现在响应里，取 `Roles`、`Groups` 前判空。
5. token 有效期默认 1 小时，过期后所有接口都 400。
6. token 等于登录凭证，不打日志、不拼进重定向、全站 HTTPS。
7. 服务器时间做 NTP 同步，过期时间是按 UTC 算的，时钟偏差大会莫名其妙判过期。
8. 主站停用某个用户后，你们那边最长延迟一个缓存周期。缓存别设太长，10 分钟以内。

---

## 六、iframe 嵌入模式

主站（Vue3）会在自己的页面里用 iframe 直接嵌你们的系统，用户在主站菜单里点一下，你们的页面就显示在主站内容区里。登录流程和上面一样，下面是额外要处理的事。6.2 是我们主站这边做的，你们扫一眼知道流程就行。

### 6.1 流程

主站每次打开嵌入页，先用自己的登录态调一次 RefreshToken 拿个新 token（token 只有 1 小时，登录时存的那份可能早过期了），然后 iframe 加载：

```
http://你们的系统/sso/login?_remotetoken=<新token>&embed=1
```

你们的 SSO 入口验证、建会话，页面显示在 iframe 里。之后的请求靠你们自己的会话，跟主站没关系。会话过期了你们发 postMessage 通知主站，主站重新拿 token 刷新 iframe，用户无感。

### 6.2 主站侧（我们做，你们可略过）

主站加个页面组件，注册路由（如 `/javasite`），菜单管理里配菜单指向它：

```vue
<template>
  <iframe v-if="src" :src="src" frameborder="0"
          style="width:100%;height:100%;border:0" />
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';
import request from '/@/utils/request';

const JAVA_ORIGIN = 'http://java系统地址';
const src = ref('');

async function loadFrame() {
  // 用当前登录态换新 token
  const res = await request({ url: '/api/admin/account/RefreshToken', method: 'post' });
  src.value = `${JAVA_ORIGIN}/sso/login?_remotetoken=${res.access_token}&embed=1`;
}

function onMessage(e: MessageEvent) {
  if (e.origin !== JAVA_ORIGIN) return;
  if (e.data?.type === 'TOKEN_EXPIRED') loadFrame();
}

onMounted(() => { loadFrame(); window.addEventListener('message', onMessage); });
onUnmounted(() => window.removeEventListener('message', onMessage));
</script>
```

### 6.3 你们要额外处理的

**1. 允许被 iframe 嵌入**

响应头里不能有 `X-Frame-Options: DENY` / `SAMEORIGIN`。用 Spring Security 的话它默认就加 DENY，要显式关掉：

```java
// Spring Security 6.x
http.headers(h -> h.frameOptions(HeadersConfigurer.FrameOptionsConfig::disable));
// 5.x 老写法：http.headers().frameOptions().disable();
```

建议再加一条白名单，只许主站嵌：

```
Content-Security-Policy: frame-ancestors 'http://主站地址'
```

**2. 页面支持"只显示主体"**

被嵌进主站后，外面已经有主站的菜单和页签了，你们页面再带一套自己的顶部菜单、侧边栏，等于菜单套菜单，没法看。做个嵌入布局：约定一个参数（上面 URL 里的 `embed=1`，名字你们定），SSO 入口建完会话跳首页时把参数带上，前端识别到就用精简布局渲染，只留页面主体，不带自己的导航框架。前后端分离的话也可以单独准备一个 /embed 路由挂精简布局。

**3. SSO 失败和会话过期的行为**

在 iframe 里验证失败不要 302 跳主站登录页，登录页会嵌死在小窗格里。请求头 `Sec-Fetch-Dest: iframe` 可以判断是不是 iframe 场景，是的话就返回个提示页（"会话过期，请从主站重新打开"），或者直接走第 4 条的 postMessage。另外 SSO 成功后用 `history.replaceState` 把 `_remotetoken` 从地址栏清掉，不然用户按 F5 会把过期的 token 再发一遍。

**4. 会话过期通知主站**

iframe 里发现没登录了，别自己跳转，通知父窗口：

```js
window.parent.postMessage({ type: 'TOKEN_EXPIRED' }, 'http://主站地址');
```

主站收到后重新拿 token 刷新 iframe（见 6.2）。

**5. Cookie**

iframe 是第三方上下文，浏览器对第三方 Cookie 卡得越来越死（Safari 默认全禁，Chrome 也基本禁了），Cookie 带不上就会一直像没登录。最省事的解法是部署在同一个父域名下，比如主站 `www.xxx.com`、你们 `java.xxx.com`，同站 Cookie 没有限制。实在要跨父域，会话 Cookie 加 `SameSite=None; Secure`（强制 HTTPS），还得接受部分浏览器照样拒；被拒的兜底是你们前端把 token 放内存、请求走 Authorization 头，不依赖 Cookie，反正 iframe 每次打开都会带新 token 重新走一遍 SSO。

**6. 登出**

iframe 里别整页跳主站。postMessage 告诉父窗口，主站来关页签或者跳主站首页。

---

## 七、联调清单

- [ ] postman 调 3.2 拿到 access_token（测试账号问我们要）
- [ ] 拿这个 token 调 3.1，能看到自己的角色
- [ ] 浏览器开 `你们的/sso/login?_remotetoken=xxx`，免登录进首页，地址栏里 token 已清掉
- [ ] 登录页输入主站账号密码能进系统，错误密码有提示
- [ ] token 过一小时后操作，能正确回主站重新登录（或走刷新）
- [ ] 登出后回到主站页面
- [ ] 故意用错误 token 调 3.1，确认按 400 处理
- [ ] iframe 模式：主站菜单点开，内嵌页面已登录、只有主体没有你们的菜单
- [ ] iframe 模式：放着超过会话时长再操作，postMessage 生效，主站自动刷新 iframe，不用重新登录
- [ ] iframe 模式：你们的页面单独在浏览器打开也正常
- [ ] 跨父域部署的话，Safari 和 Chrome 都实测一遍 Cookie

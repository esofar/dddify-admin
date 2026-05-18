# Dddify Admin

## 简介

Dddify Admin 是基于 ASP.NET Core 与 React 的现代化中后台全栈解决方案。后端基于 DDD 与 Clean Architecture 构建，强调领域边界、业务解耦与可维护性；前端基于 Ant Design Pro 构建，提供用户、角色、权限、部门、数据字典等通用中后台能力，帮助团队快速搭建可扩展的企业级管理系统。

## 技术栈

后端：

- .NET 10
- ASP.NET Core
- Entity Framework Core
- Dddify
- PostgreSQL
- Redis
- JWT Bearer Authentication
- OpenAPI

前端：

- React 19
- TypeScript 6
- Umi Max 4
- Ant Design 6
- Ant Design Pro 6
- TanStack Query
- TailwindCSS

## 项目结构

```text
Dddify.Admin
├── src
│   ├── Dddify.Admin.Domain
│   │   ├── Aggregates
│   │   ├── Events
│   │   ├── Exceptions
│   │   └── Repositories
│   ├── Dddify.Admin.Application
│   │   ├── Commands
│   │   ├── Queries
│   │   ├── Dtos
│   │   ├── Exceptions
│   │   └── Common
│   ├── Dddify.Admin.Infrastructure
│   │   ├── Data
│   │   ├── Repositories
│   │   └── Services
│   └── Dddify.Admin.Web
│       ├── Controllers
│       ├── Requests
│       ├── Resources
│       └── ClientApp
└── tests
```

## 架构分层

Dddify Admin 的分层方式遵循 Dddify 推荐实践：领域模型保持纯粹，应用层表达业务用例，基础设施层承载技术细节，Web 层作为交付入口。依赖方向始终向内，外层依赖内层，内层不感知外层实现。

| 层 | 职责 | 典型内容 |
| --- | --- | --- |
| Domain | 领域模型与业务规则 | 聚合、实体、值对象、领域事件、领域异常、仓储接口 |
| Application | 用例编排 | Command、Query、DTO、应用异常、服务接口 |
| Infrastructure | 技术实现 | EF Core、Redis、仓储实现、外部服务 |
| Web | API 与前端入口 | Controller、Request、认证授权、异常包装、ClientApp |

## 内置模块

- 认证授权：支持账号登录、刷新令牌、退出登录与会话管理。
- 用户管理：支持用户维护、状态管理、角色分配与密码重置。
- 角色管理：支持角色维护、默认角色、内置角色与权限分配。
- 权限管理：内置目录、菜单、按钮三级权限模型。
- 部门管理：支持树形部门、负责人、部门类型与状态管理。
- 数据字典：支持字典与字典项维护、排序、预设与启用状态管理。

更多企业级模块正在持续开发中，包含安全审计、平台能力、企业组织等方面。

## 环境要求

- .NET SDK 10.0+
- Node.js 20+
- PostgreSQL
- Redis

## 快速开始

### 1. 克隆项目

```bash
git clone https://github.com/esofar/dddify-admin
cd dddify-admin
```

### 2. 配置后端

后端配置位于：

```text
src/Dddify.Admin.Web/appsettings.json
```

至少需要确认以下配置：

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=dddify_admin_v2;Username=app_user;Password=app_pwd"
  },
  "Redis": {
    "Configuration": "localhost:6379,password=app_pwd,abortConnect=false",
    "InstanceName": "dddify:"
  },
  "Jwt": {
    "Secret": "YourVeryStrongAndLongSecretKeyHere1234567890ABCDEF",
    "Issuer": "your_issuer",
    "Audience": "your_audience"
  }
}
```

生产环境请使用环境变量、密钥管理服务或部署平台 Secret 覆盖敏感配置。

### 3. 启动外部依赖服务

本地开发可以使用 Docker 快速启动 PostgreSQL 和 Redis：

```bash
docker run -d \
  --name redis \
  --restart unless-stopped \
  -p 6379:6379 \
  -v redis_data:/data \
  redis:8.6 \
  redis-server --appendonly yes --requirepass app_pwd
```

```bash
docker run -d \
  --name postgres \
  --restart unless-stopped \
  -e POSTGRES_USER=app_user \
  -e POSTGRES_PASSWORD=app_pwd \
  -e TZ=Asia/Shanghai \
  -p 5432:5432 \
  -v postgres_data:/var/lib/postgresql/data \
  postgres:17
```

如果容器已经存在，可以先启动已有容器：

```bash
docker start redis postgres
```

以上命令与默认 `appsettings.json` 中的连接配置保持一致。生产环境请按实际部署方式配置独立数据库、Redis、网络隔离、备份策略和访问凭据。

### 4. 数据库迁移

项目已初始化数据库迁移文件，首次运行时只需执行数据库更新命令即可完成表结构初始化，无需手动创建迁移。

```powershell
dotnet ef database update `
  --project src/Dddify.Admin.Infrastructure/Dddify.Admin.Infrastructure.csproj `
  --startup-project src/Dddify.Admin.Web/Dddify.Admin.Web.csproj `
  --context ApplicationDbContext
```

当实体、配置或数据库结构发生变更时，需要手动创建新的迁移文件，并提交到项目中统一管理。

```powershell
dotnet ef migrations add InitialCreate `
  --project src/Dddify.Admin.Infrastructure/Dddify.Admin.Infrastructure.csproj `
  --startup-project src/Dddify.Admin.Web/Dddify.Admin.Web.csproj `
  --context ApplicationDbContext `
  --output-dir Data/Migrations
```

### 5. 启动后端

```bash
dotnet run --project src/Dddify.Admin.Web/Dddify.Admin.Web.csproj
```

启动后可访问后端 API 与 OpenAPI 文档。具体端口以 `launchSettings.json` 或控制台输出为准。

### 6. 启动前端

```bash
cd src/Dddify.Admin.Web/ClientApp
npm install
npm run dev
```

前端默认通过开发服务器代理访问后端 API。代理配置请查看 `ClientApp` 下的 Umi 配置文件。

## 常用命令

后端：

```bash
dotnet restore
dotnet build
dotnet test
```

前端：

```bash
npm run dev
npm run build
npm run lint
npm run tsc
npm run biome
npm run openapi
```

## 开发规范

前端：

- 页面优先使用 ProTable、ModalForm、DrawerForm 等 Pro Components。
- 接口调用使用 `src/services/v1` 下由 OpenAPI 生成的 services。
- 表格请求统一适配 `{ data, total, success }`。
- 成功与失败提示优先使用现有国际化 key。
- 权限判断使用 `useAccess`。
- 枚举展示统一使用 `valueEnum`、Tag 或业务组件。
- 表单组件保持类型安全，避免无意义的 `any`。
- 国际化文案同时维护 `zh-CN` 与 `en-US`。


后端：

- 优先遵循 Dddify 项目实践，而不是为了示例简化架构边界。
- 领域规则放在 Domain，不在 Controller 中堆业务逻辑。
- Command 处理写操作，Query 处理读操作。
- Controller 只负责请求转换、授权和调用应用层。
- 基础设施实现放在 Infrastructure，领域层不直接依赖 EF Core、Redis 等技术实现。
- 应用异常用于表达用例失败，领域异常用于表达领域规则被破坏。
- 缓存、分布式锁等横切能力通过接口抽象。
- 数据库种子数据应保持稳定 ID，避免迁移反复抖动。

## 配置说明

### JWT

`Jwt` 用于访问令牌和刷新令牌配置：

- `Secret`：签名密钥。
- `Issuer`：签发方。
- `Audience`：受众。
- `AccessTokenMinutes`：访问令牌有效期。
- `RefreshTokenDays`：刷新令牌有效期。

### Redis

Redis 用于缓存、分布式锁、会话辅助能力等场景：

- `Configuration`：Redis 连接串。
- `InstanceName`：缓存 key 前缀。

## 初始数据

项目包含基础种子数据：

- 内置角色：超级管理员、管理员、普通用户、访客。
- 权限树：系统、用户、角色、权限、部门、字典等模块权限。
- 部门树：集团、产品中心、技术中心、职能中心及下属部门。
- 字典：部门类型。
- 用户：一组企业通讯录风格测试用户。


超级管理员角色账号：
```
邮箱账号：chengyuan.gu@xinghan.tech
初始密码：Admin123
```

生产环境初始化后立即修改默认账号密码。

## API 文档

项目后端已集成 OpenAPI 与 Scalar，启动 Web 项目后可通过浏览器访问 API 文档。

```text
https://localhost:7225/docs/
```

实际访问地址请以 Web 项目 `Program.cs` 中的  Scalar 配置为准。

## 部署建议

- 使用环境变量或 Secret 管理数据库、Redis、JWT 等敏感配置。
- 生产环境关闭敏感数据日志。
- 启用 HTTPS。
- 为 PostgreSQL 和 Redis 配置可靠备份。
- 多实例部署时，后台任务需要具备互斥能力，例如数据库抢占、Redis 分布式锁或消息队列消费者组。
- 为关键操作保留审计日志。

## 许可证

本项目基于仓库内 [LICENSE](./LICENSE) 文件授权。
